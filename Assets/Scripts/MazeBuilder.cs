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
            [SerializeField]
            public List<Tile> tiles;

            [SerializeField]
            public GameObject helper;

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

        int width = 1000, height = 1000;

        int maxTiles = 20 * 20;


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

            Debug.Log($"Buili in {(DateTime.Now - startTime).TotalSeconds} seconds");

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
                shape.tiles.Clear();
                for (int i = 0; i < shape.helper.transform.childCount; i++)
                {
                    var child = shape.helper.transform.GetChild(i);
                    Tile t = new Tile();
                    t.type = child.gameObject.name.Substring(0,1).ToLower()[0];
                    t.coords = new Vector2(child.position.x * tileScale, child.position.z * tileScale);
                    shape.tiles.Add(t);
                }
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
                candidates.Add(ws);
            // Choose the next shape to add
            WallShape shape = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            shape = candidates[0];

            candidates.RemoveAll(s => s == shape);


            if (wallCount == 0)
            {
                // If no edge tiles start from center
                Vector2 center = new Vector2(width / 2, height / 2);

                // Add wall shape

                if (!TryAddWallShape((int)center.x, (int)center.y, shape))
                {
                    Debug.LogError("Can't even add the first wall :(");
                    return;
                }
            }
            else
            {
                // Get all the edge tile in the preferred direction
                var edges = GetOrderedEdgeTiles();

                var edge = edges[UnityEngine.Random.Range(0, edges.Count)];
                edges.Remove(edge);
                bool ok = TryAddWallShape(edge.Item1, edge.Item2, shape);

                int iterations = 0;
                while (!ok && iterations < 1000)
                {
                    // Get another edge
                    if (edges.Count > 0)
                    {
                        edge = edges[UnityEngine.Random.Range(0, edges.Count)];//edges.First();
                        edges.Remove(edge);

                        ok = TryAddWallShape(edge.Item1, edge.Item2, shape);
                    }
                    else // No more edge, lets try another shape
                    {
                        // Get a new shape
                        shape = candidates[UnityEngine.Random.Range(0, candidates.Count)];
                        // Refill edge list
                        edges = GetOrderedEdgeTiles();
                    }
                    
                    iterations++;
                }


            }


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
                                ret.Add((i, j));
                            break;
                        case 1: // E
                            if (i - 1 >= 0 && grid[i - 1, j] == 'f')
                                ret.Add((i, j));
                            break;
                        case 2: // S
                            if (j + 1 < width && grid[i, j + 1] == 'f')
                                ret.Add((i, j));
                            break;
                        case 3: // W
                            if (i + 1 < width && grid[i + 1, j] == 'f')
                                ret.Add((i, j));
                            break;
                    }
                }

            return ret;

            
            // List<(int, int)> edgeTiles = new List<(int, int)>();
            // for (int i = 0; i < width; i++)
            //     for (int j = 0; j < height; j++)
            //         edgeTiles.Add((i, j));
                    
            // switch (preferredDirection)
            // {
            //     case 0: // N
            //         return edgeTiles.FindAll(t => (grid[t.Item1, t.Item2] == '-' || grid[t.Item1, t.Item2] == 'n') && t.Item2 - 1 >= 0 && grid[t.Item1, t.Item2 - 1] == 'f').OrderBy(t => t.Item2).ToList();
            //     case 1: // E
            //         return edgeTiles.FindAll(t => (grid[t.Item1, t.Item2] == '-' || grid[t.Item1, t.Item2] == 'n') && t.Item1 - 1 >= 0 && grid[t.Item1 - 1, t.Item2] == 'f').OrderBy(t => t.Item1).ToList();
            //     case 2: // S
            //         return edgeTiles.FindAll(t => (grid[t.Item1, t.Item2] == '-' || grid[t.Item1, t.Item2] == 'n') && t.Item2 + 1 < width && grid[t.Item1, t.Item2 + 1] == 'f').OrderByDescending(t => t.Item2).ToList();
            //     case 3: // W
            //         return edgeTiles.FindAll(t => (grid[t.Item1, t.Item2] == '-' || grid[t.Item1, t.Item2] == 'n') && t.Item1 + 1 < width && grid[t.Item1 + 1, t.Item2] == 'f').OrderByDescending(t => t.Item1).ToList();
            // }

            // return null;
        }

        bool TryAddWallShape(int x, int y, WallShape wallShape)
        {
            List<(Vector2, char)> changed = new List<(Vector2, char)>();
            
            bool rollback = false;

            foreach (var tile in wallShape.tiles)
            {
                int _x = x + (int)tile.coords.x;
                int _y = y + (int)tile.coords.y;

                char type = tile.type;

                if (type == 'w')
                {
                    if (grid[_x, _y] == 'w' || grid[_x, _y] == '-' || grid[_x, _y] == 'n')
                    {
                        // Store old value for rollback
                        changed.Add((new Vector2(_x, _y), grid[_x, _y]));
                        // Update tile
                        grid[_x, _y] = type;
                    }
                    else rollback = true;
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
                    else rollback = true;

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
                    else if (grid[_x, _y] == 'f') rollback = true;
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

/*
        bool _TryAddWallShape(int x, int y, WallShape wallShape)
        {
            char[,] tmpGrid = new char[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    tmpGrid[i, j] = grid[i, j];
                    //grid.CopyTo(tmpGrid, 0);

                    List<(int, int)> tmpEdge = new List<(int,int)>();

            int tmpFloorCount = 0;
            int tmpWallCount = 0;

            // Set tiles
            foreach (Vector2 tile in wallShape.tiles)
            {
                tmpGrid[x + (int)tile.x, y + (int)tile.y] = 'w';

                tmpWallCount++; 
            }

            // Set walkable tiles all around the shape
            foreach (Vector2 tile in wallShape.tiles)
            {
                int _x = x + (int)tile.x;
                int _y = y + (int)tile.y;
                // North
                bool north = false;
                if (tmpGrid[_x, _y + 1] == '-')
                {
                    north = true;
                    tmpGrid[_x, _y + 1] = 'f';
                    tmpFloorCount++;
                    if (tmpGrid[_x, _y + 2] == '-')
                    {
                        tmpGrid[_x, _y + 2] = 'f';
                        tmpFloorCount++;
                    }
                    else if (tmpGrid[_x, _y + 2] != 'f')
                        return false;

                    tmpEdge.Add((_x, _y + 2));
                    
                }
                else if (tmpGrid[_x, _y + 1] != 'f')
                    return false;
                

                // East
                bool east = false;
                if (tmpGrid[_x + 1, _y] == '-')
                {
                    east = true;
                    tmpGrid[_x + 1, _y] = 'f';
                    tmpFloorCount++;
                    if (tmpGrid[_x + 2, _y] == '-')
                    {
                        tmpGrid[_x + 2, _y] = 'f';
                        tmpFloorCount++;
                    }
                    else if (tmpGrid[_x + 2, _y] != 'f')
                        return false;
                    
                    tmpEdge.Add((_x + 2, _y));
                    
                    if (north)
                    {
                        if (tmpGrid[_x + 1, _y + 1] == '-')
                        {
                            tmpGrid[_x + 1, _y + 1] = 'f';
                            tmpFloorCount++;
                        }
                        else if (tmpGrid[_x + 1, _y + 1] != 'f')
                            return false;

                        if (tmpGrid[_x + 2, _y + 1] == '-')
                        {
                            tmpGrid[_x + 2, _y + 1] = 'f';
                            tmpFloorCount++;
                        }
                        else if (tmpGrid[_x + 2, _y + 1] != 'f')
                            return false;

                        if (tmpGrid[_x + 1, _y + 2] == '-')
                        {
                            tmpGrid[_x + 1, _y + 2] = 'f';
                            tmpFloorCount++;
                        }
                        else if (tmpGrid[_x + 1, _y + 2] != 'f')
                            return false;

                        if (tmpGrid[_x + 2, _y + 2] == '-')
                        {
                            tmpGrid[_x + 2, _y + 2] = 'f';
                            tmpFloorCount++;
                        }
                        else if (tmpGrid[_x + 2, _y + 2] != 'f')
                            return false;
                        
                        tmpEdge.Add((_x + 2, _y + 1));
                        tmpEdge.Add((_x + 1, _y + 2));
                        tmpEdge.Add((_x + 2, _y + 2));
                    }
                }
                else if (tmpGrid[_x + 1, _y] != 'f')
                {
                    return false;
                }

                // South 
                bool south = false;
                if (tmpGrid[_x, _y - 1] == '-')
                {
                    south = true;
                    tmpGrid[_x, _y - 1] = 'f';
                    tmpFloorCount++;
                    if (tmpGrid[_x, _y - 2] == '-')
                    {
                        tmpGrid[_x, _y - 2] = 'f';
                        tmpFloorCount++;
                    }
                    else if (tmpGrid[_x, _y - 2] != 'f')
                        return false;
                    
                    tmpEdge.Add((_x, _y - 2));
                    
                    if (east)
                    {
                        if (tmpGrid[_x + 1, _y - 1] == '-')
                        {
                            tmpGrid[_x + 1, _y - 1] = 'f';
                            tmpFloorCount++;
                        }
                        else if (tmpGrid[_x + 1, _y - 1] != 'f')
                            return false;
                        
                        if (tmpGrid[_x + 1, _y - 2] == '-')
                        {
                            tmpGrid[_x + 1, _y - 2] = 'f';
                            tmpFloorCount++;
                        }
                        else if (tmpGrid[_x + 1, _y - 1] != 'f')
                            return false;

                        if (tmpGrid[_x + 2, _y - 1] == '-')
                        {
                            tmpGrid[_x + 2, _y - 1] = 'f';
                            tmpFloorCount++;
                        }
                        else if (tmpGrid[_x + 1, _y - 1] != 'f')
                            return false;

                        if (tmpGrid[_x + 2, _y - 2] == '-')
                        {
                            tmpGrid[_x + 2, _y - 2] = 'f';
                            tmpFloorCount++;
                        }
                        else if (tmpGrid[_x + 1, _y - 1] != 'f')
                            return false;

                        tmpEdge.Add((_x + 1, _y - 2));
                        tmpEdge.Add((_x + 2, _y - 1));
                        tmpEdge.Add((_x + 2, _y - 2));
                    }
                }
                else if (tmpGrid[_x, _y - 1] != 'f')
                {
                    return false;
                }

                // West
                if (tmpGrid[_x - 1, _y] == '-')
                {
                    tmpGrid[_x - 1, _y] = 'f';
                    tmpFloorCount++;
                    if (tmpGrid[_x - 2, _y] == '-')
                    {
                        tmpGrid[_x - 2, _y] = 'f';
                        tmpFloorCount++;
                    }
                    else if (tmpGrid[_x - 2, _y] != 'f')
                        return false;
                    
                    tmpEdge.Add((_x - 2, _y));
                    if (south)
                    {

                        if (tmpGrid[_x - 1, _y - 1] == '-')
                        {
                            tmpGrid[_x - 1, _y - 1] = 'f';
                            tmpFloorCount++;
                        }
                        else if (tmpGrid[_x - 1, _y - 1] != 'f')
                            return false;

                        if (tmpGrid[_x - 2, _y - 1] == '-')
                        {
                            tmpGrid[_x - 2, _y - 1] = 'f';
                            tmpFloorCount++;
                        }
                        else if (tmpGrid[_x - 2, _y - 1] != 'f')
                        return false;

                        if (tmpGrid[_x - 1, _y - 2] == '-')
                        {
                            tmpGrid[_x - 1, _y - 2] = 'f';
                            tmpFloorCount++;
                        }
                        else if (tmpGrid[_x - 1, _y - 2] != 'f')
                            return false;

                        if (tmpGrid[_x - 2, _y - 2] == '-')
                        {
                            tmpGrid[_x - 2, _y - 2] = 'f';
                            tmpFloorCount++;
                        }
                        else if (tmpGrid[_x - 2, _y - 2] != 'f')
                            return false;
                        
                        tmpEdge.Add((_x - 2, _y - 1));
                        tmpEdge.Add((_x - 1, _y - 2));
                        tmpEdge.Add((_x - 2, _y - 2));
                    }
                    if (north)
                    {
                        if (tmpGrid[_x - 1, _y + 1] == '-')
                        {
                            tmpGrid[_x - 1, _y + 1] = 'f';    
                            tmpFloorCount++;
                        }
                        else if (tmpGrid[_x - 1, _y + 1] != 'f')
                            return false;

                        if (tmpGrid[_x - 2, _y + 1] == '-')
                        {
                            tmpGrid[_x - 2, _y + 1] = 'f';
                            tmpFloorCount++;
                        }
                        else if (tmpGrid[_x - 2, _y + 1] != 'f')
                            return false;

                        if (tmpGrid[_x - 1, _y + 2] == '-')
                        {
                            tmpGrid[_x - 1, _y + 2] = 'f';    
                            tmpFloorCount++;
                        }
                        else if (tmpGrid[_x - 1, _y + 2] != 'f')
                            return false;

                        if (tmpGrid[_x - 2, _y + 2] == '-')
                        {
                            tmpGrid[_x - 2, _y + 2] = 'f';
                            tmpFloorCount++;
                        }
                        else if (tmpGrid[_x - 2, _y + 2] != 'f')
                            return false;

                        tmpEdge.Add((_x - 2, _y + 1));
                        tmpEdge.Add((_x - 1, _y + 2));
                        tmpEdge.Add((_x - 2, _y + 2));
                    }
                }
                else if (tmpGrid[_x - 1, _y] != 'f')
                {
                    return false;
                }
            }

            // Set not walkable tiles
            foreach (Vector2 tile in wallShape.tiles)
            {
                int _x = x + (int)tile.x;
                int _y = y + (int)tile.y;

                // North
                if (tmpGrid[_x, _y + 3] == '-')
                    tmpGrid[_x, _y + 3] = 'n';
                else if (tmpGrid[_x, _y + 3] == 'f')
                {
                    return false;
                }

                // East
                if (tmpGrid[_x + 3, _y] == '-')
                    tmpGrid[_x + 3, _y] = 'n';
                else if (tmpGrid[_x + 3, _y] == 'f')
                {
                    return false;
                }

                // South
                if (tmpGrid[_x, _y - 3] == '-')
                    tmpGrid[_x, _y - 3] = 'n';
                else if (tmpGrid[_x, _y - 3] == 'f')
                {
                    return false;
                }

                // West
                if (tmpGrid[_x - 3, _y] == '-')
                    tmpGrid[_x - 3, _y] = 'n';
                else if (tmpGrid[_x - 3, _y] == 'f')
                {
                    return false;
                }

            }

            // Update the edge tiles
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    grid[i, j] = tmpGrid[i, j];
            //tmpGrid.CopyTo(grid, 0);
           
            floorCount += tmpFloorCount;
            wallCount += tmpWallCount;
            return true;
            
        }
        */

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