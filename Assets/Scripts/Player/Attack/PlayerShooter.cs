using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [Header("Configuración del Disparo")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    
    public void ShootTowards(Vector2 targetWorldPosition)
    {
        if (projectilePrefab == null || firePoint == null) return;

        // 1. Calculamos la dirección: (Destino - Origen)
        Vector2 firePointPosition = firePoint.position;
        Vector2 direction = targetWorldPosition - firePointPosition;

        // 2. Creamos la bolita en el FirePoint
        GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        
        // 3. Obtenemos el proyectil y lo lanzamos en esa dirección (normalizada)
        OrbProjectile projectile = projectileObj.GetComponent<OrbProjectile>();
        if (projectile != null)
        {
            projectile.Launch(direction.normalized);
        }
    }
    

    
}
