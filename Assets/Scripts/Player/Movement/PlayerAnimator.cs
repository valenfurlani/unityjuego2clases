using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        animator.SetFloat("Speed", rb.linearVelocity.magnitude);

        if (Mathf.Abs(rb.linearVelocity.x) > 0.1f) 
        {
            float directionSign = Mathf.Sign(rb.linearVelocity.x);
            transform.localScale = new Vector3(directionSign, 1f, 1f);
        }
    }

    public void TriggerAttackAnimation()
    {
        animator.SetTrigger("Attack");
    }
}