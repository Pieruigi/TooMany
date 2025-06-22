using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

namespace TMOT
{
    public enum MonsterState { Idle, Patrolling, Chasing, Searching, Fleeing, Attacking, Pushed }

    public class MonsterController : MonoBehaviour
    {

        public delegate void OnHitPlayerDelegate(MonsterController monsterController);
        public static OnHitPlayerDelegate OnHitPlayer;

        [SerializeField]
        float sightRange = 8f;

        [SerializeField]
        float sightAngle = 60f;

        [SerializeField]
        float proximityRange = 3f;

        [SerializeField]
        float idleTime = 5f;

        [SerializeField]
        float patrollingTime = 25;
        [SerializeField]
        float searchingTime = 5f;

        [SerializeField]
        float attackRange = 1.5f;

        [SerializeField]
        float attackAngle = 60f;

        [SerializeField]
        float damage = 30;

        Vector3 destination;

        float elapsed = 0;

        
        

        float time = 0;

        MonsterState state = MonsterState.Idle;
        public MonsterState State
        {
            get{ return state; }
        }

        NavMeshAgent agent;

        float destinationUpdateTime = .2f;
        float destinationUpdateElapsed = 0;

        float patrolMinDistance = 16f;

        float destinationReachedDistance = 1f;

        MonsterState previousState = MonsterState.Idle;

        Rigidbody rb;

        Vector3 lastPlayerSpot;

     


        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            SetState(UnityEngine.Random.Range(0, 2) == 0 ? MonsterState.Patrolling : MonsterState.Idle); 

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F))
                ReportPushedBack();
#endif
                switch (state)
                {
                    case MonsterState.Patrolling:
                        UpdatePatrollingState();
                        break;
                    case MonsterState.Searching:
                        UpdateSearchingState();
                        break;
                    case MonsterState.Idle:
                        UpdateIdleState();
                        break;
                    case MonsterState.Chasing:
                        UpdateChasingState();
                        break;
                }
        }



        #region entering state
        void EnterPatrollingState()
        {
            agent.isStopped = false;
            time = UnityEngine.Random.Range(patrollingTime*.7f, patrollingTime*1.3f);
            elapsed = 0;
            //onIdleExitNextState = MonsterState.Searching;
            agent.SetDestination(GetPatrolDestination());
        }

        void EnterChasingState()
        {
            agent.isStopped = false;
            destinationUpdateElapsed = 0;
            agent.SetDestination(PlayerController.Instance.transform.position);
        }


        void EnterSearchingState()
        {
            agent.isStopped = false;
            time = searchingTime;
            elapsed = 0;
            destinationUpdateElapsed = 0;
            agent.SetDestination(lastPlayerSpot);
        }

        void EnterFleeingState()
        {
            agent.isStopped = false;
            destinationUpdateElapsed = 0;
        }

        void EnterIdleState()
        {
            agent.ResetPath();
            agent.isStopped = true;
            time = UnityEngine.Random.Range(idleTime * .7f, idleTime * 1.3f);
            elapsed = 0;
        }

        

        async void EnterAttackingState()
        {
            OnHitPlayer?.Invoke(this);
            await Task.Delay(200);
            PlayerController.Instance.ApplyDamage(damage);
            // await Task.Delay(2000);
            // SetState(MonsterState.Chasing);
            // if (CanAttack(attackRange * 1.2f, attackAngle * 1.2f))
            // {

            //     OnHitPlayer?.Invoke(this);
            // }
            // else
            // {
            //     SetState(MonsterState.Chasing);
            // }    

        }

        async void EnterPushedState()
        {
            agent.isStopped = true;

            var dir = transform.position - PlayerController.Instance.transform.position;

            rb.isKinematic = false;
            rb.AddForce(dir.normalized * PlayerController.Instance.PushForce, ForceMode.Impulse);

            await Task.Delay(TimeSpan.FromSeconds(2));

            rb.isKinematic = true;
            agent.isStopped = false;

            SetState(MonsterState.Idle);
        }
        #endregion

        #region update state
        void UpdateIdleState()
        {
            if (PlayerController.Instance.State == PlayerState.Hunter)
            {
                // Switch to fleeing
                SetState(MonsterState.Fleeing);
                return;
            }

            if (HasSpottedPlayer())
            {
                SetState(MonsterState.Chasing);
                return;
            }

            elapsed += Time.deltaTime;
            if (elapsed > time)
            {
                elapsed = 0;
                // Switch to patrolling or searching
                SetState(MonsterState.Patrolling);
            }
            

        }

        
        void UpdatePatrollingState()
        {
            if (PlayerController.Instance.State == PlayerState.Hunter)
            {
                // Switch to fleeing
                SetState(MonsterState.Fleeing);
                return;
            }

            if (HasSpottedPlayer())
            {
                SetState(MonsterState.Chasing);
                return;
            }

            elapsed += Time.deltaTime;
            if (elapsed > time)
            {
                elapsed = 0;
                // Switch to searching state
                SetState(MonsterState.Idle);
                return;
            }

            // Keep patrolling
            if (agent.pathPending) return;

            if (!agent.hasPath || DestinationReached())
            {
                agent.SetDestination(GetPatrolDestination());
                
            }



        }
        void UpdateChasingState()
        {
            if (PlayerController.Instance.State == PlayerState.Hunter)
            {
                // Switch to fleeing
                SetState(MonsterState.Fleeing);
                return;
            }

            if (CanAttack(attackRange, attackAngle))
            {
                SetState(MonsterState.Attacking);
                return;
            }

            if (!HasSpottedPlayer())
            {
                // Set searching
                lastPlayerSpot = PlayerController.Instance.transform.position;
                SetState(MonsterState.Searching);
                return;
            }


            if (agent.pathPending) return;


            // Update destination
            destinationUpdateElapsed += Time.deltaTime;
            if (destinationUpdateElapsed > destinationUpdateTime)
            {
                destinationUpdateElapsed -= destinationUpdateTime;
                agent.SetDestination(PlayerController.Instance.transform.position);
            }

      
        }

        void UpdateSearchingState()
        {
            if (PlayerController.Instance.State == PlayerState.Hunter)
            {
                // Switch to fleeing
                SetState(MonsterState.Fleeing);
                return;
            }


            if (HasSpottedPlayer())
            {
                // Set searching
                SetState(MonsterState.Chasing);
                return;
            }


         
            if (DestinationReached())
            {
                SetState(MonsterState.Idle);
                return;
            }
                

            if (!agent.hasPath)
            {
                Debug.Log("TEST - No path");
                agent.SetDestination(lastPlayerSpot);
            }

            if (agent.pathPending) return;

            if (agent.pathStatus == NavMeshPathStatus.PathPartial || agent.pathStatus == NavMeshPathStatus.PathInvalid)
                agent.SetDestination(lastPlayerSpot);

        }
        #endregion


        bool DestinationReached()
        {
            if (agent.isStopped) return false;
            if (!agent.hasPath || agent.pathPending) return false;

            return Vector3.Distance(agent.destination, transform.position) < destinationReachedDistance;
        }

        Vector3 GetPatrolDestination()
        {
            // Get all waypoints far enough from the player
            var l = WayPointManager.Instance.WayPoints.ToList().FindAll(w =>
                                                    //Vector3.Distance(PlayerController.Instance.transform.position, w.position) > patrolDistanceFromPlayer && 
                                                    Vector3.Distance(transform.position, w.position) > patrolMinDistance);


            var ret = l[UnityEngine.Random.Range(0, l.Count)].position;
           
            return ret;
            
        }

        bool HasSpottedPlayer()
        {
            var dir = PlayerController.Instance.transform.position - transform.position;
            if (dir.magnitude > sightRange) return false;

            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 1.5f, dir.normalized, out hit, dir.magnitude, LayerMask.GetMask(new string[] { "Wall" })))
                return false; // There is a wall between monster and player

            if (dir.magnitude < proximityRange)
                return true;

            if (Vector3.Angle(dir.normalized, transform.forward) < sightAngle)
                return true;

            return false;
        }

        bool CanAttack(float attackRange, float attackAngle)
        {
            var dir = PlayerController.Instance.transform.position - transform.position;
            if (dir.magnitude > attackRange) return false;

            if (Vector3.Angle(dir.normalized, transform.forward) > attackAngle)
                return false;

            return true;
        }

        public void ReportPushedBack()
        {
            SetState(MonsterState.Pushed);
        }

        public void UpdateSearchingDestination(Vector3 newDestination)
        {
            lastPlayerSpot = newDestination;
            agent.SetDestination(lastPlayerSpot);
        }

        public void SetState(MonsterState newState)
        {
            if (state == newState) return;
            previousState = state;
            state = newState;
            switch (state)
            {
                case MonsterState.Patrolling:
                    EnterPatrollingState();
                    break;
                case MonsterState.Searching:
                    EnterSearchingState();
                    break;
                case MonsterState.Fleeing:
                    EnterFleeingState();
                    break;
                case MonsterState.Idle:
                    EnterIdleState();
                    break;
                case MonsterState.Chasing:
                    EnterChasingState();
                    break;
                case MonsterState.Attacking:
                    EnterAttackingState();
                    break;
                case MonsterState.Pushed:
                    EnterPushedState();
                    break;
            }
        }
    }
}