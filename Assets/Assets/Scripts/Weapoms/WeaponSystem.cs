using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSystem : MonoBehaviour
{
    [Header("Ссылки на оружие")]
    public GameObject sword;
    public GameObject gun;
    
    [Header("UI Элементы")]
    public Image weaponIcon; 
    public Sprite swordIcon; 
    public Sprite gunIcon; 
    public Image cooldownRadial;
    
    [Header("Настройки")]
    public KeyCode switchKey = KeyCode.Alpha1;
    public float switchCooldown = 0.5f;
    
    private bool _isSwordActive = true;
    private float _lastSwitchTime;
    private bool _isOnCooldown;

    void Start()
    {
        SetWeaponActive(true, false);
        UpdateWeaponIcon(); 
        
        if (cooldownRadial != null)
        {
            cooldownRadial.type = Image.Type.Filled; 
            cooldownRadial.fillMethod = Image.FillMethod.Radial360;
            cooldownRadial.fillAmount = 0; 
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            TrySwitchWeapon();
        }
        UpdateCooldownUI(); 
    }

    void TrySwitchWeapon()
    {
        if (Time.time < _lastSwitchTime + switchCooldown) return;
        
        _isSwordActive = !_isSwordActive;
        SetWeaponActive(_isSwordActive, !_isSwordActive);
        _lastSwitchTime = Time.time;
        UpdateWeaponIcon();
    }

    void UpdateCooldownUI()
    {
        if (cooldownRadial == null) return;
        
        float cooldownLeft = _lastSwitchTime + switchCooldown - Time.time;
        float fillValue = 1f - Mathf.Clamp01(cooldownLeft / switchCooldown);
        cooldownRadial.fillAmount = fillValue;
        cooldownRadial.enabled = (fillValue > 0 && fillValue < 1);
    }

    void SetWeaponActive(bool swordState, bool pistolState)
    {
        if (sword != null) sword.SetActive(swordState);
        if (gun != null) gun.SetActive(pistolState);
        
        if (swordState) SetupSword();
        else SetupPistol();
    }
    
    void UpdateWeaponIcon()
    {
        if (weaponIcon == null) return;
        
        weaponIcon.sprite = _isSwordActive ? swordIcon : gunIcon;
        weaponIcon.preserveAspect = true; 
    }

    void SetupSword()
    {
        GetComponent<PlayerController>().moveSpeed = 12f;
    }

    void SetupPistol()
    {
        GetComponent<PlayerController>().moveSpeed = 15f;
    }

    public bool IsSwordActive()
    {
        return _isSwordActive;
    }
}
