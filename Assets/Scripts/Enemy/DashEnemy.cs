using UnityEngine;
using System.Collections;

public class DashEnemy : MonstruoBase
{
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2.0f;
    [SerializeField] private float attackDuration = 0.6f;
    [SerializeField] private float dashInterval = 3f;
    [SerializeField] private float dashForce = 18f;
    [SerializeField] private float dashDuration = 0.25f;

    private bool isAttacking = false;
    private bool isDashing = false;
    private float nextAttackTime = 0f;
    private float nextDashTime = 0f;
    private Vector2 dashDirection;

    protected override void ResetState()
    {
        isAttacking = false;
        isDashing = false;
        nextAttackTime = 0f;
        nextDashTime = 0f;
    }

    protected override void Update()
    {
        base.Update();
        if (isSpawning || isAttacking || isDashing || targetPlayer == null) return;

        float dist = Vector2.Distance(transform.position, targetPlayer.position);

        if (dist <= attackRange && Time.time >= nextAttackTime)
            StartCoroutine(AttackSequence());
        else if (Time.time >= nextDashTime)
            StartCoroutine(DashSequence());
    }

    protected override void HandlePhysics()
    {
        if (isDashing)
        {
            agent.isStopped = true;
            rb.linearVelocity = dashDirection * dashForce;
            agent.nextPosition = rb.position;
            return;
        }

        if (isAttacking || targetPlayer == null) { StopMovement(); return; }

        float dist = Vector2.Distance(rb.position, targetPlayer.position);
        if (dist > attackRange) ChaseWithNavMesh();
        else StopMovement();
    }

    private IEnumerator DashSequence()
    {
        isDashing = true;
        nextDashTime = Time.time + dashInterval;
        dashDirection = targetPlayer != null
            ? ((Vector2)targetPlayer.position - rb.position).normalized
            : Vector2.zero;
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
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
