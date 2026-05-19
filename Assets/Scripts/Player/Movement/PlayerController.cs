using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerInput playerInput;
    private Vector2 inputDirection;
    private PlayerShooter playerShooter;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
        playerInput = GetComponent<PlayerInput>();
    }
    
    void Start()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            foreach (var map in playerInput.actions.actionMaps)
            {
                map.Disable();
            }
            
            var playerMap = playerInput.actions.FindActionMap("Player");
            if (playerMap != null)
            {
                playerMap.Enable();
            }
        }
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>();
    }
    
    public void OnFire(InputAction.CallbackContext context)
    {
        // Solo disparamos una vez cuando el botón se presiona (started)
        if (context.started && playerShooter != null)
        {
            // 1. Obtenemos la posición del mouse en píxeles (pantalla)
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

            // 2. La transformamos a la posición real dentro del mundo 2D usando la Cámara
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            mouseWorldPosition.z = 0f; // Nos aseguramos de mantener el plano 2D

            // 3. Le ordenamos al PlayerShooter que dispare hacia esa posición
            playerShooter.ShootTowards(mouseWorldPosition);
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
