#define PLAYER_FIXED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DG.Tweening;
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
        GameObject diamondPinPrefab;

        [SerializeField]
        GameObject pillPinPrefab;

        [SerializeField]
        GameObject batteryPinPrefab;

        [SerializeField]
        GameObject medicalPinPrefab;

          

        [SerializeField]
        Transform pinRoot;

        // [SerializeField]
        // float scale = .3f;

        // Key: pin
        // Value: monster
        Dictionary<GameObject, GameObject> pins = new Dictionary<GameObject, GameObject>();

        float elapsed = 0;
        float time = 0;

        float sizeRatio;
        float mapRadius;

        float shakeDuration = .5f;
        float shakeStrength = 10f;

        float mapShakeStrength = 20;

        Vector3 rootLocalPositionDefault;


        void Awake()
        {
            //size = new Vector2((pinRoot.transform as RectTransform).rect.width, (pinRoot.transform as RectTransform).rect.height);
        }

        // Start is called before the first frame update
        void Start()
        {
            sizeRatio = LevelController.Instance.MapSize.x * 50f / (pinRoot.parent as RectTransform).sizeDelta.x;
            mapRadius = ((pinRoot.parent as RectTransform).sizeDelta.x - 4) / 2;
            rootLocalPositionDefault = transform.GetChild(0).localPosition;
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
            TimeUpMultiSpawner.OnTimeUpMultiSpawned += HandleOnTimeupMultiSpawned;
            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
            MedicalSpawner.OnMedicalDroneSpawned += HandleOnMedicalSpawned;
            MedicalSpawner.OnMedicalDroneUnspawned += HandleOnObjectRemoved;
            DiamondSpawner.OnDiamondSpanwed += HandleOnDiamondSpawned;
            DiamondSpawner.OnDiamondUnspanwed += HandleOnObjectRemoved;
            PillSpawner.OnSpawned += HandleOnPillSpawned;
            PillSpawner.OnUnspawned += HandleOnObjectRemoved;
            MonsterController.OnForcedBehaviour += HandleOnMonsterForcedBehaviour;
            BatterySpawner.OnSpawned += HandleOnBatterySpawned;
            BatterySpawner.OnUnspawned += HandleOnObjectRemoved;
        }


        void OnDisable()
        {
            MonsterSpawner.OnMonsterAdded -= HandleOnMonsterAdded;
            MonsterSpawner.OnMonsterRemoved -= HandleOnObjectRemoved;
            TimeUpSpawner.OnTimeUpSpawned -= HandleOnTimeUpSpawned;
            TimeUpSpawner.OnTimeUpUnspawned -= HandleOnObjectRemoved;
            TimeUpMultiSpawner.OnTimeUpMultiSpawned -= HandleOnTimeupMultiSpawned;
            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
            MedicalSpawner.OnMedicalDroneSpawned -= HandleOnMedicalSpawned;
            MedicalSpawner.OnMedicalDroneUnspawned -= HandleOnObjectRemoved;
            DiamondSpawner.OnDiamondSpanwed -= HandleOnDiamondSpawned;
            DiamondSpawner.OnDiamondUnspanwed -= HandleOnObjectRemoved;
            PillSpawner.OnSpawned -= HandleOnPillSpawned;
            PillSpawner.OnUnspawned -= HandleOnObjectRemoved;
            MonsterController.OnForcedBehaviour -= HandleOnMonsterForcedBehaviour;
            BatterySpawner.OnSpawned -= HandleOnBatterySpawned;
            BatterySpawner.OnUnspawned -= HandleOnObjectRemoved;
        }

        private void HandleOnMonsterForcedBehaviour(MonsterController monsterController)
        {
            UpdateMonsterPinColors();
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
         
            UpdateMonsterPinColors();
            
        }

        private void HandleOnMedicalSpawned(GameObject drone)
        {
            var pin = Instantiate(medicalPinPrefab, pinRoot);
            pins.Add(pin, drone);

            ShakeIn(pin);
        }

        private void HandleOnBatterySpawned(GameObject drone)
        {
            var pin = Instantiate(batteryPinPrefab, pinRoot);
            pins.Add(pin, drone);

            ShakeIn(pin);
        }

        private void HandleOnPillSpawned(GameObject drone)
        {
            var pin = Instantiate(pillPinPrefab, pinRoot);
            pins.Add(pin, drone);

            ShakeIn(pin);
        }

        private void HandleOnDiamondSpawned(GameObject diamond)
        {
            var pin = Instantiate(diamondPinPrefab, pinRoot);
            pins.Add(pin, diamond);

            ShakeIn(pin);
        }

        private void HandleOnTimeUpSpawned(GameObject timeUp)
        {
            var pin = Instantiate(timeUpPinPrefab, pinRoot);
            pins.Add(pin, timeUp);

            ShakeIn(pin);
        }

        
      

        private void HandleOnTimeupMultiSpawned(List<GameObject> list)
        {
            foreach (GameObject g in list)
            {
                var pin = Instantiate(timeUpPinPrefab, pinRoot);
                pins.Add(pin, g);
                ShakeIn(pin);
            }
        }



        private void HandleOnMonsterAdded(GameObject monster)
        {
            var pin = Instantiate(monsterPinPrefab, pinRoot);
            pins.Add(pin, monster);

            // Set pin color

            int index = 0;
            if (PlayerController.Instance.State == PlayerState.Prey)
                index = !monster.GetComponent<MonsterController>().InvertedBehaviour ? 0 : 1;
            else
                index = !monster.GetComponent<MonsterController>().InvertedBehaviour ? 1 : 0;
            pin.GetComponent<MapPinColorSetter>().SetColor(index);

            // Do shake 
            ShakeIn(pin);
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
                Destroy(keyToRemove, 1f);
                // Do shake
                ShakeOut(keyToRemove);
            }


        }

        void ShakeIn(GameObject pin)
        {
            // Shake pin
            var t = pin.transform.GetChild(0) as RectTransform;
            var s = t.localScale;
            t.DOShakeAnchorPos(shakeDuration, shakeStrength).SetEase(Ease.InOutElastic);
            t.DOScale(s.x, shakeDuration).SetEase(Ease.InOutElastic).OnComplete(()=> { t.localScale = s; });

            // Shake map
            (transform.GetChild(0) as RectTransform).DOShakePosition(shakeDuration, mapShakeStrength).SetEase(Ease.InOutElastic).onComplete += ()=> { transform.GetChild(0).localPosition = rootLocalPositionDefault; };
        }

        void ShakeOut(GameObject pin)
        {
            // Shake pin
            var t = pin.transform.GetChild(0) as RectTransform;
            t.DOShakeAnchorPos(shakeDuration, shakeStrength).SetEase(Ease.InOutElastic);
            t.DOScale(0, shakeDuration).SetEase(Ease.InOutElastic);

            // Shake map
            (transform.GetChild(0) as RectTransform).DOShakePosition(shakeDuration, mapShakeStrength).SetEase(Ease.InOutElastic).onComplete += ()=> { transform.GetChild(0).localPosition = rootLocalPositionDefault; };
        }


        void UpdatePinPositions()
        {
            foreach (var m in pins.Keys)
            {
                var mc = pins[m];
                var relPos = mc.transform.position - PlayerController.Instance.transform.position;
                relPos.y = 0;
                relPos = Quaternion.Euler(0, -PlayerController.Instance.transform.eulerAngles.y, 0f) * relPos;
                var radarPos = new Vector2(relPos.x, relPos.z) * sizeRatio;
                if (radarPos.magnitude > mapRadius)
                {
                    radarPos = radarPos.normalized * mapRadius;
                }

                // 5️⃣ Aggiorna la posizione dell'icona del nemico
                (m.transform as RectTransform).anchoredPosition = radarPos;


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