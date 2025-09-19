using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMOT
{
    public class LevelController : Singleton<LevelController>
    {

        // [SerializeField]
        // List<GameObject> gameModePrefabs;

        [SerializeField]
        Vector2 mapSize;
        public Vector2 MapSize
        {
            get{ return mapSize; }
        }

        // [SerializeField]
        // LevelBuilder levelBuilder;

        [SerializeField]
        Color playerPreyColor;
        public Color PlayerPreyColor
        {
            get{ return playerPreyColor; }
        }

        [SerializeField]
        Color playerHunterColor;
        public Color PlayerHunterColor
        {
            get{ return playerHunterColor; }
        }

        [SerializeField]
        Transform waypointRoot;

        public GameObject GameMode { get; private set; }

        //public IList<Transform> Waypoints { get{ return levelBuilder.Waypoints; } }
        //[SerializeField]
        List<Transform> waypoints;
        public IList<Transform> Waypoints { get { return waypoints; } }


        protected override void Awake()
        {
            base.Awake();
            Debug.Log("TEST - LevelController - Awake()");
        }


        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("LevelController - Start");
        }

        // Update is called once per frame
        void Update()
        {
        

        }



        public void Initialize()
        {
            waypoints = waypointRoot.GetComponentsInChildren<Transform>().Where(w => w != waypointRoot).ToList();

            Debug.Log("TEST - LevelController - initialize");
            // Instantiate the game mode objet
            var prefab = GameManager.Instance.GameModePrefabs.ToList()[(int)GameManager.Instance.GameMode];
            var gm = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            GameMode = gm;

            // Build level
            //levelBuilder.Build();
            // Move player to a random position
            PlayerController.Instance.transform.position = waypoints[Random.Range(0, waypoints.Count)].position;
            PlayerController.Instance.ForceRotation(Random.Range(0, 360));

        }


    }
}