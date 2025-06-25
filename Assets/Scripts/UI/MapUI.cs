using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Rendering;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace TMOT.UI
{
    public class MapUI : MonoBehaviour
    {
        [SerializeField]
        GameObject playerPin;

        [SerializeField]
        GameObject monsterPinPrefab;

        [SerializeField]
        Transform pinRoot;

        // [SerializeField]
        // float scale = .3f;

        // Key: pin
        // Value: monster
        Dictionary<GameObject, GameObject> monsters = new Dictionary<GameObject, GameObject>();

        float elapsed = 0;
        float time = 0;

        Vector2 size;

        Vector2 sizeRatio;


        void Awake()
        {
            size = new Vector2((pinRoot.transform as RectTransform).rect.width, (pinRoot.transform as RectTransform).rect.height);
        }

        // Start is called before the first frame update
        void Start()
        {
            sizeRatio = new Vector2(size.x / LevelController.Instance.MapSize.x, size.y / LevelController.Instance.MapSize.y);

            
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (Time.timeScale == 1)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;        
            }
        }

        void LateUpdate()
        {
            elapsed += Time.deltaTime;
            if (elapsed > time)
            {
                elapsed -= time;

                // Player
                UpdatePlayerPosition();
                UpdatePlayerRotation();

                // Monsters
                // CheckForBrokenMonsters();
                // CheckForNewMonsters();
                UpdateMonsterPositions();
            }



        }


        void OnEnable()
        {
            MonsterSpawner.OnMonsterAdded += HandleOnMonsterAdded;
            MonsterSpawner.OnMonsterRemoved += HandleOnMonsterRemoved;
        }

        void OnDisable()
        {
            MonsterSpawner.OnMonsterAdded -= HandleOnMonsterAdded;
            MonsterSpawner.OnMonsterRemoved -= HandleOnMonsterRemoved;
        }

        private void HandleOnMonsterAdded(GameObject monster)
        {
            var pin = Instantiate(monsterPinPrefab, pinRoot);
            monsters.Add(pin, monster);
        }

        private void HandleOnMonsterRemoved(GameObject monster)
        {
            Debug.Log($"Removing monster:{monster}");
            GameObject keyToRemove = null;
            foreach (var key in monsters.Keys)
            {
                if (monsters[key] == monster)
                {
                    keyToRemove = key;
                    break;
                }
            }
            Debug.Log($"Key found:{keyToRemove}");
            if (keyToRemove)
            {
                monsters.Remove(keyToRemove);
                Destroy(keyToRemove);
            }
                

        }

        void UpdatePlayerPosition()
        {
            var pos = new Vector2(PlayerController.Instance.transform.position.x, PlayerController.Instance.transform.position.z);
            pos.x *= sizeRatio.x;
            pos.y *= sizeRatio.y;
            playerPin.transform.localPosition = pos;
        }

        void UpdatePlayerRotation()
        {
            playerPin.transform.rotation = Quaternion.Euler(0,0,-PlayerController.Instance.transform.eulerAngles.y);
        }

        // void CheckForBrokenMonsters()
        // {
        //     List<GameObject> toRemove = new List<GameObject>();
        //     foreach (var key in monsters.Keys)
        //     {
        //         if (monsters[key] == null)
        //             toRemove.Add(key);
        //     }
        //     foreach (var t in toRemove)
        //         monsters.Remove(t);
        // }

        // void CheckForNewMonsters()
        // {
        //     foreach (var m in MonsterSpawner.Instance.Monsters)
        //     {
        //         if (!monsters.ContainsValue(m))
        //         {
        //             // Add pin for this monster
        //             var pin = Instantiate(monsterPinPrefab, pinRoot);
        //             monsters.Add(pin, m);
        //         }
        //     }
        // }

        void UpdateMonsterPositions()
        {
            foreach (var m in monsters.Keys)
            {
                var mc = monsters[m];
                var pos = new Vector2(mc.transform.position.x, mc.transform.position.z);
                pos.x *= sizeRatio.x;
                pos.y *= sizeRatio.y;
                m.transform.localPosition = pos;
            }
        }
    }
}