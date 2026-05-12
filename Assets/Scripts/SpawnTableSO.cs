
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSpawnTable", menuName = "FearSystem/SpawnTable")]
public class SpawnTableSO : ScriptableObject
{
    public List<MonsterDataSO> possibleMonsters;

    public MonsterDataSO GetRandomMonsterForFear(int fearLevel)
    {
        List<MonsterDataSO> validMonsters = possibleMonsters.FindAll(m => fearLevel >= m.minFearRequired);
        if (validMonsters.Count == 0) return null;
        
        // Basic weighted random logic
        float totalWeight = 0;
        foreach (var m in validMonsters) totalWeight += m.spawnWeight;
        float randomValue = Random.Range(0, totalWeight);
        float currentWeight = 0;

        foreach (var m in validMonsters)
        {
            currentWeight += m.spawnWeight;
            if (randomValue <= currentWeight) return m;
        }
        return validMonsters[0];
    }
}
