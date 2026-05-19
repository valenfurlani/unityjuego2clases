using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerInput playerInput;
    private Vector2 inputDirection;
    private PlayerShooter playerShooter;
    private PlayerAnimator playerAnimator; // Nueva referencia al sistema visual

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<PlayerAnimator>(); // Obtenemos la referencia
    }
    
    void Start()
    {
        // ... Mantén tu lógica original de Input Action Maps aquí ...
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>();
    }
    
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started && playerShooter != null)
        {
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            mouseWorldPosition.z = 0f;

            // 1. Lógica de Combate: Disparar proyectil
            playerShooter.ShootTowards(mouseWorldPosition);

            // 2. Lógica Visual: Reproducir animación de ataque
            if (playerAnimator != null)
            {
                playerAnimator.TriggerAttackAnimation();
            }
        }
    }

    void Update()
    {
        if (playerMovement != null)
        {
            playerMovement.SetMoveDirection(inputDirection);
        }
    }
}