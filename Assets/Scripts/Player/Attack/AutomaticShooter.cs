using UnityEngine;

public class AutomaticShooter : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float fireRate = 0.8f;
    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Color rangeColor = new Color(0.4f, 0.85f, 1f, 0.45f);

    private float nextFireTime = 0f;
    private LineRenderer lineRenderer;
    private const int circleSegments = 80;

    private void Awake()
    {
        SetupRangeIndicator();
    }

    private void SetupRangeIndicator()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.loop = true;
        lineRenderer.positionCount = circleSegments;
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = 0.06f;
        lineRenderer.endWidth = 0.06f;
        lineRenderer.sortingOrder = 10;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = rangeColor;
        lineRenderer.endColor = rangeColor;

        for (int i = 0; i < circleSegments; i++)
        {
            float angle = (float)i / circleSegments * Mathf.PI * 2f;
            lineRenderer.SetPosition(i, new Vector3(
                Mathf.Cos(angle) * detectionRadius,
                Mathf.Sin(angle) * detectionRadius,
                0f));
        }
    }

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
        
        // Obtenemos la bala del pool en lugar de instanciarla
        GameObject orb = bulletPool.Get(spawnPos);
        OrbProjectile projectile = orb.GetComponent<OrbProjectile>();
        projectile?.Launch(direction, bulletPool);
    }
}
