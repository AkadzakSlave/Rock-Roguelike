using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (PlayerPrefs.HasKey("SpawnPosX"))
        {
            float x = PlayerPrefs.GetFloat("SpawnPosX");
            float y = PlayerPrefs.GetFloat("SpawnPosY");
            player.transform.position = new Vector2(x, y);
            
            
            PlayerPrefs.DeleteKey("SpawnPosX");
            PlayerPrefs.DeleteKey("SpawnPosY");
        }
    }
}