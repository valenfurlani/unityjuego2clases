
using UnityEngine;
using System.Collections.Generic;

public class MonsterSpawnerFactory : MonoBehaviour, IMonsterFactory, IFearObserver
{
    [SerializeField] private FearManager fearManager;
    [SerializeField] private SpawnTableSO spawnTable;
    [SerializeField] private FearConfigSO fearConfig;
    [SerializeField] private float minSpawnRadius = 5f;
    [SerializeField] private float maxSpawnRadius = 10f;

    private int currentRangeIndex = -1;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private void OnEnable() => fearManager.RegisterObserver(this);
    private void OnDisable() => fearManager.UnregisterObserver(this);

    public void OnFearLevelChanged(int fearLevel)
    {
        if (fearConfig == null || spawnTable == null) return;
        if (spawnTable.maxEnemiesPerRange == null || spawnTable.maxEnemiesPerRange.Length == 0) return;

        CleanDeadEnemies();

        int newRangeIndex = fearConfig.GetRangeIndex(fearLevel);
        currentRangeIndex = newRangeIndex;

        int targetCount = spawnTable.maxEnemiesPerRange[newRangeIndex];
        int toSpawn = targetCount - activeEnemies.Count;

        for (int i = 0; i < toSpawn; i++)
            CreateMonster(fearLevel, GetRandomSpawnPosition());
    }

    public GameObject CreateMonster(int fearLevel, Vector3 position)
    {
        MonsterDataSO data = spawnTable.GetRandomMonsterForFear(fearLevel);

        if (data != null && data.prefab != null)
        {
            GameObject instance = Instantiate(data.prefab, position, Quaternion.identity);
            MonstruoBase controller = instance.GetComponent<MonstruoBase>();
            if (controller != null)
                controller.Initialize(data);

            activeEnemies.Add(instance);
            return instance;
        }
        return null;
    }

    private void CleanDeadEnemies()
    {
        activeEnemies.RemoveAll(e => e == null || !e.activeInHierarchy);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        GameObject player = GameObject.FindWithTag("Player");
        Vector3 center = player != null ? player.transform.position : transform.position;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Random.Range(minSpawnRadius, maxSpawnRadius);
        return center + new Vector3(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance, 0f);
    }
}
