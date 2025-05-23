using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealSystem : MonoBehaviour
{
    [Header("Настройки лечения")]
    public KeyCode healKey = KeyCode.E;
    [Range(0, 100)] private float healPercent = 30f;
    public float cooldown = 10f;
    public Color healColor = new Color(0.2f, 1f, 0.2f);
    public float flashDuration = 0.5f;

    [Header("Визуальные элементы")]
    public Image cooldownRadial;
    public GameObject cooldownUI; 
    public AudioClip healSound;
    public ParticleSystem healVFX;

    private Health _health;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private bool _isReady = true;
    private AudioSource _audioSource;

    void Start()
    {
        _health = GetComponent<Health>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        TryGetComponent(out _audioSource);

        if (_spriteRenderer != null)
            _originalColor = _spriteRenderer.color;

        ResetCooldownUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(healKey) && _isReady && !_health.IsFullHealth())
        {
            StartCoroutine(HealRoutine());
        }
    }

    IEnumerator HealRoutine()
    {
        int healAmount = Mathf.RoundToInt(_health.maxHealth * (healPercent / 100f));
        _health.Heal(healAmount);
        
        FlashCharacter();
        PlayHealSound();
        PlayVFX();
        
        _isReady = false;
        cooldownUI.SetActive(true);
        float timer = 0;

        while (timer < cooldown)
        {
            timer += Time.deltaTime;
            cooldownRadial.fillAmount = timer / cooldown;
            yield return null;
        }

        _isReady = true;
        ResetCooldownUI();
    }

    void FlashCharacter()
    {
        if (_spriteRenderer != null)
        {
            StartCoroutine(FlashRoutine());
        }
    }

    IEnumerator FlashRoutine()
    {
        _spriteRenderer.color = healColor;
        yield return new WaitForSeconds(flashDuration);
        _spriteRenderer.color = _originalColor;
    }

    void PlayHealSound()
    {
        if (healSound != null)
        {
            if (_audioSource != null)
                _audioSource.PlayOneShot(healSound);
            else
                AudioSource.PlayClipAtPoint(healSound, transform.position);
        }
    }

    void PlayVFX()
    {
        if (healVFX != null)
        {
            Instantiate(healVFX, transform.position, Quaternion.identity);
        }
    }

    void ResetCooldownUI()
    {
        if (cooldownRadial != null)
            cooldownRadial.fillAmount = 0;
        
        if (cooldownUI != null)
            cooldownUI.SetActive(false);
    }
}
