using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Quest
{
    public QuestData data;
    public int currentProgress;
    public bool isCompleted;
    public bool isTurnedIn;

    public Quest(QuestData data)
    {
        if (data == null)
        {
            Debug.LogError("Quest data is null!");
            return;
        }

        this.data = data;
        currentProgress = 0;
        isCompleted = false;
        isTurnedIn = false;
    }

    public void UpdateProgress(int amount = 1)
    {
        if (!isCompleted && data != null)
        {
            currentProgress = Mathf.Clamp(currentProgress + amount, 0, data.targetCount);
            if (currentProgress >= data.targetCount) 
            {
                isCompleted = true;
                Debug.Log($"Quest {data.questID} completed!");
            }
        }
    }
}

