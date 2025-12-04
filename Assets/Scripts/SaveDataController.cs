using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

public class SaveDataController
{
    const string fileName = "saveData.json";
    string filePath => Path.Combine(Application.persistentDataPath, fileName);

    public void SaveData(int currentScore)
    {
        var saveData = new SaveData();
        var highScore = -1;
        if (File.Exists(filePath))
        {
            var previousJson = File.ReadAllText(filePath);
            var previousData = JsonUtility.FromJson<SaveData>(previousJson);
            var previousHighScore = previousData.highScore;
            if (currentScore > previousHighScore)
            {
                highScore = currentScore;
                saveData.highestDateTime = DateTime.Now.ToString("O");
            }
            else
            {
                highScore = previousHighScore;
                saveData.highestDateTime = previousData.highestDateTime != null
                                           ? previousData.highestDateTime
                                           : DateTime.Now.ToString("O");
            }
        }
        else
        {
            highScore = currentScore;
            saveData.highestDateTime = DateTime.Now.ToString("O");
        }
        saveData.highScore = highScore;
        var json = JsonUtility.ToJson(saveData);
        File.WriteAllText(filePath, json);
    }
    public (int highScore,DateTime highestDateTime) LoadData()
    {
        if (!File.Exists(filePath)) return (-1,default);
        var json = File.ReadAllText(filePath);
        var data = JsonUtility.FromJson<SaveData>(json);
        return (data.highScore,DateTime.Parse(data.highestDateTime));
    }
}
