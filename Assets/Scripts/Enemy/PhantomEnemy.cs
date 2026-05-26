using UnityEngine;
using System.Collections;

public class PhantomEnemy : MonstruoBase
{
    [SerializeField] private float visibilityRange = 5f;
    [SerializeField] private float phantomAlpha = 0.15f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2.0f;
    [SerializeField] private float attackDuration = 0.6f;

    private bool isAttacking = false;
    private bool isPhantom = true;
    private float nextAttackTime = 0f;

    protected override void ResetState()
    {
        isAttacking = false;
        isPhantom = true;
        nextAttackTime = 0f;
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = phantomAlpha;
            spriteRenderer.color = c;
        }
        if (bodyCollider != null) bodyCollider.enabled = false;
    }
    private SpriteRenderer spriteRenderer;
    private Collider2D bodyCollider;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        bodyCollider = GetComponent<Collider2D>();
    }

    protected override void Update()
    {
        base.Update();
        if (targetPlayer == null) return;

        float dist = Vector2.Distance(transform.position, targetPlayer.position);
        UpdatePhantomState(dist);

        if (isSpawning || isAttacking || isPhantom) return;

        if (dist <= attackRange && Time.time >= nextAttackTime)
            StartCoroutine(AttackSequence());
    }

    protected override void HandlePhysics()
    {
        if (isAttacking) { StopMovement(); return; }
        if (targetPlayer == null) { StopMovement(); return; }

        float dist = Vector2.Distance(rb.position, targetPlayer.position);
        if (!isPhantom && dist <= attackRange) StopMovement();
        else ChaseWithNavMesh();
    }

    private void UpdatePhantomState(float distToPlayer)
    {
        bool shouldBePhantom = distToPlayer > visibilityRange;
        if (shouldBePhantom == isPhantom) return;

        isPhantom = shouldBePhantom;

        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = isPhantom ? phantomAlpha : 1f;
            spriteRenderer.color = c;
        }

        if (bodyCollider != null)
            bodyCollider.enabled = !isPhantom;
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;
        StopMovement();
        yield return StartCoroutine(AttackPlayer(attackDuration));
        isAttacking = false;
    }
}
