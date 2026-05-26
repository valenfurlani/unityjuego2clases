using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Responsabilidad única: decidir cuándo y dónde spawnear enemigos,
/// delegando la creación/reciclado al EnemyPool y el límite al SpawnTableSO.
///
/// SOLID:
///  S – Solo coordina spawning; EnemyPool gestiona las instancias.
///  O – Extensible via IMonsterFactory sin modificar esta clase.
///  D – Depende de FearManager e IFearObserver por abstracción.
/// </summary>
public class MonsterSpawnerFactory : MonoBehaviour, IMonsterFactory, IFearObserver
{
    [Header("Referencias")]
    [SerializeField] private FearManager fearManager;
    [SerializeField] private SpawnTableSO spawnTable;
    [SerializeField] private FearConfigSO fearConfig;

    [Header("Spawn Radius")]
    [SerializeField] private float minSpawnRadius = 5f;
    [SerializeField] private float maxSpawnRadius = 10f;
    [Tooltip("Radio de búsqueda sobre el NavMesh alrededor del punto candidato.")]
    [SerializeField] private float navMeshSampleRadius = 2f;
    [Tooltip("Intentos máximos para encontrar un punto válido en el NavMesh.")]
    [SerializeField] private int maxSpawnAttempts = 10;

    [Header("Spawn Continuo")]
    [Tooltip("Segundos entre cada comprobación de si hay que spawnear más enemigos.")]
    [SerializeField] private float spawnCheckInterval = 2f;

    // ── Estado interno ────────────────────────────────────────────────────────
    private int _currentFearLevel  = 0;
    private int _currentLimitIndex = 0;

    /// <summary>Límite activo de enemigos simultáneos (editable en Play Mode).</summary>
    private int _activeLimit = 0;

    // Un pool por prefab único
    private readonly Dictionary<GameObject, EnemyPool> _pools = new Dictionary<GameObject, EnemyPool>();

    // Todas las instancias activas en escena (sin importar su prefab)
    private readonly List<GameObject> _activeEnemies = new List<GameObject>();

    private Transform _poolParent;
    private Coroutine _maintainCoroutine;

    // ── Unity lifecycle ───────────────────────────────────────────────────────

    private void Awake()
    {
        // Crea un GameObject contenedor para mantener la jerarquía ordenada
        _poolParent = new GameObject("[EnemyPool]").transform;
        _poolParent.SetParent(transform);
    }

    private void OnEnable()  => fearManager.RegisterObserver(this);
    private void OnDisable()
    {
        fearManager.UnregisterObserver(this);
        if (_maintainCoroutine != null) StopCoroutine(_maintainCoroutine);
    }

    // ── IFearObserver ─────────────────────────────────────────────────────────

    public void OnFearLevelChanged(int fearLevel)
    {
        if (fearConfig == null || spawnTable == null) return;
        if (spawnTable.maxEnemiesPerRange == null || spawnTable.maxEnemiesPerRange.Length == 0) return;

        _currentFearLevel  = fearLevel;
        _currentLimitIndex = fearConfig.GetRangeIndex(fearLevel);
        _activeLimit       = spawnTable.maxEnemiesPerRange[_currentLimitIndex];

        // Arranca (o reinicia) el loop de mantenimiento continuo
        if (_maintainCoroutine != null) StopCoroutine(_maintainCoroutine);
        _maintainCoroutine = StartCoroutine(MaintainEnemyCount());
    }

    // ── IMonsterFactory ───────────────────────────────────────────────────────

    public GameObject CreateMonster(int fearLevel, Vector3 position)
    {
        MonsterDataSO data = spawnTable.GetRandomMonsterForFear(fearLevel);
        if (data == null || data.prefab == null) return null;

        EnemyPool pool = GetOrCreatePool(data.prefab);
        GameObject instance = pool.Get(position);

        MonstruoBase controller = instance.GetComponent<MonstruoBase>();
        if (controller != null) controller.Initialize(data);

        _activeEnemies.Add(instance);
        return instance;
    }

    // ── Loop de mantenimiento ─────────────────────────────────────────────────

    /// <summary>
    /// Comprueba periódicamente si el número de enemigos vivos es menor
    /// que el límite activo y spawna los que faltan.
    /// </summary>
    private IEnumerator MaintainEnemyCount()
    {
        while (true)
        {
            CleanDeadEnemies();

            int toSpawn = _activeLimit - _activeEnemies.Count;
            for (int i = 0; i < toSpawn; i++)
            {
                Vector3 pos = GetRandomSpawnPosition();
                if (pos != Vector3.negativeInfinity)
                    CreateMonster(_currentFearLevel, pos);
            }

            yield return new WaitForSeconds(spawnCheckInterval);
        }
    }

    // ── Pool helpers ──────────────────────────────────────────────────────────

    private EnemyPool GetOrCreatePool(GameObject prefab)
    {
        if (!_pools.TryGetValue(prefab, out EnemyPool pool))
        {
            pool = new EnemyPool(prefab, _poolParent, preloadCount: 3);
            _pools[prefab] = pool;
        }
        return pool;
    }

    // ── Limpieza ──────────────────────────────────────────────────────────────

    private void CleanDeadEnemies()
    {
        // Un enemigo muerto queda inactivo (SetActive false) en MonstruoBase.OnDie
        _activeEnemies.RemoveAll(e => e == null || !e.activeInHierarchy);
    }

    // ── Posición NavMesh ──────────────────────────────────────────────────────

    private Vector3 GetRandomSpawnPosition()
    {
        GameObject player = GameObject.FindWithTag("Player");
        Vector3 center = player != null ? player.transform.position : transform.position;

        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            float angle    = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(minSpawnRadius, maxSpawnRadius);
            Vector3 candidate = center + new Vector3(Mathf.Cos(angle) * distance,
                                                     Mathf.Sin(angle) * distance, 0f);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, navMeshSampleRadius, NavMesh.AllAreas))
                return hit.position;
        }

        Debug.LogWarning("[MonsterSpawnerFactory] No se encontró posición válida en el NavMesh.");
        return Vector3.negativeInfinity;
    }
}
