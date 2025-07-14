using System;
using System.Collections;
using System.Collections.Generic;
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
        GameObject timeUpPinPrefab;

        [SerializeField]
        Transform pinRoot;

        // [SerializeField]
        // float scale = .3f;

        // Key: pin
        // Value: monster
        Dictionary<GameObject, GameObject> pins = new Dictionary<GameObject, GameObject>();

        float elapsed = 0;
        float time = 0;

        Vector2 size;

        Vector2 sizeRatio;


        void Awake()
        {
            //size = new Vector2((pinRoot.transform as RectTransform).rect.width, (pinRoot.transform as RectTransform).rect.height);
        }

        // Start is called before the first frame update
        void Start()
        {
            size = new Vector2(LevelController.Instance.MapSize.x * 10f, LevelController.Instance.MapSize.y * 10f);
            (pinRoot.parent as RectTransform).sizeDelta = size;
            sizeRatio = new Vector2(size.x / LevelController.Instance.MapSize.x, size.y / LevelController.Instance.MapSize.y);


        }

        // Update is called once per frame
        void Update()
        {
            
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

                // Others
                UpdatePinPositions();
            }



        }


        void OnEnable()
        {
            MonsterSpawner.OnMonsterAdded += HandleOnMonsterAdded;
            MonsterSpawner.OnMonsterRemoved += HandleOnObjectRemoved;
            TimeUpSpawner.OnTimeUpSpawned += HandleOnTimeUpSpawned;
            TimeUpSpawner.OnTimeUpUnspawned += HandleOnObjectRemoved;
            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
        }

        void OnDisable()
        {
            MonsterSpawner.OnMonsterAdded -= HandleOnMonsterAdded;
            MonsterSpawner.OnMonsterRemoved -= HandleOnObjectRemoved;
            TimeUpSpawner.OnTimeUpSpawned -= HandleOnTimeUpSpawned;
            TimeUpSpawner.OnTimeUpUnspawned -= HandleOnObjectRemoved;
            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
         
            UpdateMonsterPinColors();
            
        }

        private void HandleOnTimeUpSpawned(GameObject timeUp)
        {
            var pin = Instantiate(timeUpPinPrefab, pinRoot);
            pins.Add(pin, timeUp);
        }



        private void HandleOnMonsterAdded(GameObject monster)
        {
            var pin = Instantiate(monsterPinPrefab, pinRoot);
            pins.Add(pin, monster);
        }

        private void HandleOnObjectRemoved(GameObject obj)
        {
          
            GameObject keyToRemove = null;
            foreach (var key in pins.Keys)
            {
                if (pins[key] == obj)
                {
                    keyToRemove = key;
                    break;
                }
            }
          
            if (keyToRemove)
            {
                pins.Remove(keyToRemove);
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
            playerPin.transform.localRotation = Quaternion.Euler(0, 0, -PlayerController.Instance.transform.eulerAngles.y);
        }


        void UpdatePinPositions()
        {
            foreach (var m in pins.Keys)
            {
                var mc = pins[m];
                var pos = new Vector2(mc.transform.position.x, mc.transform.position.z);
                pos.x *= sizeRatio.x;
                pos.y *= sizeRatio.y;
                m.transform.localPosition = pos;
            }
        }

        void UpdateMonsterPinColors()
        {
            
            foreach (var key in pins.Keys)
            {
                if (pins[key].CompareTag("Monster"))
                {
                    var mc = pins[key].GetComponent<MonsterController>();
                    int index;
                    if (PlayerController.Instance.State == PlayerState.Prey)
                        index = !mc.InvertedBehaviour ? 0 : 1;
                    else
                        index = !mc.InvertedBehaviour ? 1 : 0;

                    key.GetComponent<MapPinColorSetter>().SetColor(index);
                }
            }
        }
    }
}