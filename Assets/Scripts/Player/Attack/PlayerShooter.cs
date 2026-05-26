using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Configuración del Disparo")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private BulletPool bulletPool;

    public void ShootTowards(Vector2 targetWorldPosition)
    {
        if (bulletPool == null || firePoint == null) return;

        // 1. Dirección hacia el objetivo
        Vector2 direction = targetWorldPosition - (Vector2)firePoint.position;

        // 2. Pide una bala al pool (no Instantiate)
        GameObject bulletObj = bulletPool.Get(firePoint.position);

        // 3. Lanza la bala pasándole la referencia al pool para que pueda devolverse sola
        OrbProjectile projectile = bulletObj.GetComponent<OrbProjectile>();
        if (projectile != null)
            projectile.Launch(direction.normalized, bulletPool);
    }
}
