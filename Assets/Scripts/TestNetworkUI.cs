// NetworkUI.cs
// Basic UI for playing the game, choose between host/client

using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

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
        if (IsServer)
        {
            System.Random RND = new System.Random();
            levelSeed.Value = RND.Next(100);
            //Debug.Log("Server random completed, seed: " + levelSeed.Value);
        }
    }

    private void Awake() {
        hostBtn.onClick.AddListener(() => { StartGame(true); });
        clientBtn.onClick.AddListener(() => { StartGame(); });
    }

    private void Update() {
        // playerCountText.text = "Players: " + playerCount.Value.ToString();
        // if (!IsServer) return;
        // playerCount.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }

    private void StartGame(bool isHost = false)
    {
        if (isHost) { NetworkManager.Singleton.StartHost(); }
        else { NetworkManager.Singleton.StartClient(); }
        level.PopulateLevel(levelSeed.Value);
        enabled = false;
        //Debug.Log("Starting game! Seed = " + levelSeed.Value);
    }
}
