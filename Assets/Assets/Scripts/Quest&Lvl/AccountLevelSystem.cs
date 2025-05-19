using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountLevelSystem : MonoBehaviour
{
    public int currentLevel = 1;
    public int currentXP;
    public static AccountLevelSystem Instance;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        int requiredXP = GetRequiredXP(currentLevel);
        while(currentXP >= requiredXP)
        {
            currentLevel++;
            currentXP -= requiredXP;
            requiredXP = GetRequiredXP(currentLevel);
        }
    }

    private int GetRequiredXP(int level)
    {
        return 100 * level * level;
    }
}
