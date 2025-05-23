using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Настройки")]
    public string mainMenuScene = "MainMenu";
    public string HubScene = "Hub";
    public KeyCode pauseKey = KeyCode.Escape;
    
    [Header("UI")]
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button menuButton;
    public Button quitButton;
    public Button HubButton;

    private bool isPaused = false;

    void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        menuButton.onClick.AddListener(ReturnToMenu);
        quitButton.onClick.AddListener(QuitGame);
        HubButton.onClick.AddListener(ReturnToHub);
        
        pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; 
        pauseMenuUI.SetActive(true); 
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; 
        pauseMenuUI.SetActive(false); 
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene); 
        
    }
    public void ReturnToHub()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(HubScene);
    }

    public void QuitGame()
    {
        Application.Quit();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}