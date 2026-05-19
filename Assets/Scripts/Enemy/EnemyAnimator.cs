using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAnimator : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // agent.velocity.magnitude nos da un valor de 0 si está quieto, 
        // o mayor a 0 si se está moviendo. Se lo pasamos directo al Animator.
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }
}