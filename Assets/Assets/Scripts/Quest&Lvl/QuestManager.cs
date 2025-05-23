using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    
    [Header("Настройки")]
    public List<QuestData> allQuests;
    
    
    [Header("Настройки")]
    public List<QuestData> _allQuests = new List<QuestData>();
    
    
    public Quest CurrentQuest { get; private set; }
    public event System.Action<Quest> OnQuestUpdated;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
    }

    void CleanDuplicateEventSystems()
    {
        var eventSystems = FindObjectsOfType<EventSystem>();
        if (eventSystems.Length > 1)
        {
            for (int i = 1; i < eventSystems.Length; i++)
            {
                Destroy(eventSystems[i].gameObject);
            }
        }
    }
    
    public void AcceptQuest(QuestData questData)
    {
        CurrentQuest = new Quest(questData);
        
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.currentQuestID = questData.questID;
        }
        UpdateQuestUI();
        OnQuestUpdated?.Invoke(CurrentQuest);
        PlayerPrefs.SetString("CurrentQuestID", questData.questID);
        PlayerPrefs.SetInt("CurrentQuestProgress", 0);
    }
    public void TurnInQuest()
    {
        if (CurrentQuest == null) return;
    
        CurrentQuest.isTurnedIn = true;
    
        if (AccountLevelSystem.Instance != null)
        {
            AccountLevelSystem.Instance.AddXP(CurrentQuest.data.expReward);
        }

        CurrentQuest = null;
    
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.currentQuestID = string.Empty;
        }

        UpdateQuestUI();
        OnQuestUpdated?.Invoke(null);
    }
    
    public void UpdateQuestProgress(QuestData.QuestType type, string targetID)
    {
        bool progressUpdated = false;
        string debugMessage = $"Checking quest: {CurrentQuest.data.title}, Type={type}, TargetID={targetID}";

        switch (type)
        {
            case QuestData.QuestType.KillEnemies:
                if (CurrentQuest.data.targetEnemyID == targetID)
                {
                    CurrentQuest.UpdateProgress();
                    progressUpdated = true;
                    debugMessage += $"\nKilled enemy: {targetID}";
                }
                break;
                
            case QuestData.QuestType.DefeatBoss:
                if (CurrentQuest.data.bossID == targetID)
                {
                    CurrentQuest.UpdateProgress();
                    progressUpdated = true;
                    debugMessage += $"\nDefeated boss: {targetID}";
                }
                break;
                
            case QuestData.QuestType.CompleteRooms:
                CurrentQuest.UpdateProgress();
                progressUpdated = true;
                debugMessage += $"\nCompleted room";
                break;
        }

        if (progressUpdated)
        {
            UpdateQuestUI();
            OnQuestUpdated?.Invoke(CurrentQuest);
        }
    }

    void UpdateQuestUI()
    {
        if (QuestUI.Instance != null)
        {
            QuestUI.Instance.UpdateQuestDisplay(CurrentQuest);
        }
    }

    public void ResetProgress()
    {
        CurrentQuest = null;
        OnQuestUpdated?.Invoke(null);
    
        PlayerPrefs.DeleteKey("CurrentQuestID");
        PlayerPrefs.DeleteKey("QuestProgress");
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateQuestUI();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}