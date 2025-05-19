using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Настройки")]
    public int damage = 20;
    public float lifetime = 3f;
    public GameObject hitEffect;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().TakeDamage(damage);
            DestroyProjectile();
        }
        else if (!other.isTrigger) // Игнорируем триггеры
        {
            DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        
        Destroy(gameObject);
    }
}
