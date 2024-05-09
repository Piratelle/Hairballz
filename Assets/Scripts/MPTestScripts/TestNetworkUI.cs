// NetworkUI.cs
// Basic UI for playing the game, choose between host/client

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestNetworkUI : NetworkBehaviour
{
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    // [SerializeField] private TextMeshProUGUI playerCountText;

    private NetworkVariable<int> playerCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    private void Awake() {
        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            this.enabled = false;
        });
        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            this.enabled = false;
        });
    }

    private void Update() {
        // playerCountText.text = "Players: " + playerCount.Value.ToString();
        // if (!IsServer) return;
        // playerCount.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }
}
