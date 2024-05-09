// Level.cs - from Erin
// Handles dynamic map generation

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
    //private int xMax;
    private int yMin;
    //private int yMax;

    private int xGridMin;
    //private int xGridMax;
    private int yGridMin;
    //private int yGridMax;
    #endregion

    #region Player Base
    private struct BaseInfo
    {
        public readonly Rect rect;
        public readonly Vector2Int spawnPos;
        public readonly Vector2Int innerPos;

        public BaseInfo(float xMin, float yMin, int side, int playerNum)
        {
            rect = new Rect(xMin, yMin, side, side);

            float left = rect.xMin;
            float right = rect.xMax - 1;
            float top = rect.yMax - 1;
            float bottom = rect.yMin;

            spawnPos = new Vector2Int((int) ((playerNum % 2 == 0) ? left : right), (int) (playerNum < 2 ? top : bottom));
            innerPos = new Vector2Int((int) ((playerNum % 2 == 0) ? right : left), (int) (playerNum < 2 ? bottom : top));
        }

        public int XMin() { return (int) rect.xMin; }
        public int XMax() { return (int) rect.xMax; }
        public int YMin() { return (int) rect.yMin; }
        public int YMax() { return (int) rect.yMax; }
    }
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
        //PopulateLevel(0); // remove for networking! add in for testing!
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

        List<BaseInfo> bases = new List<BaseInfo> ();
        for (int p = 0; p < 4; p++)
        {
            float left = (p % 2 == 0) ? mainRect.xMin : mainRect.xMax - baseSide;
            float bottom = p < 2 ? mainRect.yMax - baseSide : mainRect.yMin;
            bases.Add(new BaseInfo(left, bottom, baseSide, p));
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
        //Debug.Log("Building bases...");
        for (int p = 0; p < bases.Count; p++)
        {
            // learn player-specific traits
            BaseInfo b = bases[p];
            Tile bgTile = baseTiles[p % baseTiles.Length];

            for (int x = b.XMin(); x < b.XMax(); x++)
            {
                for (int y = b.YMin(); y < b.YMax(); y++)
                {
                    bgTilemap.SetTile(new Vector3Int(x, y, 0), bgTile);
                }
            }
        }

        // build grid
        //Debug.Log("Populating grid...");
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
        //Debug.Log("Checking for eligible path squares...");
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
                foreach (BaseInfo b in bases)
                {
                    if (b.rect.Contains(square))
                    {
                        inUse = true;
                        continue;
                    }
                }
                if (inUse || gridRect.Contains(square)) continue;

                eligibles.Add((x, y));
            }
        }

        // modified Wilson's algorithm for pathing from each base to the grid
        //Debug.Log("Starting Wilson's run...");
        Vector2Int[] dirs = {
            new Vector2Int(1,0) // right
            , new Vector2Int(0, -1) // down
            , new Vector2Int(0, 1) // up
            , new Vector2Int(-1, 0) // left
        };
        for (int p = 0; p < bases.Count; p++)
        {
            //Debug.Log("Pathing from Base " + p);
            BaseInfo b = bases[p];

            // build path from base to grid
            Dictionary<(int, int), Vector2Int> path = new Dictionary<(int, int), Vector2Int>();
            Vector2Int current = b.innerPos;
            Vector2Int dir = dirs[p];
            Vector2Int next = current + dir;
            path[(current.x, current.y)] = dir;
            while (!gridRect.Contains(next))
            {
                //Debug.Log("Current square: " + current + ", next dir: " + dir);
                current = next;
                int i = RND.Next(dirs.Length);
                dir = dirs[i];
                next = current + dir;
                while (!eligibles.Contains((next.x, next.y)) && !gridRect.Contains(next))
                {
                    i = RND.Next(dirs.Length);
                    dir = dirs[i];
                    next = current + dir;
                }
                path[(current.x, current.y)] = dir;
            }

            // now remove the path squares from eiligible wall squares
            current = b.innerPos;
            current += path[(current.x, current.y)];
            while (!gridRect.Contains(current))
            {
                eligibles.Remove((current.x, current.y));
                current += path[(current.x, current.y)];
            }
        }

        // fill the remaining grid squares with walls
        foreach ((int x, int y) in eligibles)
        {
            inTilemap.SetTile(new Vector3Int(x, y, 0), solidWall);
        }
    }
    #endregion
}
