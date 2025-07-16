using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

namespace TMOT
{
    public class MedicalController : MonoBehaviour
    {

        NavMeshAgent agent;

        float waypointMaxDistance = 10f;

        float reachDistance = 1f;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (agent.pathPending)
                return;

            bool computePath = false;
            if (agent.path == null || agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.pathStatus == NavMeshPathStatus.PathPartial)
                computePath = true;

            if (DestinationReached())
                computePath = true;

            if (computePath)
            {
                var waypoint = GetNextWaypoint();
                if (waypoint) agent.SetDestination(waypoint.position);
            }

            

        }


        Transform GetNextWaypoint()
        {
            Transform ret = null;

            // Get all waypoints near to the agent
            var waypoints = LevelController.Instance.Waypoints.ToList().FindAll(w => Vector3.Distance(transform.position, w.position) < waypointMaxDistance);

            // Remove all the waypoints that are behind the agent
            var angle = 90f;

            waypoints = waypoints.FindAll(w =>
            {
                var dir = Vector3.ProjectOnPlane(w.position - transform.position, Vector3.up);
                return Vector3.Angle(transform.forward, dir) < angle;
            });

            // Get only waypoints not blocked by walls
            waypoints = waypoints.FindAll(w =>
            {
                var origin = transform.position + Vector3.up * .25f;
                var direction = Vector3.ProjectOnPlane(w.position - transform.position, Vector3.up);
                return !Physics.Raycast(origin, direction, direction.magnitude, LayerMask.GetMask("Wall"));
            });

            // Get a random wapypoint
            if (waypoints.Count > 0)
                ret = waypoints[UnityEngine.Random.Range(0, waypoints.Count)];      

            return ret;

        }

        bool DestinationReached()
        {
            if (agent.path == null) return false;

            if (Vector3.Distance(agent.destination, transform.position) > reachDistance)
                return false;

            return true;

        }
    }
}