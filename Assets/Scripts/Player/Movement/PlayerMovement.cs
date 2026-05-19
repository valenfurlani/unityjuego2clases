using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }
    
    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    void ApplyMovement()
    {
        Vector2 targetVelocity = moveDirection * moveSpeed;
        rb.linearVelocity = targetVelocity;
    }
}
