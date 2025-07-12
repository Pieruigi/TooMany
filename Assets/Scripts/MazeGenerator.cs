using UnityEngine;

public class PacManMazeGenerator : MonoBehaviour
{
    public int width = 20;  // larghezza in tile
    public int height = 20; // altezza in tile
    public int cellSize = 4; // ogni blocco muro occupa cellSize x cellSize
    public GameObject wallPrefab;
    public GameObject floorPrefab;

    private int[,] grid; // 0 = corridoio, 1 = muro

    void Start()
    {
        GenerateMaze();
        BuildMaze();
    }

    void GenerateMaze()
    {
        grid = new int[width, height];

        // Itera sulle celle per creare i blocchi muro
        for (int x = 0; x < width; x += cellSize + 2) // +2 per lasciare corridoio largo 2
        {
            for (int z = 0; z < height; z += cellSize + 2)
            {
                bool placeBlock = Random.value > 0.3f; // 70% probabilità di muro

                if (placeBlock)
                {
                    int blockWidth = Mathf.Clamp(cellSize, 1, width - x);
                    int blockHeight = Mathf.Clamp(cellSize, 1, height - z);

                    for (int bx = 0; bx < blockWidth; bx++)
                    {
                        for (int bz = 0; bz < blockHeight; bz++)
                        {
                            if (x + bx < width && z + bz < height)
                                grid[x + bx, z + bz] = 1;
                        }
                    }
                }
            }
        }
    }

    void BuildMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x, 0, z);
                if (grid[x, z] == 1)
                    Instantiate(wallPrefab, pos, Quaternion.identity, transform);
                else
                    Instantiate(floorPrefab, pos, Quaternion.identity, transform);
            }
        }
    }
}
