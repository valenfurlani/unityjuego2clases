using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Referencia al script que maneja la física
    private PlayerMovement playerMovement;
    private PlayerInput playerInput;
    // Almacena la dirección actual del input
    private Vector2 inputDirection;

    void Awake()
    {
        // Obtenemos la referencia del componente de movimiento en el mismo objeto
        playerMovement = GetComponent<PlayerMovement>();
    }
    
    void Start()
    {
        // Validamos que el componente y sus acciones existan
        if (playerInput != null && playerInput.actions != null)
        {
            // 1. Desactivamos todos los mapas de acción globales para limpiar el estado
            foreach (var map in playerInput.actions.actionMaps)
            {
                map.Disable();
            }

            // 2. Activamos manualmente SOLO el mapa que vamos a usar en este script
            // Nota: Asegúrate de que tu Action Map se llame "Player" en tu asset de Input Actions
            var playerMap = playerInput.actions.FindActionMap("Player");
            if (playerMap != null)
            {
                playerMap.Enable();
            }
            else
            {
                Debug.LogWarning("No se encontró el Action Map llamado 'Player'. Revisa tu asset de Input.");
            }
        }
    }

    // Este método es llamado por el componente PlayerInput (Mensaje: OnMove)
    public void OnMove(InputAction.CallbackContext context)
    {
        // Guardamos el Vector2 generado por WASD o las flechas
        inputDirection = context.ReadValue<Vector2>();
    }

    void Update()
    {
        // Pasamos constantemente la dirección al script de movimiento
        if (playerMovement != null)
        {
            playerMovement.SetMoveDirection(inputDirection);
        }
    }
}
