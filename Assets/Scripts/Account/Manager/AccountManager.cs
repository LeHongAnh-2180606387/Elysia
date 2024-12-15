using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine.UIElements;
using TMPro;
using Unity.VisualScripting;
using System;
using System.Security.Authentication;
using Systems.SaveLoad.Model;
using Systems.SaveLoad.Manager;
using Systems.Account.Service;
using Systems.Account.Enum;
using Systems.Account.Model;
using UnityEngine.InputSystem.Utilities;
using Systems.Scriptable.Events;

namespace Systems.Account.Manager
{
    // Chưa sử lý liên kết
    // Chưa sử lý đồng bộ sau khi liên kết
    public class AccountManager : PersistentSingleton<AccountManager>
    {
        public AccountService accountService = new AccountService();
        public AnonymousService anonymousAccount = new AnonymousService();
        public AccountData accountData;
        public bool IsModified = false;
        protected override void Awake()
        {
            base.Awake();
        }
        public void Start()
        {
            accountService.SetAttributeAccount();

            // Action OnPlayerLogin ? <skip login feature UI>
        }
        public async void LoginAccountAnonymous()
        {
            await anonymousAccount.SignInAnonymous();
        }
        public void LogoutAccount()
        {
            accountService.LogoutAccount();
        }
        public void DeleteAccount()
        {
            accountService.DeleteAccount();
        }
    }
}
