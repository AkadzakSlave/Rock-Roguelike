using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePersistentData : MonoBehaviour
{
    public static ScenePersistentData Instance;
    
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

    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
    }
}