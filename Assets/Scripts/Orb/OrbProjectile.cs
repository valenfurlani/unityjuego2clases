using UnityEngine;

public class OrbProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;
    
    private Rigidbody2D rb;
    private IDamageDealer damageDealer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        damageDealer = GetComponent<IDamageDealer>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Método para inicializar la dirección desde el script del jugador
    public void Launch(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<MonstruoBase>() == null) return;

        Health targetHealth = collision.GetComponent<Health>();
        if (targetHealth != null && damageDealer != null)
            targetHealth.TakeDamage(damageDealer.GetDamage());

        Destroy(gameObject);
    }
}
