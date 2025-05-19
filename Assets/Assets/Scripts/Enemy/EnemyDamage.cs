using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage = 10;
    public float attackCooldown = 1f;
    private float lastAttackTime;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time > lastAttackTime + attackCooldown)
        {
            other.GetComponent<Health>().TakeDamage(damage);
            lastAttackTime = Time.time;
        }
    }
}
