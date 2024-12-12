using UnityEngine;
using System.IO;
using Systems.Account.Manager;
using Systems.Account.Model;
using Systems.Account.Enum;
using System;
using Systems.SaveLoad.Service;
using Systems.SaveLoad.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
using Systems.Multiplayer.Manager;
using Systems.Account.Service;
using Systems.Scriptable.Events;
using Systems.Hero.Manager;
using Systems.Hero.Model;
using Unity.VisualScripting;
using JetBrains.Annotations;

namespace Systems.SaveLoad.Manager
{
    public class SaveLoadManager : PersistentSingleton<SaveLoadManager>
    {
        public Dictionary<string, Func<Task>> saveActions;
        public List<string> listNameFailedSaves = new List<string>();
        public string dataPath;
        public SaveLoadLocalService saveLoadLocalService;
        private void OnEnable()
        {
            Observer.Instance.AddObserver("onLoginAccount", OnLogin);
            Observer.Instance.AddObserver("onLogoutAccount", OnLogout);
            Observer.Instance.AddObserver("onDeleteAccount", OnDeleteAccount);

            Observer.Instance.AddObserver("onLeaveGame", OnLeaveGame);

            Observer.Instance.AddObserver("onNetworkDisable", OnNetworkDisable);
            Observer.Instance.AddObserver("onNetworkAvailable", OnNetworkAvailable);
        }

        private void OnDisable()
        {
            Observer.Instance.RemoveListener("onLoginAccount", OnLogin);
            Observer.Instance.RemoveListener("onLogoutAccount", OnLogout);
            Observer.Instance.RemoveListener("onDeleteAccount", OnDeleteAccount);

            Observer.Instance.RemoveListener("onLeaveGame", OnLeaveGame);

            Observer.Instance.RemoveListener("onNetworkDisable", OnNetworkDisable);
            Observer.Instance.RemoveListener("onNetworkAvailable", OnNetworkAvailable);
        }

        protected override void Awake()
        {
            base.Awake();
            dataPath = Application.persistentDataPath;
            saveLoadLocalService = new SaveLoadLocalService(dataPath);

        }
        public void Start()
        {
            saveActions = new Dictionary<string, Func<Task>>
            {
                { "account", async () => await SaveAccountData() },
                { "playerData", async () => await SavePlayerData() },
                // Thêm các hành động khác nếu cần
                // { "setting", async () => await SaveSettingData() }
            };
        }
        private async void OnApplicationQuit()
        {
            await SaveData();
        }
        private async void OnLogin(object[] obj)
        {
            await LoadAccountData();
        }

        private async Task LoadAccountData()
        {
            // # Check kiểm tra tính toàn vẹn của version file save in local

            // Nếu có thời gian sẽ triển khai backup
            if (SignInResult.AccountType == AccountType.Anonymous)
            {
                AccountManager.Instance.accountData = await saveLoadLocalService.LoadAsync<AccountData>(SignInResult.IdPlayer, "account");
                PlayerDataManager.Instance.playerData = await saveLoadLocalService.LoadAsync<PlayerData>(SignInResult.IdPlayer, "playerData");
                Debug.Log($"SaveLoadManager :{PlayerDataManager.Instance.playerData.position}");
                return;
            }
            // Check đối chiếu kiểm tra version save mới nhất giữa Local và Cloud
        }

        private async void OnLogout(object[] obj)
        {
            await SaveData();
            ResetListNameFailedSave();
            Debug.Log("IsLogout True");
        }

        private void OnDeleteAccount(object[] obj)
        {
            saveLoadLocalService.DeleteFolder(SignInResult.IdPlayer);
            // Xóa tài khoản trên Cloud
            ResetListNameFailedSave();
        }
        private async void OnLeaveGame(object[] obj)
        {
            await SaveData();
        }

        private async void OnNetworkAvailable(object[] obj)
        {
            await SaveData();
        }
        private async void OnNetworkDisable(object[] obj)
        {
            await SaveData();
        }
        // private async Task SaveSettingData()
        // {
        //     SettingManager.Instance.IsModified = false;
        //     await saveLoadLocalService.SaveAsync<SettingManager>(SignInResult.IdPlayer, "setting", SettingManager.Instance.setting);
        // }
        public async Task SaveData(AccountData newAccountData)
        {
            PlayerData newPlayerData = new PlayerData();

            // Lưu dữ liệu tài khoản và dữ liệu người chơi
            await saveLoadLocalService.SaveAsync<AccountData>(SignInResult.IdPlayer, "account", newAccountData);
            await saveLoadLocalService.SaveAsync<PlayerData>(SignInResult.IdPlayer, "playerData", newPlayerData);

            // AccountManager.Instance.accountData = newAccountData;
            // PlayerDataManager.Instance.playerData = newPlayerData;
        }
        public async Task SaveData()
        {
            await RetryFailedSaves();
            var changes = saveLoadLocalService.trackableService.GetAllChanges();

            foreach (var change in changes)
            {
                if (change.Value == true && saveActions.TryGetValue(change.Key, out var saveAction))
                {
                    bool saveSuccess = false;

                    // Thử lưu và ghi lại trạng thái thất bại nếu lưu không thành công
                    try
                    {
                        await saveAction();
                        saveSuccess = true; // Lưu thành công
                        if (listNameFailedSaves.Contains(change.Key))
                            listNameFailedSaves.Remove(change.Key);
                    }
                    catch (Exception)
                    {
                        Debug.Log("SavePlayerData Error");
                    }

                    // Nếu lưu không thành công, thêm vào danh sách thất bại
                    if (!saveSuccess && !listNameFailedSaves.Contains(change.Key))
                    {
                        listNameFailedSaves.Add(change.Key);
                    }
                }
            }
            saveLoadLocalService.trackableService.ResetChanges();
            // Thử lưu lại các đối tượng không lưu được
            await RetryFailedSaves();
        }
        private async Task SaveAccountData()
        {
            bool isLoggedIn = SignInResult.AccountType == AccountType.Player;
            await saveLoadLocalService.SaveAsync<AccountData>(SignInResult.IdPlayer, "account", AccountManager.Instance.accountData);
            AccountManager.Instance.IsModified = false;
            try
            {
                if (isLoggedIn && MultiplayerManager.Instance.isNetworkAvailable)
                {
                    // Xử lý lưu dữ liệu lên cloud
                }
            }
            catch (Exception)
            {
                Debug.Log("Save HeroData Cloud Error");
            }
        }
        private async Task SavePlayerData()
        {
            bool isLoggedIn = SignInResult.AccountType == AccountType.Player;
            
            await saveLoadLocalService.SaveAsync<PlayerData>(SignInResult.IdPlayer, "playerData", PlayerDataManager.Instance.playerData);

            PlayerDataManager.Instance.isModified = false;
            try
            {
                if (isLoggedIn && MultiplayerManager.Instance.isNetworkAvailable)
                {
                    // Xử lý lưu dữ liệu lên cloud
                }
            }
            catch (Exception)
            {
                Debug.Log("Save HeroData Cloud Error");
            }
        }
        private async Task RetryFailedSaves()
        {
            foreach (var key in listNameFailedSaves.ToArray())
            {
                bool saveSuccess = false;

                try
                {
                    var saveAction = saveActions[key];
                    await saveAction();
                    saveSuccess = true;
                }
                catch (Exception)
                {

                }

                if (saveSuccess)
                {
                    listNameFailedSaves.Remove(key);
                }
            }
        }
        private void ResetListNameFailedSave()
        {
            listNameFailedSaves.Clear();
        }
    }
}
