using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class LevelController : Singleton<LevelController>
    {

        [SerializeField]
        List<GameObject> gameModePrefabs;

        [SerializeField]
        Vector2 mapSize;
        public Vector2 MapSize
        {
            get{ return mapSize; }
        }
        

        public GameObject GameMode { get; private set; }

      
        protected override void Awake()
        {
            base.Awake();
        }


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        

        }
        
        

        public void Initialize()
        {
            // Instantiate the game mode objet
            var prefab = gameModePrefabs[(int)GameManager.Instance.GameMode];
            var gm = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            GameMode = gm;



        }


    }
}