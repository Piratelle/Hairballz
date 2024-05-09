using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEditor;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.Tilemaps;

public class PAController : NetworkBehaviour
{
    [SerializeField] private GameObject camHolder;
    public static event EventHandler OnAnyPlayerSpawned;

    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
    }

    public static PAController LocalInstance {get; private set;}

    [SerializeField] private List<Vector2> spawnPositionList;



    void Start()
    {
        GameObject destructableTilemapObject = GameObject.FindGameObjectWithTag("DestructableTilemap");
        PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        destructableTiles = destructableTilemapObject.GetComponent<Tilemap>();
    }

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            LocalInstance = this;
        }
        if(!IsOwner)
        {
            this.camHolder.SetActive(false);
        }

        transform.position = spawnPositionList[GameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        if(IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
        base.OnNetworkSpawn();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        return; //do nothing?
    }

    public GameObject bombPrefeb;
    public KeyCode inputKey = KeyCode.Space;

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
        StartCoroutine(Timer(instanceNetworkObject));
        
    }

    public GameObject explosionStartPrefeb;
    public GameObject explosionMiddlePrefeb;
    public GameObject explosionEndPrefeb;
    public float explosionDuration = 1f;
    public int explosionRadius = 1;
    public LayerMask explosionLayerMask;

    private IEnumerator Timer(NetworkObject instanceNetworkObject)
    {
        yield return new WaitForSeconds(3f);//bomb countdown

        //position of explosion = position of bomb
        Vector2 position = instanceNetworkObject.transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
        
        //bringing in our explosion
        var instance2 = Instantiate(explosionStartPrefeb, position, Quaternion.identity);
        var instanceNetworkObject2 = instance2.GetComponent<NetworkObject>();
        instanceNetworkObject2.Spawn();//explosion prefab has everything disabled at start, so when network spawned, its just invisible
        StartCoroutine(ExplosionTimer(instanceNetworkObject2));
        

        Explode(position, Vector2.up, explosionRadius);
        Explode(position, Vector2.down, explosionRadius);
        Explode(position, Vector2.left, explosionRadius);
        Explode(position, Vector2.right, explosionRadius);

        //Despawning our placed bomb
        instanceNetworkObject.DontDestroyWithOwner = true;
        instanceNetworkObject.Despawn();
    
    }

    private IEnumerator ExplosionTimer(NetworkObject instanceNetworkObject2)
    {
        yield return new WaitForSeconds(1f);//explosion countdown
        instanceNetworkObject2.DontDestroyWithOwner = true;
        instanceNetworkObject2.Despawn();
    
    }

    private void Explode(Vector2 position, Vector2 direction, int length)
    {
        if(length <= 0)
        {
            return;
        }

        position += direction;

        //check collisions
        if(Physics2D.OverlapBox(position, Vector2.one /2f, 0f, explosionLayerMask))
        {
            Debug.Log("Collided with wall");
            ClearDestructableClientRpc(position);
            return;
        }

        Quaternion rotation = SetDirection(direction);

        if(length > 1)
        {
            
            var instance3 = Instantiate(explosionMiddlePrefeb, position, rotation);
            var instanceNetworkObject3 = instance3.GetComponent<NetworkObject>();
            instanceNetworkObject3.Spawn();
            StartCoroutine(ExplosionTimer(instanceNetworkObject3));
        }
        else
        {
            var instance3 = Instantiate(explosionEndPrefeb, position, rotation);
            var instanceNetworkObject3 = instance3.GetComponent<NetworkObject>();
            instanceNetworkObject3.Spawn();
            StartCoroutine(ExplosionTimer(instanceNetworkObject3));
        }

        Explode(position, direction, length -1);
    }

    public Quaternion SetDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x);
        return Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    }





    public Tilemap destructableTiles;
    public Destructable destructablePrefab;

    [ClientRpc(RequireOwnership = false)]
    private void ClearDestructableClientRpc(Vector2 position)
    {
        Debug.LogWarning("Collided with Destructable");
        Vector3Int cell = destructableTiles.WorldToCell(position);
        TileBase tile = destructableTiles.GetTile(cell);

        if (tile != null) 
        {
            Debug.LogError("Tile not null");
            //Instantiate(destructablePrefab, position, Quaternion.identity);//change to network spawn code
            var instance = Instantiate(destructablePrefab, position,Quaternion.identity);
            var instanceNetworkObject = instance.GetComponent<NetworkObject>();
            instanceNetworkObject.Spawn();
            destructableTiles.SetTile(cell, null);
        }

    }


}
