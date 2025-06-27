using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace TMOT
{
    public class LevelBuilder : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> topLeftBlockPrefabs;

        [SerializeField]
        List<GameObject> topBlockPrefabs;

        [SerializeField]
        List<GameObject> middleBlockPrefabs;

        List<GameObject> blocks = new List<GameObject>();

        List<Transform> waypoints = new List<Transform>();
        public IList<Transform> Waypoints {get{ return waypoints.AsReadOnly(); }}

        int size = 3; // In blocks

        int index = 0;

        float blockSize = 18;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Build()
        {
            int count = size * size;
            float x = 0;
            float z = 0;
            int s = (size - 1) / 2;
            for (int i = 0; i < count; i++)
            {
                x = (i % size - s) * blockSize;
                z = (s - i / size) * blockSize;

                // Instantiate block
                var block = Instantiate(GetBlockPrefab(x, z));
                // Set position
                SetBlockPosition(block, x, z);

                // Check for axis inversion
                SetBlockRotation(block, x, z);

                // Check for inversion
                SetBlockInversion(block, x, z);

                // Add waypoints
                waypoints.AddRange(block.GetComponent<Block>().Waypoints);

                blocks.Add(block);
            }

            blocks[4].GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
        }

        GameObject GetBlockPrefab(float x, float z)
        {

            if (x < 0 && z > 0)
                return topLeftBlockPrefabs[UnityEngine.Random.Range(0, topLeftBlockPrefabs.Count)];

            if (x == 0 && z > 0)
                return topBlockPrefabs[UnityEngine.Random.Range(0, topBlockPrefabs.Count)];

            if (x > 0 && z > 0)
                return topLeftBlockPrefabs[UnityEngine.Random.Range(0, topLeftBlockPrefabs.Count)];

            if (x < 0 && z == 0)
                return topBlockPrefabs[UnityEngine.Random.Range(0, topBlockPrefabs.Count)];

            if (x == 0 && z == 0)
                return middleBlockPrefabs[UnityEngine.Random.Range(0, middleBlockPrefabs.Count)];

            if (x > 0 && z == 0)
                return topBlockPrefabs[UnityEngine.Random.Range(0, topBlockPrefabs.Count)];

            if (x < 0 && z < 0)
                return topLeftBlockPrefabs[UnityEngine.Random.Range(0, topLeftBlockPrefabs.Count)];

            if (x == 0 && z < 0)
                return topBlockPrefabs[UnityEngine.Random.Range(0, topBlockPrefabs.Count)];

            //if (x > 0 && z < 0)
            return topLeftBlockPrefabs[UnityEngine.Random.Range(0, topLeftBlockPrefabs.Count)];


        }

        void SetBlockPosition(GameObject block, float x, float z)
        {
            var pos = Vector3.zero;
            pos.x = x;
            pos.z = z;
            block.transform.position = pos;
        }

        void SetBlockRotation(GameObject block, float x, float z)
        {
            if (z > 0) return;

            if (z == 0)
            {
                if (x < 0)
                {
                    block.transform.rotation = Quaternion.Euler(0, -90f, 0);
                    return;
                }
                if (x > 0)
                {
                    block.transform.rotation = Quaternion.Euler(0, 90f, 0);
                    return;
                }
            }

            if (z < 0 && x == 0)
                block.transform.rotation = Quaternion.Euler(0, 180f, 0);
        }

        void SetBlockInversion(GameObject block, float x, float z)
        {
            if (z > 0 && x < 0) return;

            if (z > 0 && x > 0)
            {
                var s = block.transform.localScale;
                s.x *= -1;
                block.transform.localScale = s;
                return;
            }

            if (z < 0 && x < 0)
            {
                var s = block.transform.localScale;
                s.z *= -1;
                block.transform.localScale = s;
                return;
            }


            if (z < 0 && x > 0)
            {
                var s = block.transform.localScale;
                s.x *= -1;
                s.z *= -1;
                block.transform.localScale = s;
                return;
            }

        }

        
    }
}