using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Portal : MonoBehaviour
{
    [Header("Настройки сцены")]
    public string targetSceneName;
    public Vector2 spawnPositionInNewScene = Vector2.zero; // Новая переменная для позиции

    [Header("UI")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private TextMeshProUGUI interactionText;
    public string interactionMessage = "Нажмите Пробел";

    private bool canInteract = false;
    private GameObject player; // Ссылка на игрока

    void Start()
    {
        InitializeUIElements();
        player = GameObject.FindGameObjectWithTag("Player"); // Находим игрока при старте
    }

    void InitializeUIElements()
    {
        if (interactionUI == null)
        {
            interactionUI = GameObject.FindGameObjectWithTag("InteractionUI");
            if (interactionUI == null)
            {
                Debug.LogError("InteractionUI не найден!");
                return;
            }
        }

        if (interactionText == null && interactionUI != null)
        {
            interactionText = interactionUI.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (interactionUI != null) interactionUI.SetActive(false);
    }

    void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.Space))
        {
            LoadTargetScene();
        }
    }

    void LoadTargetScene()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("Название сцены не указано!");
            return;
        }

        // Сохраняем позицию для переноса в PlayerPrefs
        PlayerPrefs.SetFloat("SpawnPosX", spawnPositionInNewScene.x);
        PlayerPrefs.SetFloat("SpawnPosY", spawnPositionInNewScene.y);
        PlayerPrefs.Save();

        SceneManager.LoadScene(targetSceneName);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            if (interactionUI != null)
            {
                interactionUI.SetActive(true);
                if (interactionText != null) interactionText.text = interactionMessage;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
            if (interactionUI != null) interactionUI.SetActive(false);
        }
    }
}