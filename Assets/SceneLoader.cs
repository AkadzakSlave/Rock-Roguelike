using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Настройки")]
    public float loadingDelay = 1f;
    
    [Header("События")]
    public UnityEngine.Events.UnityEvent OnSceneStartLoading;

    public void LoadScene(int buildIndex)
    {
        StartCoroutine(LoadSceneAsync(buildIndex));
    }

    private System.Collections.IEnumerator LoadSceneAsync(int buildIndex)
    {
        OnSceneStartLoading?.Invoke();
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex);
        asyncLoad.allowSceneActivation = false;

        float timer = 0;
        while(timer < loadingDelay || asyncLoad.progress < 0.9f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
    }
}
