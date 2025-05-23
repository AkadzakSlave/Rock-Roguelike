using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance;

    [Header("Основные элементы")]
    public GameObject _questPanel;
    public TextMeshProUGUI _titleText;
    public TextMeshProUGUI _descriptionText;
    public TextMeshProUGUI _progressText;
    public TextMeshProUGUI _statusText;
    public Button _closeButton;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (_closeButton != null)
        {
            _closeButton.onClick.AddListener(() => _questPanel.SetActive(false));
        }

        _questPanel.SetActive(false);
    }

    public void UpdateQuestDisplay(Quest quest)
    {
        if (_questPanel == null || _titleText == null || _descriptionText == null || 
            _progressText == null || _statusText == null)
        {
            Debug.LogError("QuestUI elements are not properly assigned!");
            return;
        }

        if (quest == null || quest.data == null)
        {
            _questPanel.SetActive(false);
            return;
        }

        _questPanel.SetActive(true);
        _titleText.text = quest.data.title;
        _descriptionText.text = quest.data.description;
        
        if (quest.isCompleted)
        {
            _progressText.gameObject.SetActive(false);
            _statusText.text = "Готово к сдаче!";
            _statusText.color = Color.green;
        }
        else
        {
            _progressText.gameObject.SetActive(true);
            _statusText.text = "В процессе";
            _statusText.color = Color.yellow;
            _progressText.text = GetProgressText(quest);
        }
    }

    private string GetProgressText(Quest quest)
    {
        if (quest.data == null) return "";

        return quest.data.type switch
        {
            QuestData.QuestType.KillEnemies => 
                $"Убито: {quest.currentProgress}/{quest.data.targetCount}",
            QuestData.QuestType.DefeatBoss => 
                $"Босс: {quest.data.bossID} {(quest.isCompleted ? "Побеждён" : "Не побеждён")}",
            QuestData.QuestType.CompleteRooms => 
                $"Комнат пройдено: {quest.currentProgress}/{quest.data.targetCount}",
            _ => ""
        };
    }
}