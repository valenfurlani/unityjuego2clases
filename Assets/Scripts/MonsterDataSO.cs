
using UnityEngine;

[CreateAssetMenu(fileName = "NewMonsterData", menuName = "FearSystem/MonsterData")]
public class MonsterDataSO : ScriptableObject
{
    public string monsterName;
    public float health;
    public float speed;
    public float damage;
    public GameObject prefab;
    public AudioClip deathSound;
    public int minFearRequired;
    public float spawnWeight;
}
