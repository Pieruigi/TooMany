using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class WayPointManager : Singleton<WayPointManager>
    {
        [SerializeField]
        List<Transform> wayPoints;

        public IList<Transform> WayPoints
        {
            get { return wayPoints; }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


    }
}