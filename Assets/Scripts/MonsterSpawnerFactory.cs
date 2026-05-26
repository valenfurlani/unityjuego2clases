
using UnityEngine;

public class MonsterSpawnerFactory : MonoBehaviour, IMonsterFactory, IFearObserver
{
    [SerializeField] private FearManager fearManager;
    [SerializeField] private SpawnTableSO spawnTable;
    
    private void OnEnable() => fearManager.RegisterObserver(this);
    private void OnDisable() => fearManager.UnregisterObserver(this);

    public void OnFearLevelChanged(int fearLevel)
    {
        Debug.Log($"Factory: Preparing harder monsters for level {fearLevel}");
    }

    public GameObject CreateMonster(int fearLevel, Vector3 position)
    {
        MonsterDataSO data = spawnTable.GetRandomMonsterForFear(fearLevel);
    
        if (data != null && data.prefab != null)
        {
            // 1. Instanciar el clon del enemigo en la escena
            GameObject instance = Instantiate(data.prefab, position, Quaternion.identity);
        
            // 2. Obtener el controlador del enemigo inyectarle sus datos correspondientes
            MonstruoBase controller = instance.GetComponent<MonstruoBase>();
            if (controller != null)
            {
                controller.Initialize(data);
            }
        
            return instance;
        }
        return null;
    }
}
