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
    [SerializeField] private List<QuestData> _allQuests = new List<QuestData>();
    
    
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
        
        Debug.Log("QuestManager initialized");
    }

    void InitializeCurrentQuest()
    {
        if (GameStateManager.Instance == null || string.IsNullOrEmpty(GameStateManager.Instance.currentQuestID))
        {
            Debug.Log("No saved quest found");
            return;
        }

        QuestData data = _allQuests.Find(q => q.questID == GameStateManager.Instance.currentQuestID);
        if (data != null) 
        {
            CurrentQuest = new Quest(data);
            Debug.Log($"Loaded quest: {data.title}");
            UpdateQuestUI();
        }
        else
        {
            Debug.LogWarning($"Quest data not found for ID: {GameStateManager.Instance.currentQuestID}");
        }
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
        if (questData == null)
        {
            Debug.LogError("Trying to accept null quest!");
            return;
        }

        if (CurrentQuest != null && !CurrentQuest.isCompleted)
        {
            Debug.LogWarning("Cannot accept new quest while current is active!");
            return;
        }

        CurrentQuest = new Quest(questData);
        
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.currentQuestID = questData.questID;
        }

        Debug.Log($"Accepted new quest: {questData.title}");
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
        if (CurrentQuest == null)
        {
            Debug.Log("No active quest to update");
            return;
        }

        if (CurrentQuest.isCompleted)
        {
            Debug.Log("Quest already completed");
            return;
        }

        if (CurrentQuest.data.type != type)
        {
            Debug.Log($"Quest type mismatch. Expected {CurrentQuest.data.type}, got {type}");
            return;
        }

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

        Debug.Log(debugMessage);

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