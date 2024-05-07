using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;

public class PAController : NetworkBehaviour
{

    public GameObject bombPrefeb;
    public KeyCode inputKey = KeyCode.Space;

    public static event EventHandler OnAnyPlayerSpawned;
    private Rigidbody2D rb2D;

    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
    }

    public static PAController LocalInstance { get; private set; }
    
    [SerializeField] private List<Vector2> spawnPointList;

    void Start()
    {
        PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.isKinematic = false;
    }

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            LocalInstance = this;
        }

        transform.position = spawnPointList[GameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
        if(IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientID)
    {
        if(clientID == OwnerClientId )
        {
            return;
            //had originally different purpose - to destroy instances of objects held, could be used for bombs?
        }
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    void Update()//checks if the correct user is controlling the corrects player
    {
        if(!IsOwner)
        {
            return;
        }

        if(Input.GetKeyDown(inputKey))
        {

            SpawnBombServerRpc();
            
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnBombServerRpc()
    {
        Transform playerTransform = GetComponent<Transform>();
        Vector2 spawnPoint = playerTransform.position;

        spawnPoint.x = Mathf.Round(spawnPoint.x);
        spawnPoint.y = Mathf.Round(spawnPoint.y);

        var instance = Instantiate(bombPrefeb, spawnPoint,Quaternion.identity);
        var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
        
    }
}