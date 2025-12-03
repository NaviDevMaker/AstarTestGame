using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

public class SaveDataController
{
    const string fileName = "saveData.json";
    string filePath => Path.Combine(Application.persistentDataPath, fileName);

    //ämíËÇ≈ç≈Ç‡çÇÇ¢ílÇ™à¯êîÇ≈óàÇÈÇ©ÇÁÇ†Ç∆Ç≈íºÇµÇΩÇ©Ç¡ÇΩÇÁíºÇµÇƒ
    public void SaveData(int currentScore)
    {
        var saveData = new SaveData();
        var highScore = -1;
        if (File.Exists(filePath))
        {
            var previousJson = File.ReadAllText(filePath);
            var previousData = JsonUtility.FromJson<SaveData>(previousJson);
            var previousHighScore = previousData.highScore;
            highScore = Mathf.Max(currentScore, previousHighScore);
            if (highScore != previousHighScore) saveData.highestDateTime = DateTime.Now.ToString("O");
        }
        else highScore = currentScore;
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
