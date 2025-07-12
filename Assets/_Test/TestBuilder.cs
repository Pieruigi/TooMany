using System;
using System.Collections;
using System.Collections.Generic;
using TMOT;
using Unity.Mathematics;
using UnityEngine;

public class TestBuilder : MonoBehaviour
{
    [SerializeField]
    GameObject emptyBlockPrefab;

    [SerializeField]
    GameObject fullBlockPrefab;

    [SerializeField]
    GameObject notUsedBlockPrefab;

    [SerializeField]
    LevelBuilder builder;

    int blockSize = 2;


    List<GameObject> blocks = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
        Build();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Build();
        }
    }

    void OnEnable()
    {
        LevelBuilder.OnLevelBuilt += HandleOnLevelBuilt;
        LevelBuilder.OnLevelBuilderFailed += HandleOnLevelBuilderFailed;
    }

    void OnDisable()
    {
        LevelBuilder.OnLevelBuilt -= HandleOnLevelBuilt;
        LevelBuilder.OnLevelBuilderFailed -= HandleOnLevelBuilderFailed;
    }

    private void HandleOnLevelBuilderFailed(int width, int height, int[,] grid)
    {
        HandleOnLevelBuilt(width, height, grid);
    }

    private void HandleOnLevelBuilt(int width, int height, int[,] grid)
    {
        
        foreach (var b in blocks)
            Destroy(b);

        blocks.Clear();
        

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * blockSize, 0, z * blockSize);
                GameObject b;
                if (grid[x, z] == 1)
                    b = Instantiate(fullBlockPrefab, pos, Quaternion.identity, transform);
                else if(grid[x,z] == 0)
                    b = Instantiate(emptyBlockPrefab, pos, Quaternion.identity, transform);
                else
                    b = Instantiate(notUsedBlockPrefab, pos, Quaternion.identity, transform);
                
                 blocks.Add(b);
            }
        }


        // for (int i = 0; i < width; i++)
        // {
        //     int x = i * blockSize;
        //     for (int j = 0; j < height; j++)
        //     {

        //         int z = j * blockSize;
        //         //var g = grid[i + j * width];
        //         var g = grid[i, j];
        //         var prefab = g == 0 ? emptyBlockPrefab : (g == 1 ? fullBlockPrefab : notUsedBlockPrefab);
        //         var b = Instantiate(prefab, new Vector3(x, 0, z), Quaternion.identity);
        //         b.SetActive(true);
        //         blocks.Add(b);
        //     }

        // }
    }

    void Build()
    {
        builder.Build();




    }
    
    

}
