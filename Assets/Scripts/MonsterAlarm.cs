using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMOT
{

    public class MonsterAlarm : Singleton<MonsterAlarm>
    {
        [SerializeField]
        float callOtherDistance = 24;

        float checkTime = 1f;
        float checkElapsed = 0;

        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void LateUpdate()
        {
            checkElapsed += Time.deltaTime;
            if (checkElapsed > checkTime)
            {
                checkElapsed -= checkTime;
                bool playerChased = MonsterSpawner.Instance.Monsters.ToList().Exists(m => m.State == MonsterState.Chasing);
                if (!playerChased) return;
                var others = MonsterSpawner.Instance.Monsters.ToList().FindAll(m => m.State == MonsterState.Idle || m.State == MonsterState.Patrolling || m.State == MonsterState.Searching);

                foreach (var other in others)
                {
                    other.SetState(MonsterState.Searching);
                    other.UpdateSearchingDestination(PlayerController.Instance.transform.position);
                }
                
            }



        }
    }
}