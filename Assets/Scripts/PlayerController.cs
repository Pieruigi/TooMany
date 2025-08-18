using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace TMOT
{
    public enum PlayerState { None, Prey, Hunter, Dead }

    public class PlayerController : Singleton<PlayerController>
    {
        public delegate void StateChangedDelegate(PlayerState oldState, PlayerState newState);
        public static StateChangedDelegate OnStateChanged;

        public delegate void PlayerDamagedDelegate(float previousHealth, float currentHealth);
        public static PlayerDamagedDelegate OnPlayerDamaged;

        public delegate void PlayerHealedDelegate(float previousHealth, float currentHealth);
        public static PlayerHealedDelegate OnPlayerHealed;


        public static UnityAction OnPunch;


        [SerializeField]
        float health = 4;

        public float MaxHealth { get; private set; }
        public float Health {get{ return health; }}
       
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



        [SerializeField]
        float moveSpeed = 3;

        [SerializeField]
        float rotationSpeed = 720;

        [SerializeField]
        float turnSpeed = 1;

        float yaw = 0, pitch = 0;
        public float Pitch
        {
            get{ return pitch; }
        }

        float mouseSensitivity = 5f;

        Vector2 moveInput;
        Vector2 aimInput;

        public bool Rotating
        {
            get {return aimInput.x != 0; }
        }

        float pitchDirection = -1;

        float minPitch = -80;
        float maxPitch = 80;

        Vector3 currentVelocity = Vector3.zero;
        public Vector3 Velocity
        {
            get{ return currentVelocity; }
        }

        float killMonsterTime = .5f;

        float killMonsterElapsed = 0f;

        
        PlayerState state = PlayerState.None;

        CharacterController cc;

       

        [SerializeField]
        float sprintMultiplier = 2f;
        public float SprintMultiplier
        {
            get{ return sprintMultiplier; }
        }

        float stamina = 1;
        public float Stamina
        {
            get{ return stamina; }
        }

        float staminaDepleteSpeed = 1f;
        float staminaChargeDelay = 1.5f;
        float staminaChargeSpeed = .25f;

        bool sprinting = false;
        public bool Sprinting
        {
            get{ return sprinting; }
        }
        float staminaLastUsed = 0;

        public bool InputDisabled
        {
            get; set;
        }


        public PlayerState State
        {
            get { return state; }
        }

        protected override void Awake()
        {
            base.Awake();
            MaxHealth = health;
            cc = GetComponent<CharacterController>();
            yaw = transform.eulerAngles.y;
        }


        // Start is called before the first frame update
        void Start()
        {
            mouseSensitivity = OptionsManager.Instance.MouseSpeed;
            Debug.Log($"TEST - MouseSens:{mouseSensitivity}");
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR

            if (Input.GetKeyDown(KeyCode.P))
            {
                
                InputDisabled = !InputDisabled;
            }

            if (Input.GetKeyDown(KeyCode.H))
                ApplyDamage(1);
            if (Input.GetKeyDown(KeyCode.J))
                Heal();
#endif




                switch (state)
                {
                    case PlayerState.None:

                        break;
                    case PlayerState.Prey:
                        UpdatePreyState();
                        break;
                    case PlayerState.Hunter:
                        UpdateHunterState();
                        break;
                }

            
        }

        void CheckInput()
        {
            if (InputDisabled) return;

            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            aimInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            var s = Input.GetKey(KeyCode.LeftShift) && stamina > 0;
            if (s)
            {
                stamina -= Time.deltaTime * staminaDepleteSpeed;
                if (stamina < 0) stamina = 0;
                staminaLastUsed = 0;
            }

            sprinting = s;
        }

        // void Move()
        // {
        //     Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //     cc.Move(move.normalized * moveSpeed * Time.deltaTime);

        //     if (transform.position.y > 0)
        //     {
        //         var pos = transform.position;
        //         pos.y = 0;
        //         transform.position = pos;
        //     }
        // }

        private void Move()
        {
            var targetDirection = transform.TransformDirection(new Vector3(moveInput.x, 0, moveInput.y));
            var targetVelocity = targetDirection.normalized * moveSpeed * (sprinting ? sprintMultiplier : 1f);


            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, turnSpeed * Time.deltaTime);

            // var newPosition = transform.position + currentVelocity * Time.deltaTime;
            // newPosition.y = 0;
            //transform.position = newPosition;
            cc.Move(currentVelocity * Time.deltaTime);

            // Be sure player is grounded
            var pos = transform.position;
            if (pos.y != 0)
            {
                pos.y = 0;
                transform.position = pos;
            }
        }

        private void Rotate()
        {
            yaw += aimInput.x /** Time.deltaTime */* rotationSpeed * mouseSensitivity * 0.001f;
            yaw %= 360;

            pitch += aimInput.y /** Time.deltaTime */* rotationSpeed * mouseSensitivity * 0.001f * pitchDirection;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);


            transform.eulerAngles = new Vector3(0, yaw, 0);


   
        }

        #region update state
        void UpdatePreyState()
        {
            CheckInput();
            UpdateStamina();
            Rotate();
            Move();
        }

        void UpdateHunterState()
        {
            CheckInput();
            UpdateStamina();
            Rotate();
            Move();
            KillMonsters();
        }

        void UpdateStamina()
        {
            if (stamina == 1) return;

            if (sprinting) return;

            if (staminaLastUsed < staminaChargeDelay)
            {
                staminaLastUsed += Time.deltaTime;
                return;
            }

            // Recharge
            stamina += Time.deltaTime * staminaChargeSpeed;

            if (stamina > 1)
                stamina = 1;


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

        async UniTaskVoid EnterDeadState()
        {
            InputDisabled = true;

            await UniTask.Delay(TimeSpan.FromSeconds(.5f));

            CameraShake.Instance.Die().Forget();
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

                if (colls == null || colls.Length == 0) return;

                bool atLeastOneKilled = false;

                foreach (var coll in colls)
                {

                    if (!coll.CompareTag("Monster")) continue;

                    coll.GetComponent<MonsterController>().ReportHitByPlayer();

                    CameraShake.Instance.Shake(0.15f, 0.2f, 8, 60f);

                    atLeastOneKilled = true;
                }

                if(atLeastOneKilled)
                    OnPunch?.Invoke();
            }
        }

        public void ApplyDamage(float damage)
        {
            var oldHealth = health;
            health -= damage;
            if (health < 0) health = 0;
            if (health == 0)
            {
                Debug.Log("You are dead");
                SetState(PlayerState.Dead);
                GameManager.Instance.ReportPlayerIsLoser();
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

            CameraShake.Instance.Shake(0.4f, 0.5f, 15, 120f);

            OnPlayerDamaged?.Invoke(oldHealth, health);
        }

        public bool IsWounded()
        {
            return health < MaxHealth;
        }

        public void Heal()
        {
            Debug.Log("Healing...");
            if (health == MaxHealth) return;
            health++;
            if (health > MaxHealth) health = MaxHealth;

            OnPlayerHealed?.Invoke(health - 1, health);
        }

        public void SetState(PlayerState newState)
        {
            if (newState == state) return;
            var oldState = state;

            state = newState;
            switch (state)
            {
                case PlayerState.Prey:
                    EnterPreyState();
                    break;
                case PlayerState.Hunter:
                    EnterHunterState();
                    break;
                case PlayerState.Dead:
                    EnterDeadState().Forget();
                    break;
            }

            OnStateChanged?.Invoke(oldState, newState);

        }
    }
    
}
