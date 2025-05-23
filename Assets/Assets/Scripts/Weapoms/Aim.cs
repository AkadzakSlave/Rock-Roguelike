using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    [Header("Aiming")]
    public float rotationOffset = -90f;
    public float distanceFromPlayer = 0.5f;

    [Header("Attack")]
    public float attackCooldown = 0.5f;
    public GameObject hitEffect;

    private Transform player;
    private float nextAttackTime;
    private Animator anim;

    void Start()
    {
        player = transform.parent;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        AimAtMouse();
        
        if (Input.GetMouseButtonDown(0) && Time.time > nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void AimAtMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - player.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
        transform.position = player.position + (Vector3)(direction * distanceFromPlayer);
    }
    public void EnableDamage()
    {
        GetComponent<Collider2D>().enabled = true;
    }
    public void DisableDamage()
    {
        GetComponent<Collider2D>().enabled = false;
    }
}