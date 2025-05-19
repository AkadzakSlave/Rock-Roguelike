using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Beam : MonoBehaviour
{
    [Header("Настройки")]
    public float maxLength = 10f;
    public float expandSpeed = 20f;
    public float damagePerSecond = 15f;
    public float rotationSpeed = 200f;
    
    [Header("Цели")]
    public LayerMask playerLayer;
    public Transform playerTarget;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D beamCollider;
    private float currentLength;
    private float accumulatedDamage;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        beamCollider = GetComponent<BoxCollider2D>();
        currentLength = 0f;

        // Автоматически находим игрока если не назначен
        if(playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if(player != null) playerTarget = player.transform;
        }
    }

    void Update()
    {
        if(playerTarget == null) return;

        // Поворот в сторону игрока
        RotateTowardsPlayer();
        
        // Расширение луча
        if(currentLength < maxLength)
        {
            currentLength = Mathf.Min(maxLength, currentLength + expandSpeed * Time.deltaTime);
            UpdateBeamSize();
        }
    }

    void RotateTowardsPlayer()
    {
        Vector2 direction = (playerTarget.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            rotation, 
            rotationSpeed * Time.deltaTime
        );
    }

    void UpdateBeamSize()
    {
        // Обновляем спрайт
        spriteRenderer.size = new Vector2(spriteRenderer.size.x, currentLength);
        
        // Обновляем коллайдер
        beamCollider.size = new Vector2(beamCollider.size.x, currentLength);
        beamCollider.offset = new Vector2(0, currentLength / 2f);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Проверяем слой игрока
        if(((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            if(other.TryGetComponent(out Health health))
            {
                // Накопленный урон
                accumulatedDamage += damagePerSecond * Time.deltaTime;
                
                if(accumulatedDamage >= 1f)
                {
                    int damage = Mathf.FloorToInt(accumulatedDamage);
                    health.TakeDamage(damage);
                    accumulatedDamage -= damage;
                }
            }
        }
    }

    // Визуализация в редакторе
    void OnDrawGizmosSelected()
    {
        if(playerTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, playerTarget.position);
        }
    }
}