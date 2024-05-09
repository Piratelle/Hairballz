/* Originally from David, modified by Jonah
 * Controls bomb instantiation
 */

using UnityEngine;
using Unity.Netcode;
//using UnityEngine.Tilemaps;

public class BombController : NetworkBehaviour
{
    private int playerNum;
    [Header("Bomb")]
    public GameObject bombPrefeb;
    public KeyCode inputKey = KeyCode.Space;
    public float bombFuseTime = 3f;
    public int bombStartAmount = 3;
    private int bombsRemaining;

    [Header ("Explosion")]
    public Explosion explosionPrefab;
    public LayerMask explosionLayerMask;
    public float explosionDuration = 1f;
    [SerializeField] private int explosionRadius = 1;

    [Header("Destructable")]
    //public Tilemap destructableTiles;
    public Destructable destructablePrefab;

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        if(!IsOwner) {
            enabled = false;
            return;
        }
        explosionRadius = 1;
        playerNum = GetComponent<PlayerController>().GetPlayerNum();
    }

    private void OnEnable() 
    {
        bombsRemaining = bombStartAmount;
    }

    private void Update() 
    {
        if (bombsRemaining == 1) this.explosionRadius = 3;
        if (bombsRemaining > 0 && Input.GetKeyDown(inputKey) && IsOwner) 
        {
            bombsRemaining--;
            Vector2 position = transform.position;
            position.x = Mathf.Round(position.x);
            position.y = Mathf.Round(position.y);
            PlaceBombServerRpc(position, explosionRadius);
        }
    }

    // New bomb spawn idea; create a script for bomb, have RPC that just spawns a bomb
    [ServerRpc(RequireOwnership = false)]
    private void PlaceBombServerRpc(Vector2 position, int r) {
        GameObject bomb = Instantiate(bombPrefeb, position, Quaternion.identity);
        bomb.GetComponent<Bomb>().SetExplosionRadius(r);
        bomb.GetComponent<Bomb>().SetPlayerNum(playerNum);
        bomb.GetComponent<NetworkObject>().Spawn();
        Destroy(bomb, bombFuseTime + explosionDuration);
    }

    public void AddBomb()
    {
        bombsRemaining++;
    }

    public void IncrementExplosionRadius() {
        explosionRadius++;
    }

    public int GetExplosionRadius() {
        return explosionRadius;
    }

}
