using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{
    // Set tile pre-fabs in Unity editor
    [SerializeField] private Tile solidWall;
    [SerializeField] private Tile destructWall;
    [SerializeField] private Tile path;

    private Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>(); // careful if we switch to double tilemap option!
    }
}
