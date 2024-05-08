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
    [SerializeField] private Level level;
    // [SerializeField] private TextMeshProUGUI playerCountText;

    private NetworkVariable<int> playerCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    private NetworkVariable<int> levelSeed = new NetworkVariable<int>(0);

    public override void OnNetworkSpawn()
    {
        levelSeed.OnValueChanged += OnLevelSeedChanged;
        if (IsServer)
        {
            System.Random RND = new System.Random();
            levelSeed.Value = RND.Next(100);
        }
    }

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

    private void OnLevelSeedChanged(int previousValue, int newValue)
    {
        level.PopulateLevel(newValue);
    }
}
