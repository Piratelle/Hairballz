using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Threading.Tasks;

public class TestLobby : MonoBehaviour
{

    private Lobby hostLobby;
    private float heartbeatTimer;

    private string playerName;

    // Start is called before the first frame update
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        // Sign in anonymously
        await SignInAnonymouslyAsync();

        playerName = "Player" + UnityEngine.Random.Range(10, 99);
        Debug.Log(playerName);
    }//end start

    private void Update()
    {
        HandleLobbyHeartBeat();

        QuickJoinLobby();
        //PrintPlayers(hostLobby);
    }//end update

    private async void HandleLobbyHeartBeat()
    {
        if(hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if(heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }//end handlelobby heartbeat


    // Method for anonymous sign-in
    private async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");

            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // After signing in, you can proceed with other actions like creating or listing lobbies
            // For example:
             CreatePublicLobby();
            // ListLobbies();
            


        }
        catch (AuthenticationException ex)
        {
            // Handle authentication error
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Handle request failed error
            Debug.LogException(ex);
        }
    }//end anon log in

    private async void CreatePublicLobby()
    {
        try
        {
            string lobbyName = "MyLobby";
            int maxPlayers = 4;

            // Create lobby options
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = false;

            // Create a dictionary to hold player data, including display name
            Dictionary<string, PlayerDataObject> playerData = new Dictionary<string, PlayerDataObject>();
            playerData["displayName"] = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName);

            // Create a player object and assign the data
            Player player = new Player
            {
                Data = playerData
            };

            // Assign the player to options
            options.Player = player;

            // Create the lobby
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            // Store the created lobby
            hostLobby = lobby;

            // Print the players in the lobby
            PrintPlayers(hostLobby);

            // Log a message indicating that the lobby was successfully created
            Debug.Log("Created Lobby! " + lobby.Name + " " + lobby.MaxPlayers);
        }
        catch (LobbyServiceException e)
        {
            // Handle any exceptions that might occur during lobby creation
            Debug.Log(e);
        }
    }


    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };
    }




    private async void CreatePrivateLobby()
    {
        try
        {
            string lobbyName = "MyLobby";
            int maxPlayers = 4;
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = true;
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            
            hostLobby = lobby;

            PrintPlayers(hostLobby);
            Debug.Log("Created Lobby! " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }


    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder>()
            {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(options);

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyCode);
            Debug.Log("Joined Lobby with Code " + lobbyCode);
            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }



    private async void QuickJoinLobby ()
    {
        try
        {
            // Quick-join a random lobby with a maximum capacity of 10 or more players.
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

            options.Filter = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.MaxPlayers,
                    op: QueryFilter.OpOptions.GE,
                    value: "4") // was previously 10 from documentation, this should be the max players in a lobby i think
            };

            await LobbyService.Instance.QuickJoinLobbyAsync(options);

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }


    private void PrintPlayers (Lobby lobby)
    {
        Debug.Log("Players in Lobby " + lobby.Name);
        foreach(Player player in lobby.Players)
        {
            Debug.Log(player.Id);
        }
    }


}
