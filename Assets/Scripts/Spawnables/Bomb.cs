/* Jonah Hedlund
 * Bomb script
 * When bomb is spawned, starts fuse, then explodes
 *
 */

using UnityEngine;
using UnityEngine.Tilemaps;

public class Bomb : MonoBehaviour
{
    private int playerNum;
    Vector2 position;
    public Explosion explosionPrefab;
    public LayerMask explosionLayerMask;
    public float explosionDuration = 1f;
    [SerializeField] private int explosionRadius;
    int bombFuseTime = 3;

    // TODO: add sounds

    [Header("Destructable")]
    public Tilemap destructableTiles;
    public Destructable destructiblePrefab;

    [SerializeField] private AudioSource explodeAudioSource;
    [SerializeField] private AudioClip explodeSound;

    private void Start()
    {
        position = this.transform.position;
        explodeAudioSource = GetComponent<AudioSource>();
        Invoke("Detonate", bombFuseTime);
        // Destroy() is handled by BombController serverrpc
    }

    private void Detonate() {
        GetComponent<SpriteRenderer>().enabled = false;
        explodeAudioSource.clip = explodeSound;
        explodeAudioSource.Play();
        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.GetComponent<Explosion>().SetPlayerNum(playerNum);
        explosion.SetActiveRenderer(explosion.start);
        explosion.DestroyAfter(explosionDuration);

        Explode(position, Vector2.up, explosionRadius);
        Explode(position, Vector2.down, explosionRadius);
        Explode(position, Vector2.left, explosionRadius);
        Explode(position, Vector2.right, explosionRadius);
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

    public void SetExplosionRadius(int r) {
        this.explosionRadius = r;
    }

    public int GetPlayerNum() {
        return this.playerNum;
    }

    public void SetPlayerNum(int n) {
        this.playerNum = n;
    }
}
