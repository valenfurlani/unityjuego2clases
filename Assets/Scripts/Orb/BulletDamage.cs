using UnityEngine;

public class BulletDamage : MonoBehaviour, IDamageDealer
{
    [SerializeField] private float damageAmount;

    public float GetDamage()
    {
        return damageAmount;
    }
}
