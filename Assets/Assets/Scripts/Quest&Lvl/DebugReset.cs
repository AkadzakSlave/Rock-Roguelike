using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugReset : MonoBehaviour
{
    public QuestNPC[] npcsToReset;

    public void Update()
    {
        ResetGameProgress();
    }

    public void ResetGameProgress()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
