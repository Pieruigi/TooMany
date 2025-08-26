using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace TMOT
{
    public class FanController : MonoBehaviour
    {
        float speed = 15;

        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.eulerAngles += speed * Time.deltaTime * Vector3.up;
        }
    }
}