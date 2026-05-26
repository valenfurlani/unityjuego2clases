using System.Collections;
using UnityEngine;

public class OrbProjectile : MonoBehaviour
{
    [SerializeField] private float speed    = 10f;
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody2D  _rb;
    private IDamageDealer _damageDealer;
    private BulletPool   _pool;
    private Coroutine    _lifeCoroutine;

    private void Awake()
    {
        _rb           = GetComponent<Rigidbody2D>();
        _damageDealer = GetComponent<IDamageDealer>();
    }

    private void OnDisable()
    {
        if (_lifeCoroutine != null)
        {
            StopCoroutine(_lifeCoroutine);
            _lifeCoroutine = null;
        }
        _rb.linearVelocity = Vector2.zero;
    }

    public void Launch(Vector2 direction, BulletPool pool)
    {
        _pool = pool;
        _rb.linearVelocity = direction.normalized * speed;
        _lifeCoroutine = StartCoroutine(ReturnAfterLifetime());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MonstruoBase enemy = collision.GetComponentInParent<MonstruoBase>();
        if (enemy == null) return;

        Health targetHealth = enemy.GetComponent<Health>();
        if (targetHealth != null && _damageDealer != null)
            targetHealth.TakeDamage(_damageDealer.GetDamage());

        ReturnToPool();
    }

    private IEnumerator ReturnAfterLifetime()
    {
        yield return new WaitForSeconds(lifeTime);
        ReturnToPool();
    }

    private void OnBecameInvisible()
    {
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (_pool != null)
            _pool.Return(gameObject);
        else
            gameObject.SetActive(false);
    }
}
