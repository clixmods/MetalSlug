using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponentScore;
    [SerializeField] private HighscoreTable highscoreTable;

    private int _currentScore = 0;
    public int CurrentScore => _currentScore;
    // Start is called before the first frame update
    void Start()
    {
        AIInstance.eventGlobalAIScore += AddScore;
        LevelManager.eventEndgame += LevelManager_eventEndgame;
    }

    private void LevelManager_eventEndgame()
    {
        highscoreTable.RegisterScore(CurrentScore);
    }

    private void AddScore(int amount)
    {
        if (LevelManager.Instance.players.Count > 0)
        {
            _currentScore += amount;
            textComponentScore.text = "Score : " + _currentScore;
        }
        
    }
}