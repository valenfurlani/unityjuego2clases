using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip damageClip;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (health != null)
            health.OnDamageTaken.AddListener(OnDamageTaken);
    }

    private void OnDestroy()
    {
        if (health != null)
            health.OnDamageTaken.RemoveListener(OnDamageTaken);
    }

    private void OnDamageTaken(float amount)
    {
        if (sfxSource != null && damageClip != null)
            sfxSource.PlayOneShot(damageClip);
    }
}
