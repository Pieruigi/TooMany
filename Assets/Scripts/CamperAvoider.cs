using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace TMOT
{
    public class CamperAvoider : MonoBehaviour
    {
        Vector3 lastPlayerPos;

        float timer = 7;

        float range = 5;

        float elapsed = 0;

        bool botsCalled = false;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (PlayerController.Instance.State == PlayerState.Prey)
            {
                // Get current position
                var pos = PlayerController.Instance.transform.position;

                if (Vector3.Distance(lastPlayerPos, pos) < range)
                {
                    elapsed += Time.deltaTime;
                }
                else
                {
                    elapsed = 0; // Reset elapsed time
                    lastPlayerPos = pos; // Update the last player position
                    botsCalled = false;
                }

                if (elapsed > timer)
                {
                    if (botsCalled) return;

                    botsCalled = true;

                    Debug.Log($"TEST - Player is campering to {pos}");
                    // Get closer bots
                    var availables = MonsterSpawner.Instance.Monsters.Where(m => m.State == MonsterState.Patrolling).OrderBy(m => Vector3.Distance(m.transform.position, PlayerController.Instance.transform.position)).ToList();

                    // Rendomize number
                    if (availables.Count > 0)
                    {
                        int max = Mathf.Min(4, availables.Count);
                        int c = UnityEngine.Random.Range(0, max);

                        Debug.Log($"TEST - {c} bot(s) coming to player position");

                        for (int i = 0; i < c; i++)
                        {
                            //availables[i].ForcedDestination = PlayerController.Instance.transform.position;
                            //availables[i].SetState(MonsterState.Patrolling);
                            availables[i].GetComponent<NavMeshAgent>().SetDestination(PlayerController.Instance.transform.position);
                        }

                    }

                }

                
            }

        }

        void OnEnable()
        {
            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
        }

        void OnDisable()
        {
            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Prey:
                    lastPlayerPos = PlayerController.Instance.transform.position;
                    elapsed = 0;
                    botsCalled = false;
                    break;
            }
        }
    }
}