using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string savePath => $"{Application.persistentDataPath}/save.json";

    [System.Serializable]
    public class SaveData
    {
        public int accountLevel;
        public int accountXP;
        public int currentSceneIndex;
        // Добавьте другие данные
    }

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
    }

    public static SaveData Load()
    {
        if(!File.Exists(savePath)) return null;
        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool SaveExists() => File.Exists(savePath);
    public static void DeleteSave() => File.Delete(savePath);
}
