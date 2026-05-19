using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Velocidad")]
    [Tooltip("Velocidad de movimiento del personaje")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb2D;
    private Vector2 moveDirection;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        
        // Configuraciones recomendadas para evitar rotaciones indeseadas por colisiones
        rb2D.freezeRotation = true;
        
        // Si es un juego top-down, la gravedad del Rigidbody2D debe ser 0 en el Inspector
    }

    // Método público para que el PlayerController envíe la dirección
    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;
    }

    void FixedUpdate()
    {
        // Aplicamos la física en el ciclo FixedUpdate
        ApplyMovement();
    }

    void ApplyMovement()
    {
        // Calculamos el vector de velocidad deseado
        Vector2 targetVelocity = moveDirection * moveSpeed;
        
        // En Unity 6 se utiliza linearVelocity para Rigidbody2D
        rb2D.linearVelocity = targetVelocity;
    }
}
