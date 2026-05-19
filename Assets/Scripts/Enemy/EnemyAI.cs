using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 3.5f;
    
    private Transform playerTransform;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = baseSpeed;
    }

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            FindPlayer();
            return;
        }

        agent.SetDestination(playerTransform.position);

        if (agent.velocity.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(agent.velocity.x), 1, 1);
        }
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    public void SetSpeed(float newSpeed)
    {
        agent.speed = newSpeed;
    }

    public void ResetSpeed()
    {
        agent.speed = baseSpeed;
    }
}