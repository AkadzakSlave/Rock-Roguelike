using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Стрельба")]
    public GameObject projectilePrefab;
    public float shootInterval = 2f;
    public float projectileSpeed = 5f;
    public float detectionRange = 7f;

    [Header("Настройки")]
    public Transform firePoint;
    public LayerMask obstacleLayers;

    public Transform _player;
    private float _shootTimer;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        if (_player == null) return;

        float distance = Vector2.Distance(transform.position, _player.position);

        if (distance <= detectionRange && HasLineOfSight())
        {
            _shootTimer += Time.deltaTime;

            if (_shootTimer >= shootInterval)
            {
                Shoot();
                _shootTimer = 0f;
            }
        }
    }

    bool HasLineOfSight()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            firePoint.position,
            (_player.position - firePoint.position).normalized,
            detectionRange,
            obstacleLayers
        );

        return hit.collider == null || hit.collider.CompareTag("Player");
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        Vector2 direction = (_player.position - firePoint.position).normalized;
        projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
        
        // Развернуть снаряд по направлению движения
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    
    Vector2 PredictPlayerPosition()
    {
        float distance = Vector2.Distance(transform.position, _player.position);
        float timeToHit = distance / projectileSpeed;
        return (Vector2)_player.position + _player.GetComponent<Rigidbody2D>().velocity * timeToHit;
    }
}
