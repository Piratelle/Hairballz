using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
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
    //private int xMax;
    private int yMin;
    //private int yMax;

    private int xGridMin;
    //private int xGridMax;
    private int yGridMin;
    //private int yGridMax;

    private bool isClockwise;
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
        int xMax = mapWidth - wOff;
        yMin = 0 - hOff;
        int yMax = mapHeight - hOff;

        // store grid bounds
        int marg = (mapWidth - 2 - gridWidth) / 2;
        int xGridMax = xMax - marg - 1;
        xGridMin = xGridMax - gridWidth;
        marg = (mapWidth - 2 - gridWidth) / 2;
        int yGridMax = yMax - marg - 1;
        yGridMin = yGridMax - gridHeight;

        // initialize
        //PopulateLevel(0); // remove after Rpcs are set up!
    }

    // rebuild the grid
    public void PopulateLevel(int seed)
    {
        // clear any existing tiles
        bgTilemap.ClearAllTiles();
        inTilemap.ClearAllTiles();
        deTilemap.ClearAllTiles();

        // build Rect objects for easier boundary checks
        Rect mazeRect = new Rect(xMin, yMin, mapWidth, mapHeight);
        Rect mainRect = new Rect(xMin + 1, yMin + 1, mapWidth - 2, mapHeight - 2);
        Rect gridRect = new Rect(xGridMin, yGridMin, gridWidth, gridHeight);

        List<Rect> baseRects = new List<Rect> ();
        for (int p = 0; p < 4; p++)
        {
            float left = (p % 2 == 0) ? mainRect.xMin : mainRect.xMax - baseSide;
            float bottom = p < 2 ? mainRect.yMax - baseSide : mainRect.yMin;
            baseRects.Add(new Rect(left, bottom, baseSide, baseSide));
            //Debug.Log("Player " + p + " base from (" + left + "," + bottom +") to (" + (left + baseSide) + "," + (bottom + baseSide) + ")");
        }
        
        // build background
        for (int x = xMin; x < mazeRect.xMax; x++)
        {
            for (int y = yMin; y < mazeRect.yMax; y++)
            {
                bgTilemap.SetTile(new Vector3Int(x, y, 0), path);
            }
        }

        // build outer border
        int xMax = (int)mazeRect.xMax;
        int yMax = (int)mazeRect.yMax;
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

        // build player bases
        // 0 = top left; 1 = top right; 2 = bottom left; 3 = bottom right
        for (int p = 0; p < baseRects.Count; p++)
        {
            // learn player-specific traits
            Rect baseRect = baseRects[p];
            Tile bgTile = baseTiles[p % baseTiles.Length];

            for (int x = (int) baseRect.xMin; x < baseRect.xMax; x++)
            {
                for (int y = (int) baseRect.yMin; y < baseRect.yMax; y++)
                {
                    bgTilemap.SetTile(new Vector3Int(x, y, 0), bgTile);
                }
            }
        }

        // build grid
        int xGridMax = (int) gridRect.xMax;
        int yGridMax = (int) gridRect.yMax;
        List<(int, int)> eligibles = new List<(int, int)>();
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
                    eligibles.Add((x, y));
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
                (int, int) square = eligibles[r];
                eligibles.RemoveAt(r);
                deTilemap.SetTile(new Vector3Int(square.Item1, square.Item2, 0), destructWall);
            }
        }

        // now handle the margins, with base-to-base and base-to-grid pathing
        Rect pathRect = new Rect(mainRect.xMin + 1, mainRect.yMin + 1, mainRect.width - 2, mainRect.height - 2);
        eligibles.Clear();
        int xPathMin = (int) pathRect.xMin;
        int yPathMin = (int) pathRect.yMin;
        for (int x = xPathMin; x < pathRect.xMax; x++)
        {
            for (int y = yPathMin; y < pathRect.yMax; y++)
            {
                // check if this square is already in use
                Vector2 square = new Vector2(x, y);
                bool inUse = false;
                foreach (Rect baseRect in baseRects)
                {
                    if (baseRect.Contains(square))
                    {
                        inUse = true;
                        continue;
                    }
                }
                if (inUse || gridRect.Contains(square)) continue;

                eligibles.Add((x, y));
            }
        }

        foreach ((int x, int y) in eligibles)
        {
            if (true)
            {
                inTilemap.SetTile(new Vector3Int(x, y, 0), solidWall);
            }
        }

        // switch up the configuration for next time
        isClockwise = !isClockwise;
    }
    #endregion
}
