using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Основные параметры")]
    public float attackRate = 0.5f;
    public int damage = 10;
    public float attackRange = 1.5f;
    
    [Header("Ссылки")]
    public LayerMask enemyLayer;
    public Transform attackPoint;
    
    private float nextAttackTime;
    
    void Update()
    {
        if (Time.time >= nextAttackTime && Input.GetMouseButtonDown(0))
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }
    
    void Attack()
    {
        // Анимация атаки
        // PlayAnimation("Attack");
        
        // Обнаружение врагов в радиусе
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position, 
            attackRange, 
            enemyLayer
        );
        
        // Нанесение урона
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Health>()?.TakeDamage(damage);
        }
    }
    
    // Визуализация радиуса атаки в редакторе
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
