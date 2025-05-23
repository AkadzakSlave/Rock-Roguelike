using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public GameObject deathScreen;
    public Transform respawnPoint; 
    public string hubSceneName = "Hub";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    public void ShowDeathScreen()
    {
        deathScreen.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void HideDeathScreen()
    {
        deathScreen.SetActive(false);
        Time.timeScale = 1f;
    }
    
    public void RespawnPlayer()
    {
        HideDeathScreen();

        var player = FindObjectOfType<Health>();
        if (player != null)
        {
            player.transform.position = respawnPoint.position;
            player.GetComponent<Health>().Heal(player.GetComponent<Health>().maxHealth);
            player.GetComponent<Collider2D>().enabled = true;
        }
    }

    public void ReturnToHub()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(hubSceneName);
        RespawnPlayer();
        
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        RespawnPlayer();
        HideDeathScreen();
    }
}
