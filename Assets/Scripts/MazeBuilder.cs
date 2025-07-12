using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

namespace TMOT
{
    public class MazeBuilder : MonoBehaviour
    {
        [System.Serializable]

        public class WallShape
        {
            [SerializeField]
            public List<Vector2> tiles;

            [SerializeField]
            public int weight = 1;


        }

        /// <summary>
        /// -: not used
        /// f: floor
        /// w: wall
        /// n: not walkable (used to flag tiles that we already know being not walkable)
        /// </summary>
        char[,] grid;

        int width = 20, height = 20;


        int floorWallRatio = 1;

        int floorCount = 0;
        int wallCount = 0;



        [SerializeField]
        List<WallShape> wallShapes = new List<WallShape>();

        /// <summary>
        /// 0: north
        /// 1: east
        /// 2: south
        /// 3: west
        /// </summary>
        int preferedDirection = 0;

        List<int[,]> edgeTiles = new List<int[,]>();

        [Header("Debug")]
        [SerializeField]
        GameObject floorTilePrefab;
        [SerializeField]
        GameObject wallTilePrefab;

        [SerializeField]
        GameObject notWalkablePrefab;

        List<GameObject> debugTiles;


        // Start is called before the first frame update
        void Start()
        {
            Build();
        }



        public void Build()
        {
            Init();
            Create();

            DebugTiles();
        }

        void Init()
        {
            grid = new char[width, height];
            for (int x = 0; x < width; x++)
                for (int z = 0; z < height; z++)
                    grid[x, z] = '-';
            floorCount = 0;
            wallCount = 0;
            preferedDirection = 0;
            edgeTiles.Clear();
        }

        void Create()
        {
            int iteration = 0;
            int tot = wallCount + floorCount;
            while (tot < width * height && iteration < 1000)
            {
                AddNextWallShape();    
                
                iteration++;
            }
        }

        void AddNextWallShape()
        {
            Debug.Log("*************DebugGridBefore****************");
            DebugGrid();

            // Get available shapes
            List<WallShape> candidates = new List<WallShape>();
            foreach (var ws in wallShapes)
                candidates.Add(ws);
            // Choose the next shape to add
            WallShape shape = candidates[UnityEngine.Random.Range(0, candidates.Count)];


            if (edgeTiles.Count == 0)
            {
                // If no edge tiles start from center
                Vector2 center = new Vector2(width / 2, height / 2);

                // Add wall shape
                AddWallShape((int)center.x, (int)center.y, shape);
            }
            else
            {

            }


            preferedDirection = (preferedDirection + 1) % 4;
            
            Debug.Log("*************DebugGridAfter****************");
            DebugGrid();

        }

        void AddWallShape(int x, int y, WallShape wallShape)
        {

            // Set tiles
            foreach (Vector2 tile in wallShape.tiles)
            {
                grid[x + (int)tile.x, y + (int)tile.y] = 'w';

            }

            // Set walkable tiles all around the shape
            foreach (Vector2 tile in wallShape.tiles)
            {
                int _x = x + (int)tile.x;
                int _y = y + (int)tile.y;
                // North
                bool north = false;
                if (grid[_x, _y + 1] == '-')
                {
                    north = true;
                    grid[_x, _y + 1] = 'f';
                    grid[_x, _y + 2] = 'f';
                }

                // East
                bool east = false;
                if (grid[_x + 1, _y] == '-')
                {
                    east = true;
                    grid[_x + 1, _y] = 'f';
                    grid[_x + 2, _y] = 'f';
                    if (north)
                    {
                        grid[_x + 1, _y + 1] = 'f';
                        grid[_x + 2, _y + 1] = 'f';
                        grid[_x + 1, _y + 2] = 'f';
                        grid[_x + 2, _y + 2] = 'f';
                    }
                }

                // South 
                bool south = false;
                if (grid[_x, _y - 1] == '-')
                {
                    south = true;
                    grid[_x, _y - 1] = 'f';
                    grid[_x, _y - 2] = 'f';
                    if (east)
                    {
                        grid[_x + 1, _y - 1] = 'f';
                        grid[_x + 1, _y - 2] = 'f';
                        grid[_x + 2, _y - 1] = 'f';
                        grid[_x + 2, _y - 2] = 'f';
                    }
                }

                // West
                if (grid[_x - 1, _y] == '-')
                {
                    grid[_x - 1, _y] = 'f';
                    grid[_x - 2, _y] = 'f';
                    if (south)
                    {
                        grid[_x - 1, _y - 1] = 'f';
                        grid[_x - 2, _y - 1] = 'f';
                        grid[_x - 1, _y - 2] = 'f';
                        grid[_x - 2, _y - 2] = 'f';
                    }
                    if (north)
                    {
                        grid[_x - 1, _y + 1] = 'f';
                        grid[_x - 2, _y + 1] = 'f';
                        grid[_x - 1, _y + 2] = 'f';
                        grid[_x - 2, _y + 2] = 'f';
                    }
                }
            }

            // Set not walkable tiles
            foreach (Vector2 tile in wallShape.tiles)
            {
                int _x = x + (int)tile.x;
                int _y = y + (int)tile.y;

                // North
                if (grid[_x, _y + 3] == '-')
                    grid[_x, _y + 3] = 'n';

                // East
                if (grid[_x + 3, _y] == '-')
                    grid[_x + 3, _y] = 'n';

                // South
                if (grid[_x, _y - 3] == '-')
                    grid[_x, _y - 3] = 'n';

                // West
                if (grid[_x - 3, _y] == '-')
                    grid[_x - 3, _y] = 'n';

            }
            
        }

        void DebugGrid()
        {
            string s = "\n";
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    s += $" {grid[x, y]}";
                }
                s += "\n";
            }
            Debug.Log(s);
        }

        void DebugTiles()
        {
            debugTiles = new List<GameObject>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 pos = new Vector3(x, 0, y);
                    char c = grid[x, y];
                    GameObject go = null;
                    if (c == 'f')
                        go = Instantiate(floorTilePrefab, pos, Quaternion.identity);
                    else if (c == 'w')
                        go = Instantiate(wallTilePrefab, pos, Quaternion.identity);
                    else if(c == 'n')
                        go = Instantiate(notWalkablePrefab, pos, Quaternion.identity);

                    if(go)
                        debugTiles.Add(go);
                }

            }
        }
    }
}