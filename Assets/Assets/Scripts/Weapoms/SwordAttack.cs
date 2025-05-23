using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [Header("Настройки")]
    public int damage = 15;
    public LayerMask enemyLayer;
    public float damageCooldown = 0.2f;

    [Header("Эффекты")]
    public AudioClip hitSound;
    public ParticleSystem hitEffect;

    private float _lastHitTime;
    private AudioSource _audio;

    void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (Time.time < _lastHitTime + damageCooldown) return;
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        Health enemyHealth = other.GetComponent<Health>();
        if (enemyHealth != null)
        {
            _lastHitTime = Time.time;
            enemyHealth.TakeDamage(damage);
            PlayEffects(other.transform.position);
        }
    }
    void PlayEffects(Vector3 position)
    {
        if (hitEffect != null)
            Instantiate(hitEffect, position, Quaternion.identity);

        if (hitSound != null && _audio != null)
            _audio.PlayOneShot(hitSound);
    }
}