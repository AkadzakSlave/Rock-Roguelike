using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BossHealth : Health
{
    public event Action OnDeath;
    public event Action<int, int> OnHealthChanged;
    public string bossID; 

    public override void Die()
    {
        base.Die();
        OnDeath?.Invoke();
        
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.UpdateQuestProgress(
                QuestData.QuestType.DefeatBoss, 
                bossID); // Передаём ID босса
        }
        else
        {
            Debug.LogError("QuestManager не найден!");
        }
    }
}