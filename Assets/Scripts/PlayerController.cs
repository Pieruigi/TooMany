using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public enum PlayerState { Prey, Hunter, Dead }

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
            get{ return pushForce; }
        }


        PlayerState state = PlayerState.Prey;

        CharacterController cc;

        float moveSpeed = 3;

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
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            cc.Move(move.normalized * moveSpeed * Time.deltaTime);

            if (transform.position.y > 0)
            {
                var pos = transform.position;
                pos.y = 0;
                transform.position = pos;
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
    }
    
}
