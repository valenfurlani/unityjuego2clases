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
        // Intentamos obtener el componente de vida del objeto con el que chocamos
        Health targetHealth = collision.GetComponent<Health>();

        if (targetHealth != null && damageDealer != null)
        {
            // Aplicamos el daño de forma limpia
            targetHealth.TakeDamage(damageDealer.GetDamage());
        }

        // El proyectil se destruye al impactar con cualquier superficie sólida o entidad
        Destroy(gameObject);
    } 
}
