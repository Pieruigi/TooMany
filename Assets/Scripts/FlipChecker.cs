using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    
    public class FlipChecker : MonoBehaviour
    {
        public enum FlipAxis { X, Y, Z }

        [SerializeField]
        FlipAxis axis;


        void Awake()
        {


        }

        // Start is called before the first frame update
        void Start()
        {
            var root = transform.root;

            if (Mathf.Sign(root.localScale.x) * Mathf.Sign(root.localScale.z) < 0)
            {
                var scale = transform.localScale;

                switch (axis)
                {
                    case FlipAxis.X:
                        scale.x *= -1;
                        break;
                    case FlipAxis.Y:
                        scale.y *= -1;
                        break;
                    case FlipAxis.Z:
                        scale.z *= -1;
                        break;
                }
                
                //scale.z *= Mathf.Sign(root.localScale.z);

                transform.localScale = scale;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}