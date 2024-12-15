using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Systems.Scriptable.Events;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Multiplay;
using UnityEngine;

namespace Systems.Multiplayer.Manager
{
    public class MultiplayerManager : PersistentSingleton<MultiplayerManager>
    {
        private bool isOfflineSavePending = false;
        public bool isNetworkAvailable = true; 

        [SerializeField] private TMP_InputField ipAddressInputField;
        [SerializeField] private TMP_InputField portInputField;

        private IServerQueryHandler serverQueryHandler;
        protected async override void Awake()
        {
            base.Awake();
            if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            {
                try
                {
                    await UnityServices.InitializeAsync();
                    Debug.Log("Unity Services initialized successfully.");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to initialize Unity Services: {e.Message}");
                    return;  // Avoid further execution if initialization fails
                }
            }
        }
        private async void Start()
        {
            if (Application.platform == RuntimePlatform.LinuxServer)
            {
                Application.targetFrameRate = 60;

                await UnityServices.InitializeAsync();

                ServerConfig serverConfig = MultiplayService.Instance.ServerConfig;

                serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(10, "MyServer", "MyGameType", "0", "TestMap");

                if (serverConfig.AllocationId != string.Empty)
                {
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("0.0.0.0", serverConfig.Port, "0.0.0.0");

                    NetworkManager.Singleton.StartServer();
                }
            }
        }

        private async void Update()
        {
            isNetworkAvailable = IsNetworkAvailable();
            if (isNetworkAvailable && isOfflineSavePending)
            {
                Observer.Instance.Notify("onNetworkAvailable");
                isOfflineSavePending = false; // Đặt lại sau khi đã lưu thành công
            }
            else if (!isNetworkAvailable)
            {
                Observer.Instance.Notify("onNetworkDisable");
                isOfflineSavePending = true;
            }
            if (Application.platform == RuntimePlatform.LinuxServer)
            {
                if (serverQueryHandler != null)
                {
                    serverQueryHandler.CurrentPlayers = (ushort)NetworkManager.Singleton.ConnectedClientsList.Count;
                    serverQueryHandler.UpdateServerCheck();
                    await Task.Delay(100);
                }
            }
        }

        public void JoinToServer()
        {
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            transport.SetConnectionData(ipAddressInputField.text, ushort.Parse(portInputField.text));

            NetworkManager.Singleton.StartClient();
        }
        private bool IsNetworkAvailable()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }
}
