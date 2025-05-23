using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Основные настройки")]
        public string firstLevelName = "Hub";

        [Header("Ссылки на UI")]
        public Button continueButton;
        public GameObject resetConfirmPanel;
        public GameObject loadingScreen;
        public Slider loadingProgressBar;

        void Start()
        {
            continueButton.interactable = PlayerPrefs.HasKey("CurrentQuestID");

            if (resetConfirmPanel != null)
                resetConfirmPanel.SetActive(false);

            if (loadingScreen != null)
                loadingScreen.SetActive(false);
        }
        public void OnNewGame()
        {
            ResetAllProgress();
            StartCoroutine(LoadSceneAsync(firstLevelName));
        }
        public void OnContinue()
        {
            if (PlayerPrefs.HasKey("CurrentQuestID"))
            {
                StartCoroutine(LoadSceneAsync(firstLevelName));
            }
            else
            {
                Debug.LogWarning("Нет сохранений для продолжения игры.");
            }
        }
        
        public void OnResetProgressClicked()
        {
            resetConfirmPanel.SetActive(true);
        }
        
        public void ConfirmReset()
        {
            ResetAllProgress();
            resetConfirmPanel.SetActive(false);
            continueButton.interactable = false;
        }
        
        public void CancelReset()
        {
            resetConfirmPanel.SetActive(false);
        }
        
        public void OnExit()
        {
            Application.Quit();
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            if (loadingScreen != null)
            {
                loadingScreen.SetActive(true);
            }
            else
            {
                Debug.LogError("Loading Screen не назначен в инспекторе!");
                yield break;
            }

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);

                if (loadingProgressBar != null)
                {
                    loadingProgressBar.value = progress;
                }

                if (operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }
        
        public static void ResetAllProgress()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Весь прогресс сброшен!");
        }
    }
}
