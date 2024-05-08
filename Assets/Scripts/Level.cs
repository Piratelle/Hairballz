using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{
    #region Variables
    // link tilemaps
    [Header("Tilemaps")]
    [SerializeField] private Tilemap bgTilemap;
    [SerializeField] private Tilemap inTilemap;
    [SerializeField] private Tilemap deTilemap;

    // set map variables
    [Header("Map")]
    [SerializeField] private int mapWidth; // cannot be smaller than 2 (baseSide + 1)
    [SerializeField] private int mapHeight; // cannot be smaller than 2 (baseSide + 1)
    [SerializeField] private int baseSide; // cannot be smaller than 1
    [SerializeField] private int gridWidth; // cannot exceed mapWidth - 2!
    [SerializeField] private int gridHeight; // cannot exceed mapHeight - 2!

    // Set tile pre-fabs in Unity editor
    [Header("Tiles")]
    [SerializeField] private Tile solidWall;
    [SerializeField] private Tile destructWall;
    [SerializeField] private Tile path;
    [SerializeField] private Tile[] baseTiles;

    [Header("Behavior")]
    [SerializeField] private float density = 0.6f;

    private int xMin;
    private int xMax;
    private int yMin;
    private int yMax;

    private int xGridMin;
    private int xGridMax;
    private int yGridMin;
    private int yGridMax;
    #endregion

    #region Initialization
    // Start is called before the first frame update
    void Start()
    {
        // enforce parameter restrictions
        density = Mathf.Clamp(density, 0, 1);

        baseSide = Mathf.Max(baseSide, 1);

        int minMap = 2 * (baseSide + 1);
        mapWidth = Mathf.Max(minMap, mapWidth);
        mapHeight = Mathf.Max(minMap, mapHeight);

        gridWidth = Mathf.Min(mapWidth - 2, gridWidth);
        gridHeight = Mathf.Min(mapHeight - 2, gridHeight);

        // store bounds
        int wOff = mapWidth / 2;
        int hOff = mapHeight / 2;
        xMin = 0 - wOff;
        xMax = mapWidth - wOff;
        yMin = 0 - hOff;
        yMax = mapHeight - hOff;

        // store grid bounds
        int marg = (mapWidth - 2 - gridWidth) / 2;
        xGridMax = xMax - marg - 1;
        xGridMin = xGridMax - gridWidth;
        marg = (mapWidth - 2 - gridWidth) / 2;
        yGridMax = yMax - marg - 1;
        yGridMin = yGridMax - gridHeight;

        // initialize
        PopulateLevel(0); // remove after Rpcs are set up!
    }

    // rebuild the grid
    void PopulateLevel(int seed)
    {
        // clear any existing tiles
        bgTilemap.ClearAllTiles();
        inTilemap.ClearAllTiles();
        deTilemap.ClearAllTiles();

        List<Tuple<int, int>> eligibles = new List<Tuple<int, int>>();

        // build background
        for (int x = xMin; x < xMax; x++)
        {
            for (int y = yMin; y < yMax; y++)
            {
                bgTilemap.SetTile(new Vector3Int(x, y, 0), path);
            }
        }

        // build the outer border
        for (int x = xMin; x < xMax; x++)
        {
            inTilemap.SetTile(new Vector3Int(x, yMin, 0), solidWall);
            inTilemap.SetTile(new Vector3Int(x, yMax - 1, 0), solidWall);
        }
        for (int y = yMin + 1; y < yMax - 1; y++)
        {
            inTilemap.SetTile(new Vector3Int(xMin, y, 0), solidWall);
            inTilemap.SetTile(new Vector3Int(xMax - 1, y, 0), solidWall);
        }

        // fill in margins
        for (int x = xMin + 1; x < xMax - 1; x++)
        {
            for (int y = yMin + 1; y < yGridMin; y++)
            {
                //tilemap.SetTile(new Vector3Int(x, y, 0), solidWall);
            }
            for (int y = yGridMax; y < yMax - 1; y++)
            {
                //tilemap.SetTile(new Vector3Int(x, y, 0), solidWall);
            }
        }
        for (int y = yGridMin; y < yGridMax; y++)
        {
            for (int x = xMin + 1; x < xGridMin; x++)
            {
                //tilemap.SetTile(new Vector3Int(x, y, 0), solidWall);
            }
            for (int x = xGridMax; x < xMax - 1; x++)
            {
                //tilemap.SetTile(new Vector3Int(x, y, 0), solidWall);
            }
        }

        // build player bases
        // 0 = top left; 1 = top right; 2 = bottom left; 3 = bottom right
        for (int p = 0; p < 4; p++)
        {
            ;
            // learn player-specific traits
            int left = (p % 2 == 0) ? xMin + 1 : xMax - (baseSide + 1);
            int bottom = p < 2 ? yMax - (baseSide + 1) : yMin + 1;
            Tile bgTile = baseTiles[p % baseTiles.Length];

            for (int x = left; x < left + baseSide; x++)
            {
                for (int y = bottom; y < bottom + baseSide; y++)
                {
                    bgTilemap.SetTile(new Vector3Int(x, y, 0), bgTile);
                }
            }
        }

        // build grid
        for (int x = xGridMin; x < xGridMax; x++)
        {
            for (int y = yGridMin; y < yGridMax; y++)
            {
                // standard method: offset
                if ((x - xGridMin) % 2 == 1 && (y - yGridMin) % 2 == 1 && x < xGridMax - 1 && y < yGridMax - 1)
                {
                    inTilemap.SetTile(new Vector3Int(x, y, 0), solidWall);
                } else
                {
                    eligibles.Add(new Tuple<int, int>(x, y));
                }
            }
        }

        // now populate grid with randomized destructibles
        System.Random RND = new System.Random(seed);
        float maxSquares = eligibles.Count;
        if (maxSquares > 0) {
            while ((maxSquares - eligibles.Count) / maxSquares < density)
            {
                int r = RND.Next(eligibles.Count);
                Tuple<int, int> square = eligibles[r];
                eligibles.RemoveAt(r);
                deTilemap.SetTile(new Vector3Int(square.Item1, square.Item2, 0), destructWall);
            }
        }

        // now handle the base-to-base and base-to-grid pathing

    }
    #endregion
}
