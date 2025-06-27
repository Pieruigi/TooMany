using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TMOT
{
    public class TimerUp : MonoBehaviour
    {
        [SerializeField]
        float amount = 5;

        [SerializeField]
        GameObject text;

        bool picked = false;

        TimeUpSpawner spawner;

        TMP_Text tmpText;

        float textDist = 16;

        void Awake()
        {
            tmpText = text.GetComponent<TMP_Text>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var dist = Vector3.Distance(text.transform.position, PlayerController.Instance.transform.position);

            if (dist > textDist)
            {
                tmpText.color = new Color(1, 1, 1, 0);
            }
            else
            {
                tmpText.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), dist / textDist);
                text.transform.forward = Camera.main.transform.forward;    
            }
            

            
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