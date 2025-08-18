#define PLAYER_FIXED
using System;
using System.Collections;
using System.Collections.Generic;
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


        void Awake()
        {
            //size = new Vector2((pinRoot.transform as RectTransform).rect.width, (pinRoot.transform as RectTransform).rect.height);
        }

        // Start is called before the first frame update
        void Start()
        {
            sizeRatio = LevelController.Instance.MapSize.x * 50f / (pinRoot.parent as RectTransform).sizeDelta.x;
            mapRadius = ((pinRoot.parent as RectTransform).sizeDelta.x - 4) / 2;
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
            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
            MedicalSpawner.OnMedicalDroneSpawned += HandleOnTimeUpSpawned;
            MedicalSpawner.OnMedicalDroneUnspawned += HandleOnObjectRemoved;
            DiamondSpawner.OnDiamondSpanwed += HandleOnDiamondSpawned;
            DiamondSpawner.OnDiamondUnspanwed += HandleOnObjectRemoved;
        }

        void OnDisable()
        {
            MonsterSpawner.OnMonsterAdded -= HandleOnMonsterAdded;
            MonsterSpawner.OnMonsterRemoved -= HandleOnObjectRemoved;
            TimeUpSpawner.OnTimeUpSpawned -= HandleOnTimeUpSpawned;
            TimeUpSpawner.OnTimeUpUnspawned -= HandleOnObjectRemoved;
            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
            MedicalSpawner.OnMedicalDroneSpawned -= HandleOnTimeUpSpawned;
            MedicalSpawner.OnMedicalDroneUnspawned -= HandleOnObjectRemoved;
            DiamondSpawner.OnDiamondSpanwed -= HandleOnDiamondSpawned;
            DiamondSpawner.OnDiamondUnspanwed -= HandleOnObjectRemoved;
        }

        

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
         
            UpdateMonsterPinColors();
            
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



        private void HandleOnMonsterAdded(GameObject monster)
        {
            var pin = Instantiate(monsterPinPrefab, pinRoot);
            pins.Add(pin, monster);

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
            t.DOScale(s.x, shakeDuration).SetEase(Ease.InOutElastic);

            // Shake map
            (transform.GetChild(0) as RectTransform).DOShakePosition(shakeDuration, mapShakeStrength).SetEase(Ease.InOutElastic);
        }

        void ShakeOut(GameObject pin)
        {
            // Shake pin
            var t = pin.transform.GetChild(0) as RectTransform;
            t.DOShakeAnchorPos(shakeDuration, shakeStrength).SetEase(Ease.InOutElastic);
            t.DOScale(0, shakeDuration).SetEase(Ease.InOutElastic);

            // Shake map
            (transform.GetChild(0) as RectTransform).DOShakePosition(shakeDuration, mapShakeStrength).SetEase(Ease.InOutElastic);
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

                    index = 0;

                    key.GetComponent<MapPinColorSetter>().SetColor(index);
                }
            }
        }
    }
}