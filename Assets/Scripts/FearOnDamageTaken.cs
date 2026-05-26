using UnityEngine;

public class FearOnDamageTaken : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private FearManager fearManager;

    [Header("Configuración")]
    [SerializeField] private int fearIncreasePerHit = 10;

    private void OnEnable()
    {
        if (playerHealth == null) return;
        playerHealth.OnDamageTaken.AddListener(HandleDamageTaken);
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnDamageTaken.RemoveListener(HandleDamageTaken);
    }

    private void HandleDamageTaken(float amount)
    {
        if (fearManager == null) return;
        fearManager.CurrentFearLevel += fearIncreasePerHit;
    }
}
