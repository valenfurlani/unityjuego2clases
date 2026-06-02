using UnityEngine;

public class AutomaticShooter : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 5f;
    public float DetectionRadius => detectionRadius;
    [SerializeField] private float fireRate = 0.8f;
    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip fireClip;

    private float nextFireTime = 0f;

    private void Update()
    {
        if (Time.time < nextFireTime) return;

        Transform target = GetNearestEnemy();
        if (target == null) return;

        FireAt(target);
        nextFireTime = Time.time + fireRate;
    }

    private Transform GetNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);

        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.transform;
            }
        }

        return nearest;
    }

    private void FireAt(Transform target)
    {
        if (bulletPool == null) return;

        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        Vector3 spawnPos = transform.position + (Vector3)(direction * 0.6f);
        
        GameObject orb = bulletPool.Get(spawnPos);
        OrbProjectile projectile = orb.GetComponent<OrbProjectile>();
        projectile?.Launch(direction, bulletPool);

        if (sfxSource != null && fireClip != null)
            sfxSource.PlayOneShot(fireClip);
    }
}
