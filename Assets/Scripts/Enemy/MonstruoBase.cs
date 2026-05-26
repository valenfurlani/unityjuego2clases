using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class MonstruoBase : MonoBehaviour, IDamageDealer
{
    protected Transform targetPlayer;
    protected NavMeshAgent agent;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected float currentSpeed;
    protected float monsterDamage;

    [SerializeField] protected float spawnDuration = 1.5f;
    protected bool isSpawning = true;

    private bool isFirstEnable = true;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.updatePosition = false;

        Health health = GetComponent<Health>();
        if (health != null) health.OnDeath.AddListener(OnDie);
    }

    protected virtual void OnEnable()
    {
        if (isFirstEnable)
        {
            isFirstEnable = false;
            return;
        }

        ActivateEnemy();
    }

    protected virtual void Start()
    {
        ActivateEnemy();
    }

    private void ActivateEnemy()
    {
        isSpawning = true;
        LocatePlayer();
        StartCoroutine(SpawnSequence());
    }

    protected virtual void Update()
    {
        if (targetPlayer == null) LocatePlayer();
    }

    protected virtual void FixedUpdate()
    {
        if (!isSpawning) HandlePhysics();
    }

    public virtual void Initialize(MonsterDataSO data)
    {
        currentSpeed  = data.speed;
        monsterDamage = data.damage;
        if (agent != null) agent.speed = currentSpeed;

        Health health = GetComponent<Health>();
        if (health != null) health.SetMaxHealth(data.health);  // vida desde el SO

        ResetState();
    }

    protected virtual void ResetState() { }

    protected abstract void HandlePhysics();

    public float GetDamage() => monsterDamage;

    protected virtual void OnDie()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    protected void LocatePlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) targetPlayer = player.transform;
    }

    protected void ChaseWithNavMesh()
    {
        if (targetPlayer == null) return;
        agent.isStopped = false;
        agent.SetDestination(targetPlayer.position);
        rb.linearVelocity = agent.desiredVelocity;
        agent.nextPosition = rb.position;
        if (animator != null) animator.SetFloat("Speed", rb.linearVelocity.magnitude);
        if (rb.linearVelocity.x != 0)
            transform.localScale = new Vector3(-Mathf.Sign(rb.linearVelocity.x), 1, 1);
    }

    protected void StopMovement()
    {
        if (agent.isOnNavMesh) agent.isStopped = true;
        rb.linearVelocity = Vector2.zero;
        agent.nextPosition = rb.position;
        if (animator != null) animator.SetFloat("Speed", 0f);
    }

    protected IEnumerator AttackPlayer(float attackDuration)
    {
        if (animator != null) animator.SetTrigger("Attack");
        Health playerHealth = targetPlayer?.GetComponent<Health>();
        playerHealth?.TakeDamage(monsterDamage);
        yield return new WaitForSeconds(attackDuration);
    }

    private IEnumerator SpawnSequence()
    {
        isSpawning = true;
        StopMovement();
        if (animator != null) animator.SetTrigger("Spawn");
        yield return new WaitForSeconds(spawnDuration);
        isSpawning = false;
    }
}
