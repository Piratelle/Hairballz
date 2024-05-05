using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;



public class PlayerSpawnPoint : NetworkBehaviour
{
    public GameObject myPrefab;

    public override void OnNetworkSpawn()
    {
        SpawnPlayer();

    }

    private void SpawnPlayer()
    {
        var instance = Instantiate(NetworkManager.GetNetworkPrefabOverride(myPrefab));
        var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
    }
}
