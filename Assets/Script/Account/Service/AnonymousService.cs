using System.Collections.Generic;
using System.Threading.Tasks;
using Systems.Account.Enum;
using Systems.Account.Manager;
using Systems.Account.Model;
using Systems.SaveLoad.Manager;
using Systems.SaveLoad.Model;
using Systems.SaveLoad.Service;
using Systems.Scriptable.Events;

namespace Systems.Account.Service
{
    public class AnonymousService
    {
        public async Task SignInAnonymous()
        {
            SignInResult.AccountType = AccountType.Anonymous;
            SignInResult.IdPlayer = "AnonymousAccount";
            if (!SaveLoadManager.Instance.saveLoadLocalService.CheckAnonymousFolderExists())
            {
                AccountData playerData = new AccountData();
                await SaveLoadManager.Instance.SaveData(playerData);
            }

            Observer.Instance.Notify("onLoginAccount");
        }
    }
}