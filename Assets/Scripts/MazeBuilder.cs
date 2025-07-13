using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.U2D;

namespace TMOT
{
    public class MazeBuilder : MonoBehaviour
    {
        [System.Serializable]

        public class Tile
        {
            [SerializeField]
            public Vector2 coords;

            [SerializeField]
            public char type;
        }

        [System.Serializable]
        public class WallShape
        {
            /// <summary>
            /// Each index represent a rotation variant
            /// </summary>
            [SerializeField]
            public List<Tile>[] tiles = new List<Tile>[4]; 

            [SerializeField]
            public GameObject helper;

            [SerializeField]
            public int weight = 1;

            [SerializeField]
            public int max = -1;

            [SerializeField]
            public bool rotate90;

            [SerializeField]
            public bool rotate180;

            [SerializeField]
            public bool rotate270;

        }

        /// <summary>
        /// -: not used
        /// f: floor
        /// w: wall
        /// n: not walkable (used to flag tiles that we already know being not walkable)
        /// </summary>
        char[,] grid;

        int width = 100, height = 100;

        int maxTiles = 40 * 40;


        int floorWallRatio = 1;

        int floorCount = 0;
        int wallCount = 0;

        float tileScale = 1;



        [SerializeField]
        List<WallShape> wallShapes = new List<WallShape>();

        /// <summary>
        /// 0: north
        /// 1: east
        /// 2: south
        /// 3: west
        /// </summary>
        int preferredDirection = 0;

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
            DateTime startTime = DateTime.Now;
            
            Init();

            try
            {
                Create();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            Debug.Log($"Built in {(DateTime.Now - startTime).TotalSeconds} seconds");

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
            preferredDirection = 0;

            FillShapeList();
        }

        void FillShapeList()
        {
            foreach (var shape in wallShapes)
            {
                var helper = Instantiate(shape.helper);
                // Clear all variants
                for (int v = 0; v < 4; v++)
                {
                    shape.tiles[v] = new List<Tile>();

                    bool ok = false;

                    if (v == 0)
                    {
                        ok = true;
                    }
                    else
                    {
                        if (v == 1 && shape.rotate90)
                        {
                            helper.transform.GetChild(0).localEulerAngles = Vector3.up * 90f;
                            ok = true;
                        }
                        else if (v == 2 && shape.rotate180)
                        {
                            helper.transform.GetChild(0).localEulerAngles = Vector3.up * 180f;
                            ok = true;
                        }
                        else if (v == 3 && shape.rotate270)
                        {
                            helper.transform.GetChild(0).localEulerAngles = Vector3.up * 270f;
                            ok = true;
                        }
                    }


                    if (ok)
                    {
                        for (int i = 0; i < helper.transform.GetChild(0).childCount; i++)
                        {
                            var child = helper.transform.GetChild(0).GetChild(i);
                            Tile t = new Tile();
                            t.type = child.gameObject.name.Substring(0, 1).ToLower()[0];
                            t.coords = new Vector2(Mathf.Round(child.position.x) * tileScale, Mathf.Round(child.position.z) * tileScale);
                            shape.tiles[v].Add(t);


                        }

                        
                    }
                }



                Destroy(helper);
            }
            

        }


        void Create()
        {
            int iteration = 0;
            int tot = wallCount + floorCount;
            while (tot < maxTiles && iteration < 50)
            {
                AddNextWallShape();

                wallCount = CountWalls();
                floorCount = CountFloors();

                iteration++;
            }
        }

        int CountWalls()
        {
            int count = 0;
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    if (grid[i, j] == 'w') count++;

            return count;
        }

        int CountFloors()
        {
            int count = 0;
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    if (grid[i, j] == 'f') count++;

            return count;
        }

        void AddNextWallShape()
        {
            Debug.Log("*************DebugGridBefore****************");
            DebugGrid();

            // Get available shapes
            List<WallShape> candidates = new List<WallShape>();
            foreach (var ws in wallShapes)
            {
                var l = new List<WallShape>();
                for (int i = 0; i < ws.weight; i++)
                    l.Add(ws);
                candidates.AddRange(l);
            }
                
            // Choose the next shape to add
            WallShape shape = candidates[UnityEngine.Random.Range(0, candidates.Count)];
           

            int x, y;

            // Get all the edge tile in the preferred direction
            List<(int, int)> edges = null;
            int iterations = 0;
            bool ok = false;
            do
            {
                if (wallCount == 0)
                {
                    x = (int)width / 2;
                    y = (int)height / 2;

                }
                else
                {
                    
                    if (edges == null || edges.Count == 0)
                    {
                        if (edges != null) 
                        {
                            // No more edge, lets try another shape
                            shape = candidates[UnityEngine.Random.Range(0, candidates.Count)];    
                        }
                        
                        // Refill edge list
                        edges = GetOrderedEdgeTiles();
                    }

                    var edge = edges[0];// edges[UnityEngine.Random.Range(0, edges.Count)];
                    edges.Remove(edge);

                    x = edge.Item1;
                    y = edge.Item2;
                }


                ok = TryAddWallShape(x, y, shape);

                if (ok)
                {
                    if (shape.max > 0)
                    {
                        shape.max--;
                        if (shape.max <= 0)
                            shape.weight = 0;
                        candidates.RemoveAll(s => s == shape);    
                        
                    }
                        
                }
            }
            while (!ok && iterations < 1000);

            preferredDirection = (preferredDirection + 1) % 4;
            
            Debug.Log("*************DebugGridAfter****************");
            DebugGrid();

        }

        List<(int, int)> GetOrderedEdgeTiles()
        {
            List<(int, int)> ret = new List<(int, int)>();
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    char type = grid[i, j];
                    if (type != '-' && type != 'n') continue;
                    switch (preferredDirection)
                    {
                        case 0: // N
                            if (j - 1 >= 0 && grid[i, j - 1] == 'f')
                            {
                                if(!ret.Contains((i,j))) ret.Add((i, j));
                                if (grid[i - 1, j] == '-' && !ret.Contains((i - 1, j))) ret.Add((i - 1, j));
                                if (grid[i + 1, j] == '-' && !ret.Contains((i + 1, j))) ret.Add((i + 1, j));
                            }
                            
                            break;
                        case 1: // E
                            if (i - 1 >= 0 && grid[i - 1, j] == 'f')
                            {
                                if(!ret.Contains((i,j))) ret.Add((i, j));
                                if (grid[i, j + 1] == '-' && !ret.Contains((i, j + 1))) ret.Add((i, j + 1));
                                if (grid[i, j - 1] == '-' && !ret.Contains((i, j - 1))) ret.Add((i, j - 1));
                            }
                                
                            break;
                        case 2: // S
                            if (j + 1 < width && grid[i, j + 1] == 'f')
                            {
                                if(!ret.Contains((i,j))) ret.Add((i, j));
                                if (grid[i + 1, j] == '-' && !ret.Contains((i + 1, j))) ret.Add((i + 1, j));
                                if (grid[i - 1, j] == '-' && !ret.Contains((i - 1, j))) ret.Add((i - 1, j));
                            }
                                
                            break;
                        case 3: // W
                            if (i + 1 < width && grid[i + 1, j] == 'f')
                            {
                                if(!ret.Contains((i,j))) ret.Add((i, j));
                                if (grid[i, j + 1] == '-' && !ret.Contains((i, j + 1))) ret.Add((i, j + 1));
                                if (grid[i, j - 1] == '-' && !ret.Contains((i, j - 1))) ret.Add((i, j - 1));
                            }
                                
                            break;
                    }
                }

            switch (preferredDirection)
            {
                case 0:
                    ret = ret.OrderBy(t => t.Item2).ToList();
                    break;
                case 1:
                    ret = ret.OrderBy(t => t.Item1).ToList();
                    break;
                case 2:
                    ret = ret.OrderByDescending(t => t.Item2).ToList();
                    break;
                case 3:
                    ret = ret.OrderByDescending(t => t.Item1).ToList();
                    break;
            }

            return ret;

            
         
        }

        bool TryAddWallShape(int x, int y, WallShape wallShape)
        {
            List<(Vector2, char)> changed = new List<(Vector2, char)>();
            
            bool rollback = false;

            List<int> variants = new List<int>();
            variants.Add(0);
            if (wallShape.rotate90) variants.Add(1);
            if (wallShape.rotate180) variants.Add(2);
            if (wallShape.rotate270) variants.Add(3);

            int variant = variants[UnityEngine.Random.Range(0, variants.Count)];
           
           
            int count = 0;
            foreach (var tile in wallShape.tiles[variant])
            {
                
                int _x = x + (int)tile.coords.x;
                int _y = y + (int)tile.coords.y;

                char type = tile.type;

                count++;

                if (type == 'w')
                {
                    if (grid[_x, _y] == 'w' || grid[_x, _y] == '-' || grid[_x, _y] == 'n')
                    {
                        // Store old value for rollback
                        changed.Add((new Vector2(_x, _y), grid[_x, _y]));
                        // Update tile
                        grid[_x, _y] = type;
                    }
                    else
                    {
                        rollback = true;
                        break;
                    }
                }
                else if (type == 'f')
                {
                    if (grid[_x, _y] == 'f' || grid[_x, _y] == '-')
                    {
                        // Store old value for rollback
                        changed.Add((new Vector2(_x, _y), grid[_x, _y]));
                        // Update tile
                        grid[_x, _y] = type;
                    }
                    else
                    {
                        rollback = true;
                        break;
                    }

                }
                else if (type == 'n')
                {
                    if (grid[_x, _y] == '-')
                    {
                        // Store old value for rollback
                        changed.Add((new Vector2(_x, _y), grid[_x, _y]));
                        // Update tile
                        grid[_x, _y] = type;
                    }
                    else if (grid[_x, _y] == 'f')
                    {
                        rollback = true;
                        break;
                    }
                }

            }

            if (rollback)
            {
                foreach (var c in changed)
                {
                    grid[(int)c.Item1.x, (int)c.Item1.y] = c.Item2;
                }

                return false;
            }

            Debug.Log($"Added wall tile to ({x},{y}), direction:{preferredDirection}");

            return true;
        }


        void DebugGrid()
        {
            return;
            string s = $"Grid - Used:{wallCount+floorCount}, floors:{floorCount}\n";
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

                    if (go)
                        debugTiles.Add(go);
                   
                        
                }

            }
        }
    }
}