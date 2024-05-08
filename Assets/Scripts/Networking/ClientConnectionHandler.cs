using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ClientConnectionHandler : Singleton<ClientConnectionHandler>
{
    private static readonly int PLAYER_COUNT = 4;
    private static readonly SortedSet<int> availablePlayers = new();
    private static readonly Dictionary<ulong, int> clientPlayers = new();

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += AddPlayer;
        NetworkManager.Singleton.OnClientDisconnectCallback += RemovePlayer;

        // build initial list of available player numbers
        if (availablePlayers.Count > 0) return;
        for (int i = 0; i < PLAYER_COUNT; i++)
        {
            availablePlayers.Add(i);
        }
    }

    public static int GetPlayerNum(ulong clientId)
    {
        if (clientPlayers.ContainsKey(clientId)) return clientPlayers[clientId];
        return 0;
    }

    private void AddPlayer(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (NetworkManager.Singleton.ConnectedClients.Count >= PLAYER_COUNT)
        {
            response.Approved = false;
            response.Reason = "Player limit reached.";
            Debug.Log("Connection from Client " + request.ClientNetworkId + " refused as game is full.");
        } else
        {
            response.Approved = true;
            int playerNum = availablePlayers.First();
            availablePlayers.Remove(playerNum);
            clientPlayers.Add(request.ClientNetworkId, playerNum);
            Debug.Log("Connection from Client " + request.ClientNetworkId + " approved as Player " + (playerNum + 1));
        }
        response.Pending = false;
    }

    private void RemovePlayer(ulong clientId)
    {
        if (clientPlayers.Count == 0) return; // connections are only tracked on the server!

        int playerNum = clientPlayers[clientId];
        clientPlayers.Remove(clientId);
        availablePlayers.Add(playerNum);
        Debug.Log("Client " + clientId + " disconnected from Player " + (playerNum + 1));
    }
}
