using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Systems.SaveLoad.Service;
using Unity.Services.Core;
using System;
using Systems.Account.Manager;
using Systems.Account.Model;
using Systems.Account.Enum;
using Systems.SaveLoad.Manager;
using Systems.Scriptable.Events;
using Systems.Hero.Manager;
public class GameManager : PersistentSingleton<GameManager>
{
    public bool isPlayingInMap = false;
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {

    }
    private void OnEnable()
    {
        Observer.Instance.AddObserver("onLoginAccount", OnLogin);
        Observer.Instance.AddObserver("onLogoutAccount", OnLogout);
        Observer.Instance.AddObserver("onDeleteAccount", OnDeleteAccount);

        Observer.Instance.AddObserver("onNetworkDisable", OnNetworkDisable);
        Observer.Instance.AddObserver("onNetworkAvailable", OnNetworkAvailable);
    }

    private void OnDisable()
    {
        Observer.Instance.RemoveListener("onLoginAccount", OnLogin);
        Observer.Instance.RemoveListener("onLogoutAccount", OnLogout);
        Observer.Instance.RemoveListener("onDeleteAccount", OnDeleteAccount);

        Observer.Instance.RemoveListener("onNetworkDisable", OnNetworkDisable);
        Observer.Instance.RemoveListener("onNetworkAvailable", OnNetworkAvailable);
    }
    private void OnLogin(object[] obj)
    {
        throw new NotImplementedException();
    }
    private void OnLogout(object[] obj)
    {
        ResetPlayerData();
    }
    public void ResetPlayerData()
    {
        AccountManager.Instance.accountData = null;
        PlayerDataManager.Instance.playerData = null;
    }
    private void OnDeleteAccount(object[] obj)
    {
        ResetPlayerData();
    }
    private void OnNetworkAvailable(object[] obj)
    {
        throw new NotImplementedException();
    }
    private void OnNetworkDisable(object[] obj)
    {
        throw new NotImplementedException();
    }
    private void PlayGame()
    {
        
        Observer.Instance.Notify("onPlayGame");
        isPlayingInMap = true;
        // Load Scene Map Hero Playing
    }
    public void LeaveGame()
    {
        isPlayingInMap = false;
        Observer.Instance.Notify("onLeaveGame");
        // Leave Scene Map Hero Playing
    }
}
