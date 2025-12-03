using UnityEngine;

[System.Serializable]
public class SaveData
{
    [SerializeField] int _highScore;
    [SerializeField] string _highestDateTime;
    public int highScore { get => _highScore;set => _highScore = value;  }
    public string highestDateTime { get => _highestDateTime;set => _highestDateTime = value; }
}
