using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class PlayerCamera : MonoBehaviour
    {
       

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void LateUpdate()
        {
            // Apply pitch
            var eulerAngles = transform.eulerAngles;
            eulerAngles.x = PlayerController.Instance.Pitch;
            transform.eulerAngles = eulerAngles;
            
        }
    }
}