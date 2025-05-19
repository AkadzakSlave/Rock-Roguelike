using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSave : MonoBehaviour
{
    public static CanvasSave Instance;
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
}
