using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
//[RequireComponent(typeof(Health))] // Asegura que tenga el script de vida universal
public class EnemyController : MonoBehaviour
{
    private Transform targetPlayer;
    private NavMeshAgent agent;
    private Animator animator;
    //private Health healthSystem;

    // Campos de configuración que ahora se llenarán dinámicamente
    private float currentSpeed;
    private float monsterDamage;
    
    [Header("Configuración de Ataque")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2.0f;
    [SerializeField] private float spawnDuration = 1.5f;
    [SerializeField] private float attackDuration = 0.6f;

    private bool isAttacking = false;
    private bool isSpawning = true;
    private float nextAttackTime = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        //healthSystem = GetComponent<Health>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Start()
    {
        LocatePlayer();
        StartCoroutine(SpawnSequence());
    }

    // ESTE MÉTODO CONECTA EL SCRIPTABLE OBJECT CON LA LÓGICA
    public void Initialize(MonsterDataSO data)
    {
        currentSpeed = data.speed;
        monsterDamage = data.damage;
        
        if (agent != null)
        {
            agent.speed = currentSpeed;
        }

        // Si tu script universal Health tiene un método público para setear la vida máxima:
        // healthSystem.SetMaxHealth(data.health);
    }

    private void Update()
    {
        if (isSpawning || isAttacking) return;

        if (targetPlayer == null)
        {
            LocatePlayer();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);

        if (distanceToPlayer <= attackRange)
        {
            if (Time.time >= nextAttackTime)
            {
                StartCoroutine(AttackSequence());
            }
            else
            {
                StopMovement();
            }
        }
        else
        {
            ChasePlayer();
        }
    }

    private IEnumerator SpawnSequence()
    {
        isSpawning = true;
        StopMovement();
        animator.SetTrigger("Spawn");
        yield return new WaitForSeconds(spawnDuration);
        isSpawning = false;
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;
        StopMovement();

        animator.SetTrigger("Attack");
        
        // Aquí aplicarías el monsterDamage al Player usando su script de Health
        
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }

    private void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(targetPlayer.position);
        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (agent.velocity.x != 0)
        {
            transform.localScale = new Vector3(-Mathf.Sign(agent.velocity.x), 1, 1);
        }
    }

    private void StopMovement()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        animator.SetFloat("Speed", 0f);
    }

    private void LocatePlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            targetPlayer = player.transform;
        }
    }
}