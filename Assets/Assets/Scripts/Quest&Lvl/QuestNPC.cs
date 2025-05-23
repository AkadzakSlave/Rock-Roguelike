using UnityEngine;
using System.Collections;

public class QuestNPC : MonoBehaviour
{
    [Header("Настройки")]
    public QuestData[] availableQuests;
    public int maxQuests = 3;
    
    [Header("Диалоги")]
    public Dialog startDialog;
    public Dialog completeDialog;
    public Dialog finalDialog;
    public Dialog reminderDialog; 
    
    private int _currentQuestIndex;
    private int _completedQuestsCount;
    private bool _allQuestsCompleted;
    private bool _canInteract;

    void Awake()
    {
        LoadNPCState();
    }

    void Update()
    {
        if (_canInteract && Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }

    private IEnumerator ShowQuestReminder()
    {
        if (QuestManager.Instance.CurrentQuest != null)
        {
            yield return DialogManager.Instance.ShowDialog(new Dialog {
                Lines = new[] { $"Вернись, когда выполнишь: {QuestManager.Instance.CurrentQuest.data.title}" }
            });
        }
        else
        {
            yield return DialogManager.Instance.ShowDialog(reminderDialog);
        }
    }
    public void Interact()
    {
        if (!_canInteract) return;

        if (_allQuestsCompleted)
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(finalDialog));
            return;
        }

        if (QuestManager.Instance.CurrentQuest == null)
        {
            StartCoroutine(StartNewQuest());
        }
        else if (QuestManager.Instance.CurrentQuest.isCompleted)
        {
            StartCoroutine(CompleteQuest());
        }
        else
        {
            StartCoroutine(ShowQuestReminder());
        }
    }


    private IEnumerator StartNewQuest()
    {
        yield return DialogManager.Instance.ShowDialog(startDialog);
        QuestManager.Instance.AcceptQuest(availableQuests[_currentQuestIndex]);
    }

    private IEnumerator CompleteQuest()
    {
        _completedQuestsCount++;
        SaveNPCState();

        if (_completedQuestsCount >= maxQuests)
        {
            _allQuestsCompleted = true;
            yield return DialogManager.Instance.ShowDialog(finalDialog);
            QuestManager.Instance.TurnInQuest();
            SaveNPCState();
            yield break;
        }

        yield return DialogManager.Instance.ShowDialog(completeDialog);
        QuestManager.Instance.TurnInQuest();
        
        _currentQuestIndex = (_currentQuestIndex + 1) % availableQuests.Length;
        QuestManager.Instance.AcceptQuest(availableQuests[_currentQuestIndex]);
        SaveNPCState();
    }

    private void SaveNPCState()
    {
        PlayerPrefs.SetInt(gameObject.name + "_completed", _completedQuestsCount);
        PlayerPrefs.SetInt(gameObject.name + "_index", _currentQuestIndex);
        PlayerPrefs.SetInt(gameObject.name + "_completedFlag", _allQuestsCompleted ? 1 : 0);
    }

    private void LoadNPCState()
    {
        _completedQuestsCount = PlayerPrefs.GetInt(gameObject.name + "_completed", 0);
        _currentQuestIndex = PlayerPrefs.GetInt(gameObject.name + "_index", 0);
        _allQuestsCompleted = PlayerPrefs.GetInt(gameObject.name + "_completedFlag", 0) == 1;
    }

    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteKey(gameObject.name + "_completed");
        PlayerPrefs.DeleteKey(gameObject.name + "_index");
        PlayerPrefs.DeleteKey(gameObject.name + "_completedFlag");
        
        _completedQuestsCount = 0;
        _currentQuestIndex = 0;
        _allQuestsCompleted = false;
    
        Debug.Log("Прогресс NPC сброшен!");
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _canInteract = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _canInteract = false;
    }
}