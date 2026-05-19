using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPathfinder : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 3.5f;
    
    private Transform targetPlayer;
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
        LocatePlayer();
    }

    private void Update()
    {
        if (targetPlayer == null)
        {
            LocatePlayer();
            return;
        }

        agent.SetDestination(targetPlayer.position);

        if (agent.velocity.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(agent.velocity.x), 1, 1);
        }
    }

    private void LocatePlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            targetPlayer = player.transform;
        }
    }

    public void ModifySpeed(float newSpeed)
    {
        agent.speed = newSpeed;
    }

    public void RestoreDefaultSpeed()
    {
        agent.speed = baseSpeed;
    }
}