using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour
{
    public Vector2 target;
    public bool moving;
    NavMeshAgent agent;
    public bool showDebug;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Start()
    {
        
    }

    void Update()
    {
     

        if (moving)
        {
            UpdateAgentPosition();
        }
    }

    void OnDrawGizmos()
    {
        if (showDebug)
        {
            DebugExtension.DrawPoint(new Vector3(target.x, target.y, -15), 1.0f);
        }
    }


    void UpdateAgentPosition()
    {
        agent.SetDestination(new Vector3(target.x, target.y, transform.position.z));
    }
}
