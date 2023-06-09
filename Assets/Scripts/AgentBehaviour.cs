using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AgentBehaviour : MonoBehaviour
{
    public float Range
    {
        get => nextDestinationRange;
        set => nextDestinationRange = value;
    }

    public float SearchDistance
    {
        get => searchDistance;
        set => searchDistance = value;
    }

    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    public float Acceleration
    {
        get => acceleration;
        set => acceleration = value;
    }

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float nextDestinationRange = 5f;
    [SerializeField] private float searchDistance = 5f;
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float agentsComfortZone = 3f;
    [SerializeField] private Vector3 nextDestination;
    [SerializeField] private bool walkBehavior;
    [SerializeField] private bool crowdBehavior;

    public float AgentsComfortZone
    {
        get => agentsComfortZone;
        set => agentsComfortZone = value;
    }

    

    [SerializeField] private float time;
    [SerializeField] private float timeDelay = 3f;

    public float TimeDelay
    {
        get => timeDelay;
        set => timeDelay = value;
    }

    public bool CrowdBehavior
    {
        get => crowdBehavior;
        set => crowdBehavior = value;
    }

    public bool ChaseBehavior
    {
        get => chaseBehavior;
        set => chaseBehavior = value;
    }

    public bool AvoidEnemyBehavior
    {
        get => avoidEnemyBehavior;
        set => avoidEnemyBehavior = value;
    }

    [SerializeField] private bool runFromEnemyBehavior;
    [SerializeField] private bool chaseBehavior;
    [SerializeField] private bool avoidEnemyBehavior;
    [SerializeField] private bool runWithOthersBehavior;

    public bool RunWithOthersBehavior
    {
        get => runWithOthersBehavior;
        set => runWithOthersBehavior = value;
    }

    public bool RunFromEnemyBehavior
    {
        get => runFromEnemyBehavior;
        set => runFromEnemyBehavior = value;
    }

    public bool WalkBehavior
    {
        get => walkBehavior;
        set => walkBehavior = value;
    }

    public Statuses MyStatus
    {
        get => myStatus;
        set => myStatus = value;
    }

    public enum Statuses
    {
        Walk,
        Run
    }

    [SerializeField] private Statuses myStatus = Statuses.Walk;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, searchDistance, transform.forward, 0f);
        time += 1f * Time.deltaTime;
        if (myStatus != Statuses.Run && crowdBehavior && time >= timeDelay)
        {
            time = 0f;
            myStatus = Statuses.Walk;
            agent.speed = speed;
            nextDestination = agentsSearch(hits);
            agent.SetDestination(nextDestination);
        }

        if (myStatus != Statuses.Run && chaseBehavior)
        {
            myStatus = Statuses.Run;
            agent.speed += acceleration;
            nextDestination = closerEnemySearch(hits);
            if (nextDestination != Vector3.zero) agent.SetDestination(nextDestination);
        }
        
        
        

        if (myStatus == Statuses.Walk && runWithOthersBehavior)
        {
            foreach (var hit in hits)
            {
                if (hit.transform.CompareTag("Agent") && transform.position != hit.transform.position &&
                    hit.transform.GetComponent<AgentBehaviour>().myStatus == Statuses.Run)
                {
                    myStatus = Statuses.Run;
                    agent.speed = speed + acceleration;
                    agent.SetDestination((transform.position - hit.transform.position) *
                                         (nextDestinationRange / 2f));
                    return;
                }
            }
        }

        if (runFromEnemyBehavior)
        {
            Vector3 enemyPos = enemySearch(hits);
            if (enemyPos != Vector3.zero)
            {
                time = 0f;
                myStatus = Statuses.Run;
                agent.speed = speed + acceleration;

                agent.SetDestination((transform.position - enemyPos) * (nextDestinationRange / 2));
            }
        }

        if (walkBehavior && agent.remainingDistance <= 1.5f)
        {
            myStatus = Statuses.Walk;
            agent.speed = speed;
            nextDestination = GetRandomPoint(transform.position + transform.forward * nextDestinationRange,
                nextDestinationRange);
            agent.SetDestination(nextDestination);
        }

        if (agent.remainingDistance <= 1.5f)
        {
            myStatus = Statuses.Walk;
            agent.speed = speed;
        }
    }


    private Vector3 GetRandomPoint(Vector3 position, float range)
    {
        Vector3 randomDirection = Random.insideUnitSphere * range + position;
        NavMesh.SamplePosition(randomDirection, out var navHit, range, NavMesh.AllAreas);
        return navHit.position;
    }

    private Vector3 agentsSearch(RaycastHit[] hits)
    {
        Vector3 average = Vector3.zero;
        
        float countAll = 0f;
        float countAround = 0;
        float distance2;
        foreach (var hit in hits)
        {
            if (hit.transform.CompareTag("Agent"))
            {
                distance2 = Vector3.Distance(hit.transform.position, transform.position);
                if (1f < distance2)
                {
                    average = hit.transform.position;
                    countAll++;
                }
                if (1f < distance2 && distance2 > agentsComfortZone * 1.5f)
                {
                    countAround++;
                }
            }
        }

        if (countAll == 0 || countAround > 2)
            return transform.position;
        return GetRandomPoint(average, nextDestinationRange);
    }

    private Vector3 enemySearch(RaycastHit[] hits)
    {
        Vector3 average = Vector3.zero;
        float count = 0f;
        foreach (var hit in hits)
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                average += hit.transform.position;
                count++;
            }
        }

        if (count == 0) return average;
        return average / count;
    }

    private Vector3 closerEnemySearch(RaycastHit[] hits)
    {
        Vector3 average = Vector3.zero;
        float distance = float.MaxValue;
        float count = 0f;
        float distance2;
        foreach (var hit in hits)
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                distance2 = Vector3.Distance(hit.transform.position, transform.position);
                if (1f < distance2 && distance2 < distance)
                {
                    average = hit.transform.position;
                    count++;
                }
            }
        }
        if (count == 0) return average;
        return average;
    }
}