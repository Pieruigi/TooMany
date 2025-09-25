using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace TMOT
{
    public class DiamondSpawner : Singleton<DiamondSpawner>
    {
        public delegate void DiamondSpawnedDelegate(GameObject diamond);
        public static DiamondSpawnedDelegate OnDiamondSpanwed;

        public delegate void DiamondUnspawnedDelegate(GameObject diamond);
        public static DiamondUnspawnedDelegate OnDiamondUnspanwed;

        [SerializeField]
        GameObject diamondPrefab;

        List<GameObject> diamonds = new List<GameObject>();

        float diamondToPlayerMinDistance = 16;
        float diamondToDiamondMinDistance = 16;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        

        public void SpawnDiamond()
        {
            // Get a spawn point 
            var candidates = LevelController.Instance.Waypoints.Where(w => Vector3.Distance(PlayerController.Instance.transform.position, w.position) > diamondToPlayerMinDistance).ToList();

            // Not too close to any existing diamond
            List<Transform> toRemoveList = new List<Transform>();
            foreach (var diamond in diamonds)
            {
                toRemoveList.AddRange(candidates.Where(w => Vector3.Distance(w.position, diamond.transform.position) < diamondToDiamondMinDistance));
            }

            foreach (var r in toRemoveList)
                candidates.Remove(r);

            var spawnPoint = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            var d = Instantiate(diamondPrefab, spawnPoint.position, Quaternion.identity);

            diamonds.Add(d);

            OnDiamondSpanwed?.Invoke(d);
        }

        public void UnspawnAllDiamonds()
        {
            foreach (var diamond in diamonds)
            {
                diamond.GetComponent<CustomDroneController>().ForceDestroy();
                Destroy(diamond, 2f);
                OnDiamondUnspanwed?.Invoke(diamond);
            }

            diamonds.Clear();

        }

        
        public void UnspawnDiamond(GameObject diamond)
        {
            diamonds.Remove(diamond);
            Destroy(diamond, 1f);
            OnDiamondUnspanwed?.Invoke(diamond);
        }
        
        
    }
}