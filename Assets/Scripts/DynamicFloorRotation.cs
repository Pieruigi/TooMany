using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

namespace TMOT
{
    public class DynamicRFloorRotation : MonoBehaviour
    {


        // Start is called before the first frame update
        void Start()
        {
            
            transform.localEulerAngles = transform.root.eulerAngles;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}