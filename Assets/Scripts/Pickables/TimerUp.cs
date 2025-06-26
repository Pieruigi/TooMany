using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class TimerUp : MonoBehaviour
    {
        [SerializeField]
        float amount = 5;

        bool picked = false;

        TimeUpSpawner spawner;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerEnter(Collider other)
        {
            if (picked) return;

            if (!other.CompareTag("Player")) return;

            (GameMode1.Instance as GameMode1).IncreasePlayerChaseTime(amount);
            TimeUpSpawner.Instance.ReportTimeUpPicked();
        }

        
    }
}