using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using TMPro;

public class ConnectionManager : MonoBehaviour
{
    public TMP_InputField joinCodeInput;
    public TMP_Text joinCodeText;

    public async void StartHost()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn) await AuthenticationService.Instance.SignInAnonymouslyAsync();
        
        //allocate max 4 player slots
        var allocation = await RelayService.Instance.CreateAllocationAsync(4);
        
        //get server info: ip, port, allocation ID etc;
        //connect via relay not ip and port
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "udp"));
        
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        joinCodeText.text = "Join code: " + joinCode;
        
        NetworkManager.Singleton.StartHost();
    }

    public async void StartClient()
    {
        await UnityServices.InitializeAsync();
        if(!AuthenticationService.Instance.IsSignedIn) await AuthenticationService.Instance.SignInAnonymouslyAsync();
        
        var joinCode = joinCodeInput.text.Trim();
        var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "udp"));
    }
}
