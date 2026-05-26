using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsabilidad única: gestionar un pool de instancias de un prefab concreto.
/// Reutiliza GameObjects inactivos en lugar de Instantiate/Destroy en cada spawn.
/// </summary>
public class EnemyPool
{
    private readonly GameObject _prefab;
    private readonly Transform _poolParent;
    private readonly Queue<GameObject> _inactive = new Queue<GameObject>();

    public EnemyPool(GameObject prefab, Transform poolParent, int preloadCount = 0)
    {
        _prefab    = prefab;
        _poolParent = poolParent;

        for (int i = 0; i < preloadCount; i++)
            _inactive.Enqueue(CreateNew());
    }

    /// <summary>
    /// Devuelve una instancia inactiva o crea una nueva si no hay disponibles.
    /// </summary>
    public GameObject Get(Vector3 position)
    {
        GameObject instance = _inactive.Count > 0
            ? _inactive.Dequeue()
            : CreateNew();

        instance.transform.position = position;
        instance.SetActive(true);
        return instance;
    }

    /// <summary>
    /// Devuelve una instancia al pool desactivándola.
    /// MonstruoBase ya llama a SetActive(false) en OnDie, por lo que
    /// este método puede invocarse manualmente si hace falta.
    /// </summary>
    public void Return(GameObject instance)
    {
        if (instance == null) return;
        instance.SetActive(false);
        instance.transform.SetParent(_poolParent);
        _inactive.Enqueue(instance);
    }

    /// <summary>Cuántas instancias activas tiene este pool en escena.</summary>
    public int ActiveCount { get; private set; }

    public void TrackActivated()  => ActiveCount++;
    public void TrackDeactivated() => ActiveCount = Mathf.Max(0, ActiveCount - 1);

    // ── Privado ───────────────────────────────────────────────────────────────

    private GameObject CreateNew()
    {
        GameObject obj = Object.Instantiate(_prefab, _poolParent);
        obj.SetActive(false);
        return obj;
    }
}
