// Jonah Hedlund
// When explosion detected, destroys tile, drops bomb
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructableTilemap : MonoBehaviour
{
    public Tilemap destructableTiles;
    private void Start() {
        destructableTiles = GetComponent<Tilemap>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Explosion")) {
            Debug.Log("Wall Destructing");
            Vector3 hitPos = Vector3.zero;
            foreach(ContactPoint2D hit in collision.contacts) {
                hitPos.x = hit.point.x * hit.normal.x;
                hitPos.y = hit.point.y * hit.normal.y;
                destructableTiles.SetTile(destructableTiles.WorldToCell(hitPos), null);
            }
        }
    }
}
