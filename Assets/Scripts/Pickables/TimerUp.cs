using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

        [SerializeField]
        GameObject mesh;

        bool picked = false;

        TimeUpSpawner spawner;

        TMP_Text tmpText;

        float textDist = 16;

        float rotateSpeed = 10;

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
            if (picked) return;

            var dist = Vector3.Distance(text.transform.position, PlayerController.Instance.transform.position);

            if (dist > textDist)
            {
                tmpText.color = new Color(1, 1, 1, 0);
            }
            else
            {
                tmpText.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), dist / textDist);
                text.transform.forward = Camera.main.transform.forward;
                mesh.transform.forward = Camera.main.transform.forward;
            }


            //mesh.transform.localEulerAngles = Quaternion.Euler(0, rotateSpeed * Time.deltaTime, 0) * mesh.transform.localEulerAngles;

            
        }

        void OnTriggerEnter(Collider other)
        {
            if (picked) return;

            if (!other.CompareTag("Player")) return;

            (GameMode1.Instance as GameMode1).IncreasePlayerChaseTime(amount);
            
            Vector3 forward = Camera.main.transform.forward.normalized;

            // POP OUT (punch scale)
            mesh.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 8, 1);

            // MUOVITI verso la camera.forward con un piccolo “lancio”
            mesh.transform.DOMove(mesh.transform.position + forward * 1.5f, 0.5f)
                .SetEase(Ease.OutQuad);

            // RUOTA veloce
            mesh.transform.DORotate(new Vector3(0, 720, 0), 0.5f, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuad);

            // SHRINK e distruggi dopo un po’
            mesh.transform.DOScale(0, 0.25f)
                .SetEase(Ease.InBack)
                .SetDelay(0.25f) 
                .OnComplete(() => TimeUpSpawner.Instance.ReportTimeUpPicked());




        }

        
    }
}