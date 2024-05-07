/* Originally from David, modified by Jonah
 * Controls bomb intantiation
 */

using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Tilemaps;

public class BombController : NetworkBehaviour
{
    [Header("Bomb")]
    public GameObject bombPrefeb;
    public KeyCode inputKey = KeyCode.Space;
    public float bombFuseTime = 3f;
    public int bombAmount = 3;
    private int bombsRemaining;

    [Header ("Explosion")]
    public Explosion explosionPrefab;
    public LayerMask explosionLayerMask;
    public float explosionDuration = 1f;
    public int explosionRadius = 1;

    [Header("Destructable")]
    public Tilemap destructableTiles;
    public Destructable destructiblePrefab;

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        if(!IsOwner) {
            this.enabled = false;
            return;
        }
    }

    private void OnEnable() 
    {
        bombsRemaining = bombAmount;
    }

    private void Update() 
    {
        if (bombsRemaining > 0 && Input.GetKeyDown(inputKey) && IsOwner) 
        {
            bombsRemaining--;
            Vector2 position = transform.position;
            position.x = Mathf.Round(position.x);
            position.y = Mathf.Round(position.y);
            PlaceBombServerRpc(position);
            bombsRemaining++;
        }
    }

    // New bomb spawn idea; create a script for bomb, have RPC that just spawns a bomb
    [ServerRpc(RequireOwnership = false)]
    private void PlaceBombServerRpc(Vector2 position) {
        GameObject bomb = Instantiate(bombPrefeb, position, Quaternion.identity);
        bomb.GetComponent<Bomb>().explosionRadius = explosionRadius;
        bomb.GetComponent<NetworkObject>().Spawn();
        Destroy(bomb, bombFuseTime + explosionDuration);
    }

    /*
    // Modified from PlaceBomb() to be a ServerRpc
    [ServerRpc(RequireOwnership = false)]
    private void PlaceBombServerRpc(Vector2 position) {
        GameObject bomb = Instantiate(bombPrefeb, position, Quaternion.identity);
        bomb.GetComponent<NetworkObject>().Spawn();
        StartCoroutine(FuseTimer(position));
        Destroy(bomb, bombFuseTime);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DetonateServerRpc(Vector2 position) {
        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.GetComponent<NetworkObject>().Spawn();
        explosion.SetActiveRenderer(explosion.start);
        explosion.DestroyAfter(explosionDuration);

        // TODO: send client data to explosion for score tracking

        Explode(position, Vector2.up, explosionRadius);
        Explode(position, Vector2.down, explosionRadius);
        Explode(position, Vector2.left, explosionRadius);
        Explode(position, Vector2.right, explosionRadius);
    }

    private IEnumerator FuseTimer(Vector2 position) {
        yield return new WaitForSeconds(bombFuseTime);
        DetonateServerRpc(position);
    }

    private void Explode(Vector2 position, Vector2 direction, int length) 
    {
        if (length <= 0) 
        {
            return;
        }

        position += direction;

        if(Physics2D.OverlapBox(position, Vector2.one /2, 0f, explosionLayerMask))
        {
            ClearDestructable(position);
            return;
        }

        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(length>1 ? explosion.middle : explosion.end);
        explosion.SetDirection(direction);
        explosion.DestroyAfter(explosionDuration);

        Explode(position, direction, length - 1);
    }

    private void ClearDestructable(Vector2 position) 
    {
        Vector3Int cell = destructableTiles.WorldToCell(position);
        TileBase tile = destructableTiles.GetTile(cell);

        if (tile != null) 
        {
            Instantiate(destructiblePrefab, position, Quaternion.identity);
            destructableTiles.SetTile(cell, null);
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bomb")) 
        {
            other.isTrigger = false;
        }
    }
    */

    public void AddBomb()
    {
        bombAmount++;
        bombsRemaining++;
    }

}
