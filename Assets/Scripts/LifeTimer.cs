using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class LifeTimer : MonoBehaviour
    {
        [SerializeField]
        float lifeTime = 5;

        // Start is called before the first frame update
        void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}