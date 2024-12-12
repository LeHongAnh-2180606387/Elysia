using System.Collections.Generic;
using Systems.Account.Enum;
using Systems.Account.Model;
using Unity.Services.CloudSave;

namespace Systems.SaveLoad.Service
{
    public class SaveLoadCloudServerUnityService
    {
        public async static void SaveData(string key, object inData)
		{
            if (SignInResult.AccountType == AccountType.Player)
            {
                var data = new Dictionary<string, object> { { key, inData } };
                await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            }
		}
    }
}