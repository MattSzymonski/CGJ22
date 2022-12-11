using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour
{
    public Vector2 target, currentTarget;
    public bool moving;
    NavMeshAgent agent;
    public bool showDebug;

    // Movement Direction jitter
    [Header("Movement Direction Jitter")]
    public float changeDirectionMin = 0.3f;
    public float changeDirectionMax = 1.5f;

    public float minDistance = 1.0f;
    public float maxDistance = 5.0f;

    public float directionFanAngle = 45f;

    public int moveForwardsProb = 50;
    public int moveForwardsJitter = 75;
    //public int moveRandomDirection = 100;

    Mighty.MightyTimer changeDirectionTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Start()
    {
        float randTime = Random.Range(changeDirectionMin, changeDirectionMax);
        changeDirectionTimer = Mighty.MightyTimersManager.Instance.CreateTimer("ChangeDirectionTimer", randTime, 1f, false, false);
    }

    void Update()
    {
        if (MainGameManager.Instance.notPlaying)
        {
            agent.velocity = Vector2.zero;
            return;
        }

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
            DebugExtension.DrawArrow(new Vector3(transform.position.x, transform.position.y, 0), new Vector3(currentTarget.x, currentTarget.y, 0), Color.red);
        }
    }


    void UpdateAgentPosition()
    {
        if (!changeDirectionTimer.finished)
            return;
        
        // maybe add a minimal delay
        Mighty.MightyTimersManager.Instance.RemoveTimer(changeDirectionTimer);
        float randTime = Random.Range(changeDirectionMin, changeDirectionMax);
        changeDirectionTimer = Mighty.MightyTimersManager.Instance.CreateTimer("ChangeDirectionTimer", randTime, 1f, false, false);

        int prob = Random.Range(0, 100);
        if (GetComponent<Enemy>()) // randomize walk towards the target
        {
            if (prob < moveForwardsProb)
            {
                currentTarget = target;
            } 
            else if (prob < moveForwardsJitter)
            {
                // find a direction in a fan
                float randomAngle = Random.Range(-directionFanAngle, directionFanAngle);
                Vector3 randomDir = Quaternion.AngleAxis(randomAngle, Vector3.forward) * agent.velocity.normalized;
                randomDir += transform.position;

                float randomDistance = Random.Range(minDistance, maxDistance);

                NavMesh.SamplePosition(randomDir, out NavMeshHit navHit, randomDistance, -1);

                currentTarget = navHit.position;
            } 
            else // else go in a random direciton totally (check bounds!)
            {
                float randomDistance = Random.Range(minDistance, maxDistance);
                Vector2 randomDir = Random.insideUnitCircle * randomDistance;

                randomDir += Mighty.MightyUtilites.Vec3ToVec2(transform.position);

                NavMesh.SamplePosition(randomDir, out NavMeshHit navHit, randomDistance, -1);

                currentTarget = navHit.position;
            }
            agent.SetDestination(new Vector3(currentTarget.x, currentTarget.y, transform.position.z));

        } else if (GetComponent<Copernicus>()) // less jitter for Big C  - TODO: for now no jitter introduced
        {

            agent.SetDestination(new Vector3(currentTarget.x, currentTarget.y, transform.position.z));
        }

        if (agent.velocity.x < 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = true;
        }
        else
        {
            GetComponentInChildren<SpriteRenderer>().flipX = false;
        }
    }

    public void SetTarget(Vector3 t)
    {
        target = t;
        currentTarget = target;
    }
}
