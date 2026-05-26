using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class MonsterSpawnerFactory : MonoBehaviour, IMonsterFactory, IFearObserver
{
    [Header("Referencias")]
    [SerializeField] private FearManager fearManager;
    [SerializeField] private SpawnTableSO spawnTable;
    [SerializeField] private FearConfigSO fearConfig;

    [Header("Spawn Radius")]
    [SerializeField] private float minSpawnRadius = 5f;
    [SerializeField] private float maxSpawnRadius = 10f;
    [SerializeField] private float navMeshSampleRadius = 2f;
    [SerializeField] private int maxSpawnAttempts = 10;

    [Header("Spawn Continuo")]
    [SerializeField] private float spawnCheckInterval = 2f;

    private int _currentFearLevel  = 0;
    private int _currentLimitIndex = 0;
    private int _activeLimit = 0;

    private readonly Dictionary<GameObject, EnemyPool> _pools = new Dictionary<GameObject, EnemyPool>();
    private readonly List<GameObject> _activeEnemies = new List<GameObject>();

    private Transform _poolParent;
    private Coroutine _maintainCoroutine;

    private void Awake()
    {
        _poolParent = new GameObject("[EnemyPool]").transform;
        _poolParent.SetParent(transform);
    }

    private void OnEnable()  => fearManager.RegisterObserver(this);
    private void OnDisable()
    {
        fearManager.UnregisterObserver(this);
        if (_maintainCoroutine != null) StopCoroutine(_maintainCoroutine);
    }

    public void OnFearLevelChanged(int fearLevel)
    {
        if (fearConfig == null || spawnTable == null) return;
        if (spawnTable.maxEnemiesPerRange == null || spawnTable.maxEnemiesPerRange.Length == 0) return;

        _currentFearLevel  = fearLevel;
        _currentLimitIndex = fearConfig.GetRangeIndex(fearLevel);
        _activeLimit       = spawnTable.maxEnemiesPerRange[_currentLimitIndex];

        if (_maintainCoroutine != null) StopCoroutine(_maintainCoroutine);
        _maintainCoroutine = StartCoroutine(MaintainEnemyCount());
    }

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

    private EnemyPool GetOrCreatePool(GameObject prefab)
    {
        if (!_pools.TryGetValue(prefab, out EnemyPool pool))
        {
            pool = new EnemyPool(prefab, _poolParent, preloadCount: 3);
            _pools[prefab] = pool;
        }
        return pool;
    }

    private void CleanDeadEnemies()
    {
        _activeEnemies.RemoveAll(e => e == null || !e.activeInHierarchy);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        GameObject player = GameObject.FindWithTag("Player");
        Vector3 center = player != null ? player.transform.position : transform.position;

        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(minSpawnRadius, maxSpawnRadius);
            Vector3 candidate = center + new Vector3(Mathf.Cos(angle) * distance,
                                                     Mathf.Sin(angle) * distance, 0f);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, navMeshSampleRadius, NavMesh.AllAreas))
                return hit.position;
        }

        return Vector3.negativeInfinity;
    }
}
