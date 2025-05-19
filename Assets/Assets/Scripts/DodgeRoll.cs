using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeRoll : MonoBehaviour
{
    public float rollDistance = 2f;
    public float rollDuration = 0.3f;
    public float invincibilityDuration = 0.5f;
    
    private bool isRolling = false;
    private Health health;

    void Start()
    {
        health = GetComponent<Health>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling)
        {
            StartCoroutine(Roll());
        }
    }

    IEnumerator Roll()
    {
        isRolling = true;
        health.isInvincible = true;
        
        Vector2 rollDirection = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;
        
        float elapsed = 0;
        while (elapsed < rollDuration)
        {
            transform.Translate(rollDirection * (rollDistance/rollDuration) * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        yield return new WaitForSeconds(invincibilityDuration - rollDuration);
        health.isInvincible = false;
        isRolling = false;
    }
}
