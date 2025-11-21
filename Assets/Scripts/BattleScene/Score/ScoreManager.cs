using UnityEngine;
using UnityEngine.UI;
using Game.Enemy;
using Game.Player;
public class ScoreManager : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] PlayerController player;
    int currentScore = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Initialize()
    {
        scoreText.text = currentScore.ToString();
        player.AddScoreAction += AddScore;
    }
    void AddScore(IEnemy targetEnemy)
    {
        var score = targetEnemy._enemyStatusData.Score;
        currentScore += score;
        scoreText.text = currentScore.ToString();
    }
}
