using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/New Quest")]
public class QuestData : ScriptableObject
{
    public enum QuestType { KillEnemies, DefeatBoss, CompleteRooms }

    [Header("Основные настройки")]
    public string _questID;
    public string questID => _questID;
    public string title;
    [TextArea] public string description;
    public QuestType type;

    [Header("Цели")]
    public int targetCount = 1;
    public string targetEnemyID;
    public string bossID;

    [Header("Награды")]
    public int expReward;

    void OnValidate()
    {
        if (string.IsNullOrEmpty(_questID))
        {
            _questID = System.Guid.NewGuid().ToString();
        }

        if (type == QuestType.DefeatBoss && targetCount != 1)
        {
            targetCount = 1;
            Debug.LogWarning("DefeatBoss quests should have targetCount = 1. Auto-corrected.");
        }
    }
}
