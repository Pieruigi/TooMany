using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

namespace TMOT
{
    enum CustomDroneType {Medical, TimeUp}

    public class CustomDroneController : MonoBehaviour
    {

        [SerializeField]
        CustomDroneType type;

        NavMeshAgent agent;

        float waypointMaxDistance = 10f;

        float reachDistance = 1f;

        float hitDistance = 2f;

        bool picked = false;

        [SerializeField]
        List<Rigidbody> parts;

        [SerializeField]
        ParticleSystem destroyParticlePrefab;

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

            if (picked) return;

            if (agent.pathPending) return;

            bool computePath = false;
            if (agent.path == null || agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.pathStatus == NavMeshPathStatus.PathPartial)
                computePath = true;

            if (DestinationReached())
                computePath = true;

            if (computePath)
            {
                var waypoint = GetNextWaypoint();
                Debug.Log("TEST - Waypoint to reach:" + waypoint.gameObject);
                if (waypoint) agent.SetDestination(waypoint.position);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (picked) return;

            if (!other.CompareTag("Player")) return;

            picked = true;

            DoAction();

            Explode();

            //await Task.Delay(TimeSpan.FromSeconds(1f));

            ReportPickedUp();
        }



        void DoAction()
        {
            switch (type)
            {
                case CustomDroneType.Medical:
                    PlayerController.Instance.Heal();
                    break;
                case CustomDroneType.TimeUp:
                    (GameMode1.Instance as GameMode1).IncreasePlayerChaseTime(5f);
                    break;
            }
        }

        void ReportPickedUp()
        {
            switch (type)
            {
                case CustomDroneType.Medical:
                    MedicalSpawner.Instance.ReportMedicalPicked();
                    break;
                case CustomDroneType.TimeUp:
                    TimeUpSpawner.Instance.ReportTimeUpPicked();
                    break;
            }
        }

        void Explode()
        {
            // Create particle system
            //var pos = animator.transform.position + Vector3.up * .5f;
            var pos = transform.position + Vector3.up * .5f;
            var ps = Instantiate(destroyParticlePrefab, pos, Quaternion.identity);


            foreach (var part in parts)
            {
                part.isKinematic = false;

                var smr = part.GetComponent<SkinnedMeshRenderer>();
                if (smr)
                {
                    smr.rootBone = null;
                    smr.bones = new Transform[0];
                }

                var dir = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 1f, UnityEngine.Random.Range(-0.5f, 0.5f));
                dir += PlayerController.Instance.transform.forward;
                part.AddForce(dir.normalized * UnityEngine.Random.Range(130f, 180f), ForceMode.Impulse);
                var torque = new Vector3(UnityEngine.Random.Range(-360f, 360f), UnityEngine.Random.Range(-360f, 360f), UnityEngine.Random.Range(-360f, 360f));
                part.AddTorque(torque);
            }
        }

        Transform GetNextWaypoint()
        {
            Transform ret = null;

            // Get all waypoints near to the agent
            var waypoints = LevelController.Instance.Waypoints.ToList().FindAll(w => Vector3.Distance(transform.position, w.position) < waypointMaxDistance);

            // Remove all the waypoints that are behind the agent
            var angle = 100f;

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
            else
                ret = LevelController.Instance.Waypoints[UnityEngine.Random.Range(0, LevelController.Instance.Waypoints.Count)];

            return ret;

        }

        bool DestinationReached()
        {
            if (agent.path == null) return false;

            if (Vector3.Distance(agent.destination, transform.position) > reachDistance)
                return false;

            return true;

        }

        public void ForceDestroy()
        {
            picked = true;

            Explode();
        }
    }
}