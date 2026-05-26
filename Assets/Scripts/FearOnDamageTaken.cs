using UnityEngine;

/// <summary>
/// Responsabilidad única: aumentar el nivel de miedo cuando el jugador recibe daño.
/// Escucha Health.OnDamageTaken y delega la lógica de miedo al FearManager.
///
/// SOLID:
///  S – Solo traduce "daño recibido" a "incremento de miedo".
///  D – Depende de FearManager por referencia directa (mismo GameObject o asignada).
/// </summary>
public class FearOnDamageTaken : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Health del jugador que genera el daño.")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private FearManager fearManager;

    [Header("Configuración")]
    [Tooltip("Cuántos puntos de miedo suma cada vez que el jugador recibe daño.")]
    [SerializeField] private int fearIncreasePerHit = 10;

    private void OnEnable()
    {
        if (playerHealth == null)
        {
            Debug.LogError("[FearOnDamageTaken] Falta asignar el componente Health del jugador.");
            return;
        }
        playerHealth.OnDamageTaken.AddListener(HandleDamageTaken);
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnDamageTaken.RemoveListener(HandleDamageTaken);
    }

    /// <param name="amount">Daño recibido (no se usa, pero lo exige la firma del UnityEvent).</param>
    private void HandleDamageTaken(float amount)
    {
        if (fearManager == null) return;
        fearManager.CurrentFearLevel += fearIncreasePerHit;
    }
}
