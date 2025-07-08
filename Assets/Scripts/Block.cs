using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class Block : MonoBehaviour
    {
        [SerializeField]
        List<Transform> waypoints;

        public IList<Transform> Waypoints {get{ return waypoints.AsReadOnly(); }}

        // Start is called before the first frame update
        void Start()
        {
            var wg = transform.Find("Waypoints");
            waypoints = new List<Transform>();
            for (int i = 0; i < wg.childCount; i++)
                waypoints.Add(wg.GetChild(i));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}