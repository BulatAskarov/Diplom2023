using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class CrowdToolWindow : EditorWindow
{
    private float agentsComfortZone = 1.5f;
    private float spawnAreaDistance = 10f;
    private float agentsCount = 10f;
    private GameObject agent;
    private ArrayList agentsArrayList = new ArrayList();

    private float nextDestinationRange = 5f;
    private float searchDistance = 5f;
    private float speed = 3.5f;
    private float acceleration = 1f;
    private float timeDelay = 3f;

    private bool walkBehavior;
    private bool crowdBehavior;
    private bool runFromEnemyBehavior;
    private bool chaseBehavior;
    private bool avoidEnemyBehavior;
    private bool runWithOthersBehavior;

    [MenuItem("Window/CrowdTool")]
    public static void ShowWindow()
    {
        GetWindow<CrowdToolWindow>("CrowdTool");
    }

    void OnGUI()
    {
        GUILayout.Label("CrowdTool", EditorStyles.boldLabel);

        agent = (GameObject)EditorGUILayout.ObjectField("Agent", agent, typeof(GameObject), true);

        spawnAreaDistance = EditorGUILayout.FloatField("Spawn Area Distance", spawnAreaDistance);
        agentsCount = EditorGUILayout.FloatField("Agents Count", agentsCount);
        agentsComfortZone = EditorGUILayout.FloatField("Agent's Comfort Zone", agentsComfortZone);
        timeDelay = EditorGUILayout.FloatField("Check Time", timeDelay);

        nextDestinationRange = EditorGUILayout.FloatField("Next Destination Range", nextDestinationRange);
        searchDistance = EditorGUILayout.FloatField("Search Distance", searchDistance);
        speed = EditorGUILayout.FloatField("Speed", speed);
        acceleration = EditorGUILayout.FloatField("Acceleration", acceleration);

        GUILayout.Label("Agent Behaviors", EditorStyles.boldLabel);

        walkBehavior = EditorGUILayout.Toggle("Walk", walkBehavior);
        crowdBehavior = EditorGUILayout.Toggle("Crowd", crowdBehavior);
        runFromEnemyBehavior = EditorGUILayout.Toggle("Run From Enemy", runFromEnemyBehavior);
        if (runFromEnemyBehavior)
        {
            runWithOthersBehavior = EditorGUILayout.Toggle("Run With Others", runWithOthersBehavior);
            chaseBehavior = false;
        }

        if (!runFromEnemyBehavior)
        {
            chaseBehavior = EditorGUILayout.Toggle("Chase Enemy", chaseBehavior);
        }

        if (GUILayout.Button("Spawn"))
        {
            if (agent.GetComponent<AgentBehaviour>() is null)
            {
                agent.tag = "Agent";
                agent.AddComponent(typeof(AgentBehaviour));
                agent.GetComponent<AgentBehaviour>().Range = nextDestinationRange;
                agent.GetComponent<AgentBehaviour>().SearchDistance = searchDistance;
                agent.GetComponent<AgentBehaviour>().Speed = speed;
                agent.GetComponent<AgentBehaviour>().Acceleration = acceleration;
                agent.GetComponent<AgentBehaviour>().WalkBehavior = walkBehavior;
                agent.GetComponent<AgentBehaviour>().CrowdBehavior = crowdBehavior;
                agent.GetComponent<AgentBehaviour>().RunFromEnemyBehavior = runFromEnemyBehavior;
                agent.GetComponent<AgentBehaviour>().ChaseBehavior = chaseBehavior;
                agent.GetComponent<AgentBehaviour>().AvoidEnemyBehavior = avoidEnemyBehavior;
                agent.GetComponent<AgentBehaviour>().RunWithOthersBehavior = runWithOthersBehavior;
                agent.GetComponent<AgentBehaviour>().TimeDelay = timeDelay;
                agent.GetComponent<AgentBehaviour>().AgentsComfortZone = agentsComfortZone;
                agent.AddComponent<NavMeshAgent>();
                agent.GetComponent<NavMeshAgent>().radius = agentsComfortZone;
            }
            else
            {
                agent.tag = "Agent";
                agent.tag = "Agent";
                agent.GetComponent<AgentBehaviour>().Range = nextDestinationRange;
                agent.GetComponent<AgentBehaviour>().SearchDistance = searchDistance;
                agent.GetComponent<AgentBehaviour>().Speed = speed;
                agent.GetComponent<AgentBehaviour>().Acceleration = acceleration;
                agent.GetComponent<AgentBehaviour>().WalkBehavior = walkBehavior;
                agent.GetComponent<AgentBehaviour>().CrowdBehavior = crowdBehavior;
                agent.GetComponent<AgentBehaviour>().RunFromEnemyBehavior = runFromEnemyBehavior;
                agent.GetComponent<AgentBehaviour>().ChaseBehavior = chaseBehavior;
                agent.GetComponent<AgentBehaviour>().AvoidEnemyBehavior = avoidEnemyBehavior;
                agent.GetComponent<AgentBehaviour>().RunWithOthersBehavior = runWithOthersBehavior;
                agent.GetComponent<AgentBehaviour>().TimeDelay = timeDelay;
                agent.GetComponent<AgentBehaviour>().AgentsComfortZone = agentsComfortZone;
                agent.GetComponent<NavMeshAgent>().radius = agentsComfortZone;
            }

            Spawn();
        }


        if (GUILayout.Button("Clear"))
        {
            DestroyImmediate(agent.GetComponent<AgentBehaviour>(), true);
            DestroyImmediate(agent.GetComponent<NavMeshAgent>(), true);
            agent.tag = "Untagged";
            Clear();
        }
    }

    void Spawn()
    {
        for (var i = 0; i < agentsCount; i++)
        {
            Instantiate(agent, RandomPoint(spawnAreaDistance), Quaternion.identity);
        }
    }

    Vector3 RandomPoint(float distance)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
        randomDirection.y = 1f;
        return randomDirection;
    }

    void Clear()
    {
        while (GameObject.Find(agent.name + "(Clone)"))
        {
            DestroyImmediate(GameObject.Find(agent.name + "(Clone)"));
        }
    }
}