using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using UnityEngine.Serialization;

public class Health : MonoBehaviour
{
    [Header("Основные параметры")]
    [Tooltip("Максимальное количество здоровья")]
    public int maxHealth = 100;
    [Tooltip("Текущее количество здоровья")]
    public int currentHealth;
    [Tooltip("Является ли объект игроком?")]
    public bool isPlayer = false;
    [Tooltip("Может ли получать урон сейчас?")]
    public bool isInvincible = false;
    [Tooltip("Длительность неуязвимости после урона")]
    public float invincibilityDuration = 0.5f;

    [Header("Визуальные эффекты")]
    public GameObject deathEffect;
    public AudioClip hurtSound;
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;

    [Header("Полоска здоровья")]
    public Slider healthBar;
    public float healthBarSmoothSpeed = 5f;
    private float _targetHealthValue;
    
    [Tooltip("Событие, вызываемое при смерти")]
    public event Action OnDeath;
    public string enemyType;
    [Header("Quest Settings")]
    [SerializeField] private string enemyID = "Enemy_Skeleton";
    [Header("Экран смерти")]
    public GameObject deathScreen; 
    public Button ContButton;
    public Button exitButton; 
    public Vector2 spawnPositionInNewScene = Vector2.zero;
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private AudioSource audioSource;

    void Start()
    {
        InitializeHealth();
    }

    void InitializeHealth()
    {
        currentHealth = maxHealth;
        TryGetComponent(out audioSource);
        TryGetComponent(out spriteRenderer);

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        UpdateHealthBar();
    }

    void Update()
    {
        SmoothHealthBar();
    }

    // ========== ОСНОВНЫЕ МЕТОДЫ ==========
    public virtual void TakeDamage(int damage)
    {
        if (ShouldIgnoreDamage()) return;

        ApplyDamage(damage);
        PlayDamageEffects();
        CheckDeath();
    }
    public bool IsFullHealth()
    {
        return currentHealth >= maxHealth;
    }
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHealthBar();
        Debug.Log($"Восстановлено {amount} здоровья");
    }
    // ========== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ==========
    private bool ShouldIgnoreDamage()
    {
        return isInvincible || currentHealth <= 0;
    }

    private void ApplyDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log($"{gameObject.name} получил {damage} урона! Осталось: {currentHealth}/{maxHealth}");
        UpdateHealthBar();

        if (isPlayer)
            StartCoroutine(InvincibilityCoroutine());
    }

    private void PlayDamageEffects()
    {
        if (spriteRenderer != null)
            StartCoroutine(FlashEffect());

        PlaySound(hurtSound);
    }

    private void CheckDeath()
    {
        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
            Die();
        }
        else
        {
            deathScreen.SetActive(false);
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
            _targetHealthValue = (float)currentHealth / maxHealth;
    }

    private void SmoothHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = Mathf.Lerp(
                healthBar.value, 
                _targetHealthValue, 
                Time.deltaTime * healthBarSmoothSpeed
            );
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        if (audioSource != null)
            audioSource.PlayOneShot(clip);
        else
            AudioSource.PlayClipAtPoint(clip, transform.position);
    }

    // ========== КОРУТИНЫ ==========
    private IEnumerator FlashEffect()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    // ========== ОБРАБОТКА СМЕРТИ ==========
    public virtual void Die()
    {
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        
        if (isPlayer)
        {
            HandlePlayerDeath();
        }
        else
        {
            Destroy(gameObject);
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.UpdateQuestProgress(
                    QuestData.QuestType.KillEnemies, 
                    enemyID);
            }
            else
            {
                Debug.LogWarning("QuestManager is missing!");
            }
            QuestManager.Instance.UpdateQuestProgress(QuestData.QuestType.KillEnemies, enemyType);
            Destroy(gameObject);
            Debug.Log("Враг умер!");
        }
    }

    private void HandlePlayerDeath()
    {
        Debug.Log("Игрок умер!");
        
        var playerController = GetComponent<PlayerController>();
        if (playerController != null) playerController.enabled = false;

        var rigidbody = GetComponent<Rigidbody2D>();
        if (rigidbody != null) rigidbody.velocity = Vector2.zero;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowDeathScreen();
        }
        else
        {
            Debug.LogError("GameManager не найден!");
            Invoke(nameof(ReloadLevel), 3f);
        }
    }

    private void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    #if UNITY_EDITOR
    void OnValidate()
    {
        if (Application.isPlaying) return;
        currentHealth = maxHealth;
    }
    #endif
}