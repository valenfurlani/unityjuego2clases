
using UnityEngine;

public interface IMonsterFactory
{
    GameObject CreateMonster(int fearLevel, Vector3 position);
}
