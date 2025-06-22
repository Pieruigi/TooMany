using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestAgent : MonoBehaviour
{
    [SerializeField]
    Transform target;

    float time = 0.2f;
    float elapsed = 0;

    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!target) return;
        agent.SetDestination(target.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!target) return;
        elapsed += Time.deltaTime;
        if (elapsed < time) return;
        elapsed = 0;

        agent.SetDestination(target.position);
        
    }
}
