using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Multiplay;
using Systems.Hero.Manager;
using Systems.Scriptable.Events;

namespace Systems.Multiplayer.Manager
{
    public class RelayManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI joinCodeText;
        [SerializeField] private TMP_InputField joinCodeInputField;
        private string joinCode;
        private void Start()
        {
            // await UnityServices.InitializeAsync();
            // if (!AuthenticationService.Instance.IsSignedIn)
            // {
            //     await AuthenticationService.Instance.SignInAnonymouslyAsync();
            // }
        }
        public async void StartRelay()
        {
            joinCode = await StartHostWithRelay();
            // ...
            PlayerDataManager.Instance.playerHero = GameObject.FindWithTag("Player");
            Debug.Log($"JoinCode: {joinCode}");
            joinCodeText.text = joinCode;
        }
        public async void JoinRelay()
        {
            if (joinCodeInputField.text == "")
            {
                Debug.LogError("Join code is empty");
                return;
            }
            bool joined = await StartClientWithRelay(joinCodeInputField.text);
            // PlayerDataManager.Instance.playerHero = GameObject.FindWithTag("Player");
            if (joined)
            {
                joinCodeInputField.text = "";
            }
        }

        public void QuitHostTest()
        {
            QuitHost();
            Observer.Instance.Notify("onLeaveGame");
        }
        private async Task<string> StartHostWithRelay(int maxConnections = 4)
        {
            if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            {
                await UnityServices.InitializeAsync();
            }
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return NetworkManager.Singleton.StartHost() ? joinCode : null;
        }

        private async Task<bool> StartClientWithRelay(string joinCode)
        {
            if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            {
                await UnityServices.InitializeAsync();
            }
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
        }
        private void QuitHost()
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("Host đã ngắt kết nối.");
        }
    }

}
