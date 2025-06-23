using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TMOT
{
    public enum PlayerState { None, Prey, Hunter, Dead }

    public class PlayerController : Singleton<PlayerController>
    {
        [SerializeField]
        float health = 1000;

        [SerializeField]
        float pushRadius = 5f;

        [SerializeField]
        float pushForce = 10f;
        public float PushForce
        {
            get { return pushForce; }
        }

        [SerializeField]
        float monsterKillRange = 2f;

        PlayerState state = PlayerState.None;

        CharacterController cc;

        float moveSpeed = 3;

        float killMonsterTime = .5f;

        float killMonsterElapsed = 0f;

        



        public PlayerState State
        {
            get { return state; }
        }

        protected override void Awake()
        {
            base.Awake();

            cc = GetComponent<CharacterController>();
        }


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case PlayerState.Prey:
                    UpdatePreyState();
                    break;
                case PlayerState.Hunter:
                    UpdateHunterState();
                    break;
            }

            
        }

        void Move()
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            cc.Move(move.normalized * moveSpeed * Time.deltaTime);

            if (transform.position.y > 0)
            {
                var pos = transform.position;
                pos.y = 0;
                transform.position = pos;
            }
        }

        #region update state
        void UpdatePreyState()
        {
            Move();
        }

        void UpdateHunterState()
        {
            Move();

            KillMonsters();
        }

        #endregion

        #region enter state
        void EnterPreyState()
        {
            
        }

        void EnterHunterState()
        {
            killMonsterElapsed = 0;

        }

        #endregion

        void KillMonsters()
        {
            killMonsterElapsed += Time.deltaTime;
            if (killMonsterElapsed > killMonsterTime)
            {
                killMonsterElapsed -= killMonsterTime;
                // Overlapp sphere
                Collider[] colls = Physics.OverlapSphere(transform.position, monsterKillRange);
                Debug.Log($"TEST - Colls.Length:{colls.Length}");
                if (colls == null || colls.Length == 0) return;

                foreach (var coll in colls)
                {
                    Debug.Log($"TEST - Coll:{gameObject.name}");
                    if (!coll.CompareTag("Monster")) continue;

                    coll.GetComponent<MonsterController>().ReportHitByPlayer();
                    
                }
            }

            
        }

        public void ApplyDamage(float damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Debug.Log("You are dead");

            }
            else
            {
                Collider[] colls = Physics.OverlapSphere(transform.position, pushRadius);
                if (colls == null || colls.Length == 0) return;

                // Push away monsters
                foreach (var coll in colls)
                {
                    if (coll.CompareTag("Monster"))
                        coll.GetComponent<MonsterController>().ReportPushedBack();
                }

            }
        }

        public void SetState(PlayerState newState)
        {
            if (newState == state) return;

            state = newState;
            switch (state)
            {
                case PlayerState.Prey:
                    EnterPreyState();
                    break;
                case PlayerState.Hunter:
                    EnterHunterState();
                    break;
            }

        }
    }
    
}
