// LobbyListSingleUI.cs
// handles joinable Lobby UI

using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshPro lobbyNameText;

    private Lobby lobby;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>{
            GameLobby.Instance.JoinWithId(lobby.Id);
        });
    }

    public void SetLobby(Lobby lobby)
    {
        this.lobby = lobby;
        lobbyNameText.text = lobby.Name;
    }
}
