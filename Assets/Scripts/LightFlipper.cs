using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class LightFlipper : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            var root = transform.root;
            if (root.localScale.z < 0)
            {
                var light = transform.GetComponentInChildren<Light>();
                // var scale = light.transform.localScale;
                // scale.z *= -1;
                // light.transform.localScale = scale;
                var eulers = light.transform.localEulerAngles;
                eulers.x *= -1;
                light.transform.localEulerAngles = eulers;
            }
                
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}