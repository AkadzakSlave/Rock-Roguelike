using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Attack Patterns")] public AttackPattern[] attackPatterns;
    public float timeBetweenPatterns = 3f;
    private int currentPatternIndex;
    private float nextAttackTime;

    [Header("Projectiles")] public GameObject projectilePrefab; 
    public GameObject bigProjectilePrefab; 
    public GameObject beamPrefab; 
    public GameObject beamWarningPrefab; 

    [Header("Settings")] public float rotationSpeed = 30f;
    public float beamChargeTime = 1.5f;
    public LayerMask projectileLayer; 

    private Transform player;
    private Transform weaponPivot;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        weaponPivot = transform.Find("BossWeaponPivot");
        nextAttackTime = Time.time + timeBetweenPatterns;

        
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Enemy"),
            projectileLayer.value,
            true
        );
    }

    void Update()
    {
        if (player == null) return;

        if (Time.time > nextAttackTime)
        {
            StartCoroutine(PerformAttackPattern());
            nextAttackTime = Time.time + timeBetweenPatterns;
        }

        RotateWeaponPivot();
    }

    void RotateWeaponPivot()
    {
        if (weaponPivot != null)
            weaponPivot.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    IEnumerator PerformAttackPattern()
    {
        AttackPattern pattern = attackPatterns[currentPatternIndex];

        switch (pattern.type)
        {
            case AttackType.CircleShot:
                CircleAttack(pattern.projectileCount);
                break;

            case AttackType.Beam:
                yield return StartCoroutine(BeamAttack());
                break;

            case AttackType.TargetedBigProjectile:
                LaunchBigProjectile();
                break;
        }

        currentPatternIndex = (currentPatternIndex + 1) % attackPatterns.Length;
    }

    void CircleAttack(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = i * (360f / count);
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
            GameObject projectile = Instantiate(
                projectilePrefab,
                weaponPivot.position,
                Quaternion.identity
            );
            projectile.GetComponent<Rigidbody2D>().velocity = dir * 5f;
        }
    }

    IEnumerator BeamAttack()
    {
        GameObject beamWarning = Instantiate(
            beamWarningPrefab,
            weaponPivot.position,
            weaponPivot.rotation
        );
        yield return new WaitForSeconds(beamChargeTime);
        Destroy(beamWarning);

        GameObject beam = Instantiate(
            beamPrefab,
            weaponPivot.position,
            weaponPivot.rotation
        );
        Destroy(beam, 2f);
    }

    void LaunchBigProjectile()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(
            bigProjectilePrefab,
            weaponPivot.position,
            Quaternion.identity
        );
        projectile.GetComponent<Rigidbody2D>().velocity = direction * 7f;
    }
}

[System.Serializable]
public class AttackPattern
{
    public AttackType type;
    public int projectileCount;
}

public enum AttackType
{
    CircleShot,
    Beam,
    TargetedBigProjectile
}
