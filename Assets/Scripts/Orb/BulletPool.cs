using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsabilidad única: gestionar instancias reciclables del proyectil del jugador.
/// Las balas se devuelven solas al pool al expirar su lifetime o al impactar.
/// </summary>
public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int        preloadCount = 10;

    private readonly Queue<GameObject> _inactive = new Queue<GameObject>();
    private Transform _poolParent;

    private void Awake()
    {
        _poolParent = new GameObject("[BulletPool]").transform;
        _poolParent.SetParent(transform);

        for (int i = 0; i < preloadCount; i++)
            _inactive.Enqueue(CreateNew());
    }

    // ── API pública ───────────────────────────────────────────────────────────

    /// <summary>
    /// Saca una bala del pool, la posiciona y la activa.
    /// </summary>
    public GameObject Get(Vector3 position)
    {
        GameObject bullet = _inactive.Count > 0
            ? _inactive.Dequeue()
            : CreateNew();

        bullet.transform.position = position;
        bullet.transform.rotation = Quaternion.identity;
        bullet.SetActive(true);
        return bullet;
    }

    /// <summary>
    /// Devuelve una bala al pool. OrbProjectile lo llama internamente.
    /// </summary>
    public void Return(GameObject bullet)
    {
        if (bullet == null) return;
        bullet.SetActive(false);
        bullet.transform.SetParent(_poolParent);
        _inactive.Enqueue(bullet);
    }

    // ── Privado ───────────────────────────────────────────────────────────────

    private GameObject CreateNew()
    {
        GameObject obj = Instantiate(bulletPrefab, _poolParent);
        obj.SetActive(false);
        return obj;
    }
}
