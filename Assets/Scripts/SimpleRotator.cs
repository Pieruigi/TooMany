using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace TMOT
{

    public class SimpleRotator : MonoBehaviour
    {
        [SerializeField]
        Vector3 eulers;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(eulers * Time.deltaTime, Space.Self);
        }
    }
}