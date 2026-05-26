using UnityEngine;
using System.Collections;

public class EnemyController : MonstruoBase
{
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2.0f;
    [SerializeField] private float attackDuration = 0.6f;

    private bool isAttacking = false;
    private float nextAttackTime = 0f;

    protected override void ResetState()
    {
        isAttacking = false;
        nextAttackTime = 0f;
    }

    protected override void Update()
    {
        base.Update();
        if (isSpawning || isAttacking || targetPlayer == null) return;

        float dist = Vector2.Distance(transform.position, targetPlayer.position);
        if (dist <= attackRange && Time.time >= nextAttackTime)
            StartCoroutine(AttackSequence());
    }

    protected override void HandlePhysics()
    {
        if (isAttacking || targetPlayer == null) { StopMovement(); return; }

        float dist = Vector2.Distance(rb.position, targetPlayer.position);
        if (dist > attackRange) ChaseWithNavMesh();
        else StopMovement();
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