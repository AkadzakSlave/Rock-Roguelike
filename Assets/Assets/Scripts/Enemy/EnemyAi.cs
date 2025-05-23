using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAi : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public LayerMask playerLayer;
    public LayerMask wallLayer;

    [Header("Behavior Settings")]
    public float returnDelay = 2f;
    public float stoppingDistance = 0.1f; 
    [Header("Attack Settings")]
    public int damage = 10;
    public float attackCooldown = 2f;
    public float knockbackForce = 3f;

    private Transform player;
    private Rigidbody2D rb;
    private float lastAttackTime;
    private Vector2 originPosition; 
    private bool isReturning;
    private float lostPlayerTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        originPosition = transform.position;

        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        
        if (distance < detectionRange)
        {
            lostPlayerTime = Time.time;
            isReturning = false;
            MoveTowardsPlayer(distance);
        }
        else if (Time.time > lostPlayerTime + returnDelay)
        {
            ReturnToOrigin();
        }
    }

    void MoveTowardsPlayer(float distance)
    {
        Vector2 direction = (player.position - transform.position).normalized;
        
        if (!Physics2D.Raycast(transform.position, direction, 0.5f, wallLayer))
        {
            if (distance > attackRange)
            {
                rb.velocity = direction * moveSpeed;
            }
            else
            {
                rb.velocity = Vector2.zero;
                TryAttack();
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    void ReturnToOrigin()
    {
        isReturning = true;
        float distanceToOrigin = Vector2.Distance(transform.position, originPosition);
        
        if (distanceToOrigin > stoppingDistance)
        {
            Vector2 direction = (originPosition - (Vector2)transform.position).normalized;
            
            if (!Physics2D.Raycast(transform.position, direction, 0.5f, wallLayer))
            {
                rb.velocity = direction * moveSpeed * 0.7f;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            isReturning = false;
        }
    }

    void TryAttack() 
    {
        if (Time.time > lastAttackTime + attackCooldown)
        {
            if (player.TryGetComponent(out Health health))
            {
                health.TakeDamage(damage);
                ApplyKnockback();
                lastAttackTime = Time.time;
            }
        }
    }
    void ApplyKnockback()
    {
        if (player.TryGetComponent<Rigidbody2D>(out var playerRb))
        {
            Vector2 knockbackDirection = (player.position - transform.position).normalized;
            playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            rb.velocity = Vector2.zero;
        }
    }
}