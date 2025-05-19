using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 12f;
    public float speedBoostMultiplier = 1.5f;
    public float speedBoostDuration = 3f;
    public float speedBoostCooldown = 5f;
    
    [Header("Controls")]
    public KeyCode RunKey = KeyCode.LeftShift;
    
    [Header("Ability UI")]
    public Image cooldownRadial; 
    public GameObject cooldownUI; 

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float originalSpeed;
    private bool isSpeedBoostActive = false;
    private float speedBoostTimer;
    private float cooldownTimer;

    [Header("Combat")]
    public Transform weaponPivot;
    private Vector2 aimDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalSpeed = moveSpeed;
        InitializeAbilityUI();
    }

    void InitializeAbilityUI()
    {
        if(cooldownRadial != null)
        {
            cooldownRadial.type = Image.Type.Filled;
            cooldownRadial.fillMethod = Image.FillMethod.Radial360;
            cooldownRadial.fillOrigin = (int)Image.Origin360.Top; 
            cooldownRadial.fillClockwise = true;
            cooldownRadial.fillAmount = 0;
        }
        ToggleCooldownUI(false);
    }

    void Update()
    {
        HandleMovementInput();
        HandleAim();
        HandleSpeedBoost();
        UpdateAbilityUI();
    }

    void HandleMovementInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();
    }

    void HandleAim()
    {
        aimDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        weaponPivot.rotation = Quaternion.Euler(0, 0, angle);
    }

    void HandleSpeedBoost()
    {
        if(Input.GetKeyDown(RunKey) && !isSpeedBoostActive && cooldownTimer <= 0)
        {
            ActivateSpeedBoost();
        }

        if(isSpeedBoostActive)
        {
            speedBoostTimer -= Time.deltaTime;
            if(speedBoostTimer <= 0)
            {
                DeactivateSpeedBoost();
            }
        }
        else if(cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    void ActivateSpeedBoost()
    {
        isSpeedBoostActive = true;
        moveSpeed *= speedBoostMultiplier;
        speedBoostTimer = speedBoostDuration;
        ToggleCooldownUI(true);
    }

    void DeactivateSpeedBoost()
    {
        isSpeedBoostActive = false;
        moveSpeed = originalSpeed;
        cooldownTimer = speedBoostCooldown;
    }

    void UpdateAbilityUI()
    {
        if(cooldownRadial == null || cooldownUI == null) return;

        if(isSpeedBoostActive)
        {
            cooldownRadial.fillAmount = speedBoostTimer / speedBoostDuration;
        }
        else
        {
            cooldownRadial.fillAmount = cooldownTimer / speedBoostCooldown;
            ToggleCooldownUI(cooldownTimer > 0);
        }
    }
    
    void ToggleCooldownUI(bool state)
    {
        cooldownRadial.fillClockwise = false;
        cooldownUI.SetActive(state);
        
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput * moveSpeed;
    }
}