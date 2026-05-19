using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Rigidbody2D))] // Aseguramos que tenga Rigidbody
public class EnemyController : MonoBehaviour
{
    private Transform targetPlayer;
    private NavMeshAgent agent;
    private Animator animator;
    private FearManager fearManager;
    private Rigidbody2D rb;

    private float currentSpeed;
    private float monsterDamage;
    
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2.0f;
    [SerializeField] private float spawnDuration = 1.5f;
    [SerializeField] private float attackDuration = 0.6f;
    [SerializeField] private int fearDealtOnHit = 15;

    private bool isAttacking = false;
    private bool isSpawning = true;
    private float nextAttackTime = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        fearManager = FindObjectOfType<FearManager>();
        rb = GetComponent<Rigidbody2D>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
        // LA MAGIA: Le quitamos al agente el control directo del movimiento
        agent.updatePosition = false; 
    }

    private void Start()
    {
        LocatePlayer();
        StartCoroutine(SpawnSequence());
    }

    public void Initialize(MonsterDataSO data)
    {
        currentSpeed = data.speed;
        monsterDamage = data.damage;
        if (agent != null) agent.speed = currentSpeed;
    }

    private void Update()
    {
        if (targetPlayer == null) LocatePlayer();

        // La lógica de estado (cuándo atacar o nacer) sigue aquí
        if (!isSpawning && !isAttacking && targetPlayer != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);
            
            if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
            {
                StartCoroutine(AttackSequence());
            }
        }
    }

    private void FixedUpdate()
    {
        // Las físicas y el movimiento se aplican aquí
        if (isSpawning || isAttacking || targetPlayer == null)
        {
            StopMovementFísico();
            return;
        }

        float distanceToPlayer = Vector2.Distance(rb.position, targetPlayer.position);

        if (distanceToPlayer > attackRange)
        {
            ChasePlayerPhysics();
        }
        else
        {
            StopMovementFísico();
        }
    }

    private void ChasePlayerPhysics()
    {
        agent.isStopped = false;
        // 1. Le pedimos al GPS (NavMesh) que calcule la ruta
        agent.SetDestination(targetPlayer.position);

        // 2. Usamos esa dirección para empujar el cuerpo (Rigidbody)
        rb.linearVelocity = agent.desiredVelocity;

        // 3. Sincronizamos el GPS con la posición real para que no se pierda al chocar
        agent.nextPosition = rb.position;

        animator.SetFloat("Speed", rb.linearVelocity.magnitude);
        if (rb.linearVelocity.x != 0)
        {
            transform.localScale = new Vector3(-Mathf.Sign(rb.linearVelocity.x), 1, 1);
        }
    }

    private void StopMovementFísico()
    {
        if (agent.isOnNavMesh) agent.isStopped = true;
        rb.linearVelocity = Vector2.zero;
        agent.nextPosition = rb.position;
        animator.SetFloat("Speed", 0f);
    }

    private IEnumerator SpawnSequence()
    {
        isSpawning = true;
        StopMovementFísico();
        animator.SetTrigger("Spawn");
        yield return new WaitForSeconds(spawnDuration);
        isSpawning = false;
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;
        StopMovementFísico();

        animator.SetTrigger("Attack");

        if (targetPlayer != null)
        {
            Health playerHealth = targetPlayer.GetComponent<Health>();
            if (playerHealth != null) playerHealth.TakeDamage(monsterDamage);
            if (fearManager != null) fearManager.CurrentFearLevel += fearDealtOnHit;
        }
        
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }

    private void LocatePlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) targetPlayer = player.transform;
    }
}