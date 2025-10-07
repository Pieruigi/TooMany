using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

namespace TMOT
{
    public enum MonsterState { None, Idle, Patrolling, Chasing, Searching, Fleeing, Attacking, Pushed, Dying }

    public class MonsterController : MonoBehaviour
    {

        public delegate void HitPlayerDelegate(MonsterController monsterController);
        public static HitPlayerDelegate OnHitPlayer;

        public delegate void ExplodedDelegate(MonsterController monsterController);
        public static ExplodedDelegate OnExploded;

        public delegate void StateChangedDelegate(MonsterController monsterController, MonsterState oldState, MonsterState newState);
        public static StateChangedDelegate OnStateChanged;

        public delegate void ForcedBehaviourDelegate(MonsterController monsterController);
        public static ForcedBehaviourDelegate OnForcedBehaviour;

        public delegate void HitByPlayerDelegate(MonsterController monsterController);
        public static HitByPlayerDelegate OnHitByPlayer;


        static Dictionary<Transform, DateTime> lastPatrolPoints = new Dictionary<Transform, DateTime>();


        //[SerializeField]
        float killerSpeed = 4f;

        //[SerializeField]
        float killerAcc = 12f;

        //[SerializeField]
        float killerAngSpeed = 720f;

        //[SerializeField]
        float preySpeed = 2.5f;

        //[SerializeField]
        float preyAcc = 7.5f;

        //[SerializeField]
        float preyAngSpeed = 450;

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
        float damage = 1;

        /// <summary>
        /// Inverted behaviour means blue bot.
        /// </summary>
        [SerializeField]
        bool invertedBehaviour = false;
        public bool InvertedBehaviour
        {
            get { return invertedBehaviour; }
            set { invertedBehaviour = value; }
        }

        [SerializeField]
        List<Rigidbody> parts;

        [SerializeField]
        ParticleSystem destroyParticlePrefab;

        Vector3 destination;

        float elapsed = 0;

        float time = 0;

        MonsterState state = MonsterState.None;
        public MonsterState State
        {
            get{ return state; }
        }

        NavMeshAgent agent;

        float destinationUpdateTime = .2f;
        float destinationUpdateElapsed = 0;

        float patrolMinDistance = 4f;

        float patrolMaxDistance = 12f;

        float destinationReachedDistance = 1f;

        float escapeDistance = 5f;

        MonsterState previousState = MonsterState.None;

        Rigidbody rb;

        Vector3 lastPlayerSpot;

        float keepFleeingTime = 5;
        float keepFleeingElapsed = 0;

        float hunterScale = 1;
        float preyScale = .6f;

        [SerializeField]
        Animator animator;

        float speedRandomMul = 1f;


        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
            patrolMaxDistance = LevelController.Instance.MapSize.x / 3f;
            patrolMinDistance = patrolMaxDistance / 3f;

            // killerSpeed *= StageManager.GetRedBotMul(GameManager.Instance.GameStage);
            // killerAcc *= StageManager.GetRedBotMul(GameManager.Instance.GameStage);
            // killerAngSpeed *= StageManager.GetRedBotMul(GameManager.Instance.GameStage);

            // preySpeed *= StageManager.GetBlueBotMul(GameManager.Instance.GameStage);
            // preyAcc *= StageManager.GetBlueBotMul(GameManager.Instance.GameStage);
            // preyAngSpeed *= StageManager.GetBlueBotMul(GameManager.Instance.GameStage);

            //speedRandomMul = UnityEngine.Random.Range(.9f, 1.1f);
            speedRandomMul = 1;
            //agent.speed = killerSpeed * speedRandomMul;
            SetRedStats();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (GameManager.Instance.GameState == GameState.Playing)
                SetState(UnityEngine.Random.Range(0, 2) == 0 ? MonsterState.Patrolling : MonsterState.Idle);

            UpdateScale();
            UpdateSpeed();
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F))
                SetState(MonsterState.Dying);
                //ReportPushedBack();

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
                    case MonsterState.Fleeing:
                        UpdateFleeingState();
                        break;
                }

            UpdateAnimations();
        }

        void OnEnable()
        {
            GameManager.OnStateChanged += HandleOnGameStateChanged;
            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
        }

        void OnDisable()
        {
            GameManager.OnStateChanged -= HandleOnGameStateChanged;
            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
        }

        void SetRedStats()
        {
            agent.speed = killerSpeed;
            agent.acceleration = killerAcc;
            agent.angularSpeed = killerAngSpeed;
        }

        void SetBlueStats()
        {
            agent.speed = preySpeed;
            agent.acceleration = preyAcc;
            agent.angularSpeed = preyAngSpeed;
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Hunter:
                case PlayerState.Prey:
                    UpdateScale();
                    UpdateSpeed();
                    break;
                case PlayerState.Dead:
                    SetState(MonsterState.Patrolling);
                    break;
            }
        }

        private void HandleOnGameStateChanged(GameState oldState, GameState newState)
        {
            switch (newState)
            {
                case GameState.Starting:
                    SetState(MonsterState.None);
                    break;
                case GameState.Playing:
                    SetState(UnityEngine.Random.Range(0, 2) == 0 ? MonsterState.Patrolling : MonsterState.Idle); 
                    break;

            }
        }



        void UpdateAnimations()
        {
            if (animator.IsInTransition(0)) return;
            if (state == MonsterState.Chasing && !animator.GetCurrentAnimatorStateInfo(0).IsName("Chase"))
            {
                animator.SetFloat("Offset", UnityEngine.Random.Range(0f, 1f));
                animator.SetTrigger("Chase");
                return;
            }
            if (state == MonsterState.Patrolling && !animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                animator.SetFloat("Offset", UnityEngine.Random.Range(0f, 1f));
                animator.SetTrigger("Walk");
                return;
            }
            if (state == MonsterState.Idle && !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                animator.SetFloat("Offset", UnityEngine.Random.Range(0f, 1f));
                animator.SetTrigger("Idle");
                return;
            }
            if (state == MonsterState.Fleeing && !animator.GetCurrentAnimatorStateInfo(0).IsName("Flee"))
            {
                animator.SetFloat("Offset", UnityEngine.Random.Range(0f, 1f));
                animator.SetTrigger("Flee");
                return;
            }
        }

        void UpdateSpeed()
        {
            if (PlayerController.Instance.State == PlayerState.Hunter) // Player is red
            {
                if (!invertedBehaviour)
                    SetBlueStats();//agent.speed = preySpeed * speedRandomMul;
                else
                    SetRedStats();//agent.speed = killerSpeed * speedRandomMul;
            }
            else // Player is blue
            {
                if (!invertedBehaviour)
                    SetRedStats();// agent.speed = killerSpeed * speedRandomMul;
                else
                    SetBlueStats();// agent.speed = preySpeed * speedRandomMul;
            }
                

        }

        void UpdateScale() {
            float scaleTime = 1f;
            if (PlayerController.Instance.State == PlayerState.Hunter) // Player is red
            {
                if (!invertedBehaviour)
                    transform.DOScale(preyScale, scaleTime).SetEase(Ease.InOutElastic); //StartCoroutine(ScaleMonster(preyScale));
                else
                    transform.DOScale(hunterScale, scaleTime).SetEase(Ease.InOutElastic);//StartCoroutine(ScaleMonster(hunterScale));    

            }
            else // Player is blue
            {
                if (!invertedBehaviour)
                    transform.DOScale(hunterScale, scaleTime).SetEase(Ease.InOutElastic);//StartCoroutine(ScaleMonster(hunterScale));   
                else
                    transform.DOScale(preyScale, scaleTime).SetEase(Ease.InOutElastic); //StartCoroutine(ScaleMonster(preyScale));
            }
                
                
        }

      

        void Explode()
        {
            // Create particle system
            var pos = animator.transform.position + Vector3.up * .5f;
            var ps = Instantiate(destroyParticlePrefab, pos, Quaternion.identity);


            foreach (var part in parts)
            {
                part.isKinematic = false;
                var smr = part.GetComponent<SkinnedMeshRenderer>();
                smr.rootBone = null;
                smr.bones = new Transform[0];
                var dir = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 1f, UnityEngine.Random.Range(-0.5f, 0.5f));
                dir += PlayerController.Instance.transform.forward;
                part.AddForce(dir.normalized * UnityEngine.Random.Range(130f, 180f), ForceMode.Impulse);
                var torque = new Vector3(UnityEngine.Random.Range(-360f, 360f), UnityEngine.Random.Range(-360f, 360f), UnityEngine.Random.Range(-360f, 360f));
                part.AddTorque(torque);
            }

            OnExploded?.Invoke(this);
        }

        #region entering state
        void EnterPatrollingState()
        {
            agent.isStopped = false;
            time = UnityEngine.Random.Range(patrollingTime * .7f, patrollingTime * 1.3f);
            elapsed = 0;
            //onIdleExitNextState = MonsterState.Searching;
            // if (ForcedDestination.HasValue)
            // {
            //     agent.SetDestination(ForcedDestination.Value);
            //     ForcedDestination = null;
            // }
            // else
            agent.SetDestination(GetPatrolDestination());
            
            //animator.SetTrigger("Walk");
        }

        void EnterChasingState()
        {
            
            agent.isStopped = false;
            destinationUpdateElapsed = 0;
            agent.SetDestination(PlayerController.Instance.transform.position);

            //animator.SetTrigger("Chase");
        }


        void EnterSearchingState()
        {
            agent.isStopped = false;
            time = searchingTime;
            elapsed = 0;
            destinationUpdateElapsed = 0;
            agent.SetDestination(lastPlayerSpot);

            //animator.SetTrigger("Walk");
        }

        void EnterFleeingState()
        {
            agent.isStopped = false;
            destinationUpdateElapsed = 0;

            agent.SetDestination(GetEscapeDestination());

            //animator.SetTrigger("Flee");
           
        }

        void EnterIdleState()
        {
            agent.ResetPath();
            agent.isStopped = true;
            time = UnityEngine.Random.Range(idleTime * .7f, idleTime * 1.3f);
            elapsed = 0;

            //animator.SetTrigger("Idle");
        }

        void EnterDyingState()
        {
            agent.ResetPath();
            agent.isStopped = true;

            GetComponent<Collider>().enabled = false;


            Explode();
            MonsterSpawner.Instance.DestroyMonsterDelayed(this, 3f);



        }


        async UniTaskVoid EnterAttackingState()
        {
            OnHitPlayer?.Invoke(this);
            await UniTask.Delay(200);
            PlayerController.Instance.ApplyDamage(damage);
           
        }

        async UniTaskVoid EnterPushedState()
        {
            agent.isStopped = true;

            var dir = transform.position - PlayerController.Instance.transform.position;

            rb.isKinematic = false;
            rb.AddForce(dir.normalized * PlayerController.Instance.PushForce, ForceMode.Impulse);

            await UniTask.Delay(TimeSpan.FromSeconds(2));

            rb.isKinematic = true;
            agent.isStopped = false;

            SetState(MonsterState.Idle);


        }
        #endregion

        #region update state
        void UpdateFleeingState()
        {
            if ((PlayerController.Instance.State == PlayerState.Prey && !invertedBehaviour) || (PlayerController.Instance.State == PlayerState.Hunter && invertedBehaviour))
            {
                // Stop fleeing
                SetState(UnityEngine.Random.Range(0, 2) == 0 ? MonsterState.Patrolling : MonsterState.Idle);
 
                return;
            }

            if (!HasSpottedPlayer())
            {
                keepFleeingElapsed += Time.deltaTime;
                if(keepFleeingElapsed > keepFleeingTime)
                    SetState(UnityEngine.Random.Range(0, 2) == 0 ? MonsterState.Patrolling : MonsterState.Idle);
                return;
            }
            else
            {
                keepFleeingElapsed = 0;
            }

            if (!agent.hasPath || DestinationReached())
            {
                agent.SetDestination(GetEscapeDestination());
            }

            if (agent.pathPending) return;
          

            // // Update destination
            destinationUpdateElapsed += Time.deltaTime;
            if (destinationUpdateElapsed > destinationUpdateTime)
            {
                destinationUpdateElapsed -= destinationUpdateTime;

                if (!IsOptimalEscapeDestination(agent.destination))
                {
                    var d = GetEscapeDestination();
                    if (IsOptimalEscapeDestination(d))
                        agent.SetDestination(d);
                }

                
            }
        }

        void UpdateIdleState()
        {
            if (HasSpottedPlayer())
            {
                if((PlayerController.Instance.State == PlayerState.Prey && !invertedBehaviour) || (PlayerController.Instance.State == PlayerState.Hunter && invertedBehaviour))
                    SetState(MonsterState.Chasing);
                else
                    SetState(MonsterState.Fleeing);
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
            // if (PlayerController.Instance.State == PlayerState.Hunter)
            // {
            //     // Switch to fleeing
            //     SetState(MonsterState.Fleeing);
            //     return;
            // }

            
            if (HasSpottedPlayer())
            {
                if((PlayerController.Instance.State == PlayerState.Prey && !invertedBehaviour) || (PlayerController.Instance.State == PlayerState.Hunter && invertedBehaviour))
                    SetState(MonsterState.Chasing);
                else
                    SetState(MonsterState.Fleeing);
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
            if ((PlayerController.Instance.State == PlayerState.Hunter && !invertedBehaviour)||(PlayerController.Instance.State == PlayerState.Prey && invertedBehaviour))
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
            if ((PlayerController.Instance.State == PlayerState.Hunter && !invertedBehaviour) || (PlayerController.Instance.State == PlayerState.Prey && invertedBehaviour))
            {
                // Switch to fleeing
                SetState(UnityEngine.Random.Range(0, 2) == 0 ? MonsterState.Patrolling : MonsterState.Idle); 
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


        Vector3 GetEscapeDestination()
        {
            // Get points far enough from this agent
            var l = LevelController.Instance.Waypoints.ToList().FindAll(w => Vector3.Distance(transform.position, w.position) > escapeDistance);

            // Filter for valid destinations
            var ldot = l.FindAll(w => IsOptimalEscapeDestination(w.transform.position));
            //var ldot = l.FindAll(w => Vector3.Dot(w.transform.position - PlayerController.Instance.transform.position, w.transform.position - transform.position) > 0);

            if (ldot.Count == 0)
                ldot = l;

            return ldot[UnityEngine.Random.Range(0, ldot.Count)].position;

        }

        bool IsOptimalEscapeDestination(Vector3 destination)
        {
            return Vector3.Dot(transform.position - PlayerController.Instance.transform.position, destination - transform.position) > 0;
        }

        Vector3 GetPatrolDestination()
        {
            // Clear old patrol points stored in the last patrol list
            List<Transform> toRemove = new List<Transform>();
            foreach (var p in lastPatrolPoints)
            {
                if ((DateTime.Now - p.Value).TotalSeconds > 10)
                    toRemove.Add(p.Key);
            }

            foreach (var t in toRemove)
            {
                lastPatrolPoints.Remove(t);
            }

            // Get all waypoints far enough from the player
            var l = LevelController.Instance.Waypoints.ToList().FindAll(w =>
            {
                float dist = Vector3.Distance(transform.position, w.position);

                return dist > patrolMinDistance && dist < patrolMaxDistance && !lastPatrolPoints.ContainsKey(w);
            });

            // Most of the time l won't be empty
            if (l.Count == 0)
            {
                LevelController.Instance.Waypoints.ToList().FindAll(w =>
                {
                    float dist = Vector3.Distance(transform.position, w.position);

                    return dist > patrolMinDistance && dist < patrolMaxDistance;
                });
            }

            var ret = l[UnityEngine.Random.Range(0, l.Count)];
            
            // Add the new waypoint to the patrol list
            lastPatrolPoints.Add(ret, DateTime.Now);

            return ret.position;
            
        }



        bool HasSpottedPlayer()
        {
            if (PlayerController.Instance.State == PlayerState.Dead) return false;

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
            if (PlayerController.Instance.State == PlayerState.Dead) return false;

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

        public void ReportHitByPlayer()
        {
            GameMode.Instance.ReportMonsterDroneHitByPlayer(this);
            SetState(MonsterState.Dying);

            OnHitByPlayer?.Invoke(this);
        }

        /// <summary>
        /// This method is only called when player is already in hunter state and we want to switch a specific monster; ex. in game mode 3 
        /// </summary>
        public void ForceToPrey()
        {
            if (!invertedBehaviour) return;
            invertedBehaviour = false;

            UpdateScale();
            UpdateSpeed();

            OnForcedBehaviour?.Invoke(this);
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
                    EnterAttackingState().Forget();
                    break;
                case MonsterState.Pushed:
                    EnterPushedState().Forget();
                    break;
                case MonsterState.Dying:
                    EnterDyingState();
                    break;
            }

            OnStateChanged?.Invoke(this, previousState, newState);
        }
    }
}