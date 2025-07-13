using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

namespace TMOT
{
    public class LevelBuilder : MonoBehaviour
    {
        public delegate void LevelBuiltDelegate(int width, int height, int[,] grid);
        public static LevelBuiltDelegate OnLevelBuilt;

        public delegate void LevelBuilderFailedDelegate(int width, int heiht, int[,] grid);
        public static LevelBuilderFailedDelegate OnLevelBuilderFailed;

        int width = 20; // In tile size
        int height = 20;

        float freeBlockRatioRatio = 1; // It's Empty/Full ratio
        

        /// <summary>
        /// -1: not used yet
        /// 0: empty
        /// 1: full
        /// Starting from south-west
        /// </summary>
        int[,] grid; 

        int tileSize = 2;

        List<int[,]> blockShapes = new List<int[,]>();
        
        

        public int[,] Grid
        {
            get { return grid; }
        }



        void Start()
        {
           
        }

        public void Build()
        {
            Init();

            try
            {
                Fill();
                OnLevelBuilt?.Invoke(width, height, grid);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                OnLevelBuilderFailed?.Invoke(width, height, grid);
            
            }

            DebugGrid();
        }

        void Init()
        {
            // Store level size
            if (LevelController.Instance)
            {
                width = (int)LevelController.Instance.MapSize.x;    
                height = (int)LevelController.Instance.MapSize.y;
            }
            
            

            // Init grid
            grid = new int[width, height];
            for (int x = 0; x < width; x++)
                for(int z =0; z<height; z++)
                    grid[x,z] = -1;


            // Init shapes
            int[] shape = new int[1];
            shape[0] = 0; // Offset zero
            //blockShapes.Add(shape);

            

        }

        void Fill()
        {
            int freeCount = 0;
            int blockCount = 0;

            // Lets fill the four angle first
            FillSouthWestBorder();
            FillSouthEastBorder();
            FillNorthWestBorder();
            FillNorthEastBorder();


            for (int x = 0; x < width; x++)
            {
                
                for (int z = 0; z < height; z++)
                {
                    Debug.Log($"{x},{z}");
                    if (grid[x, z] > -1) continue;

                    Next(x, z, freeCount, blockCount);
                }
            }

           
          
            
        }

        void FillSouthWestBorder()
        {

            for (int i = 0; i < 4; i++)
            {
                int row = i;
                int col = 0;
                grid[col, row] = grid[col+1, row] = 0;
                if (i < 2)
                    grid[col+2, row] = grid[col+3, row] = 0;
            }
            
        }

        void FillSouthEastBorder()
        {
            for (int i = 0; i < 4; i++)
                {
                    int row = i;
                    int col = width-1;
                    grid[col, row] = grid[col-1, row] = 0;
                    if (i < 2)
                        grid[col-2, row] = grid[col-3, row] = 0;
                }
         
            
        }

        void FillNorthWestBorder()
        {
            for (int i = 0; i < 4; i++)
            {
                int row = height - 1 - i;
                int col = 0;
                grid[col, row] = grid[col+1, row] = 0;
                if (i < 2)
                    grid[col+2, row] = grid[col+3, row] = 0;
            }
        }

        void FillNorthEastBorder()
        {
            for (int i = 0; i < 4; i++)
            {
                int row = height - 1 - i;
                int col = width - 1;
                grid[col, row] = grid[col-1, row] = 0;
                if (i < 2)
                    grid[col-2, row] = grid[col-3, row] = 0;
            }
        }

        void Next(int x, int z, int freeCount, int blockCount)
        {
            // bool freeAvailable = CanAddFreeTile(index);
            // bool blockAvailable = CanAddBlockShape(index);
            Debug.Log($"Processing ({x},{z}), freeCount:{freeCount}, blockCount:{blockCount}");

            bool isBlock = false;
            int tot = freeCount + blockCount;
            if (tot < width * height / 10)
            {
                isBlock = UnityEngine.Random.Range(0, 2) == 1;
            }
            else
            {
                if (blockCount == 0)
                    isBlock = true;
                else
                    isBlock = (freeCount / blockCount > freeBlockRatioRatio) ? true : false;

            }

            if (isBlock)
            {
                // int[] shape = GetBlockShape(index);
                // if (shape != null)
                //     AddBlockShape(index, shape);
                if (!TryeAddBlockShape(x,z))
                    TryAddFreeTile(x,z);
            }
            else
            {
                if (!TryAddFreeTile(x,z))
                    TryeAddBlockShape(x,z);
            }
            

           
        }

        bool TryAddFreeTile(int x, int z)
        {
            Debug.Log($"Processing ({x},{z}) try adding free tile");

            int westIndex = x-1;
           
            int southIndex = z-1;
           
          

            if (z-1 != -1 && x-1 != -1 && grid[x,z-1] == 0 && grid[x-1,z] == 0)
            {
                if (grid[x-1, z-1] == 0) return false;

                // A square of 4 tiles
                (int,int)[] tiles = new (int,int)[] { (x,z), (x+1,z), (x,z+1), (x+1,z+1) };
                Debug.Log($"Processing ({x},{z}), adding {tiles.Length} tiles");
                AddFreeTiles(tiles);
                return true;
            }

            if (x-1 != -1 && grid[x-1,z] == 0) 
            {
                // No free tile from south, so we need at least another tile to north
                if (z+1 == height) return false; // No north tile

                // Fill from west
                (int,int)[] tiles = new[] { (x,z), (x,z+1) };
                Debug.Log($"Processing ({x},{z}), adding {tiles.Length} tiles");
                AddFreeTiles(tiles);
                return true;
            }

            if (z-1 != -1 && grid[x, z-1] == 0)
            {
                // There is no free tile from west, so we need at least another tile to east
                if (x+1 == width) return false;

                (int,int)[] tiles = new (int,int)[] { (x,z),(x+1,z) };
                Debug.Log($"Processing ({x},{z}), adding {tiles.Length} tiles");
                AddFreeTiles(tiles);
                return true;
            }

            return false;
        }

        bool TryeAddBlockShape(int x, int z)
        {
            return false;
        }

        // bool TryAddBlockShapes(int index)
        // {
        //     // Get available shapes
        //     List<int[]> availables = new List<int[]>();
        //     foreach (var shape in blockShapes)
        //     {
        //         if (IsBlockShapeAvailable(index, shape))
        //         {
        //             availables.Add(shape);            
        //         }
        //     }
        // }

        // bool IsBlockShapeAvailable(int index, int[] shape)
        // {

        // }
        void AddFreeTiles((int,int)[] tiles)
        {
            
            foreach ((int,int) tile in tiles)
                grid[tile.Item1, tile.Item2] = 0;

        }


        void AddBlockShape(int index, int[] shape)
        {
            
        }

       
        (int, int) IndexToCoords(int index)
        {
            return (index % width, index / width);
        }

        int CoordsToIndex(int col, int row)
        {
            return (col + row * width);
        }

      

     



        void DebugGrid()
        {
            string row = "";

            
            
            for (int z = 0; z < height; z++)   
            {
                for (int x = 0; x < width; x++)
                {
                    row += $" {grid[x,z]}";
                }
                row += "\n";

            }
            
            Debug.Log(row);
        }
        
    }
}