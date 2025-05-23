using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TrialRoomController : MonoBehaviour
{
    [Header("Основные настройки")]
    public List<DoorController> roomDoors;
    public List<Health> roomEnemies;
    public Collider2D roomTrigger;

    [Header("Настройки босса")]
    public bool hasBoss = false;
    public BossHealth boss;
    public Slider bossHealthSlider;
    public Text bossNameText;
    public GameObject bossHealthPanel;

    [Header("Опции")]
    public bool debugLogs = true;
    public bool deactivateEnemiesInitially = true;

    private bool _isActive = false;
    private int _aliveEnemies;

    void Start()
    {
        InitializeRoom();
    }

    void InitializeRoom()
    {   
        roomEnemies.RemoveAll(enemy => enemy == null);
        _aliveEnemies = roomEnemies.Count;
        
        if(hasBoss && boss != null)
        {
            boss.gameObject.SetActive(false);
            boss.OnDeath += OnBossDied;
            boss.OnHealthChanged += UpdateBossHealth;
            
            UpdateBossHealth(Int32.MaxValue, 1000);
            if(bossNameText != null)
                bossNameText.text = boss.gameObject.name;
            
            ToggleBossUI(false);
        }

        foreach (var enemy in roomEnemies)
        {
            enemy.OnDeath += OnEnemyDied;
            if(deactivateEnemiesInitially)
                enemy.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!_isActive && other.CompareTag("Player"))
        {
            StartTrial();
        }
    }

    void StartTrial()
    {
        _isActive = true;
        
        if(hasBoss && boss != null)
        {
            boss.gameObject.SetActive(true);
            ToggleBossUI(true);
        }
        
        foreach(var enemy in roomEnemies)
        {
            if(enemy != null)
                enemy.gameObject.SetActive(true);
        }
        
        foreach(var door in roomDoors)
        {
            if(door != null)
                door.LockDoor();
        }

        if(debugLogs)
            Debug.Log($"Испытание началось! Врагов: {_aliveEnemies}" + 
                    (hasBoss ? " + Босс" : ""));
    }

    void UpdateBossHealth(int currentHealth, int maxHealth)
    {
        if(bossHealthSlider != null)
        {
            bossHealthSlider.maxValue = maxHealth;
            bossHealthSlider.value = currentHealth;
        }
    }

    void ToggleBossUI(bool state)
    {
        if(bossHealthPanel != null)
            bossHealthPanel.SetActive(state);
    }

    void OnEnemyDied()
    {
        _aliveEnemies--;

        if(debugLogs)
            Debug.Log($"Враг убит. Осталось: {_aliveEnemies}");

        CheckCompletion();
    }

    void OnBossDied()
    {
        if(debugLogs)
            Debug.Log("Босс повержен!");
            
        ToggleBossUI(false);
        CheckCompletion();
    }

    void CheckCompletion()
    {
        bool enemiesDead = _aliveEnemies <= 0;
        bool bossDead = !hasBoss || (boss != null && boss);

        if(enemiesDead && bossDead)
        {
            EndTrial();
        }
    }

    void EndTrial()
    {
        foreach(var door in roomDoors)
        {
            if(door != null)
                door.UnlockDoor();
        }

        if(debugLogs)
            Debug.Log("Испытание пройдено! Двери открыты.");
        QuestManager.Instance.UpdateQuestProgress(
            QuestData.QuestType.CompleteRooms, 
            ""
        );
    }

    // Для динамического добавления врагов
    public void AddEnemy(Health enemy)
    {
        if(!roomEnemies.Contains(enemy))
        {
            roomEnemies.Add(enemy);
            _aliveEnemies++;
            enemy.OnDeath += OnEnemyDied;
            
            if(_isActive)
                enemy.gameObject.SetActive(true);
        }
    }

    #if UNITY_EDITOR
    void OnValidate()
    {
        if(roomTrigger == null)
            roomTrigger = GetComponent<Collider2D>();
    }
    #endif
}