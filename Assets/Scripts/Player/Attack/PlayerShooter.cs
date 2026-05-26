using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Configuración del Disparo")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private BulletPool bulletPool;

    public void ShootTowards(Vector2 targetWorldPosition)
    {
        if (bulletPool == null || firePoint == null) return;

        Vector2 direction = targetWorldPosition - (Vector2)firePoint.position;

        GameObject bulletObj = bulletPool.Get(firePoint.position);

        OrbProjectile projectile = bulletObj.GetComponent<OrbProjectile>();
        if (projectile != null)
            projectile.Launch(direction.normalized, bulletPool);
    }
}
