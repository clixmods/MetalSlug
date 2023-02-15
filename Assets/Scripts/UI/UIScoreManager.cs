using System.Collections;
using System.Collections.Generic;
using Powerup;
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
        UIPointsPlusPanel.OnMultiplierSend += AddScore;
        LevelManager.eventEndgame += LevelManager_eventEndgame;
        LevelManager.eventResetSession += LevelManagerOneventResetSession;
        AIInstance.EventAISpawn += AIInstanceOnEventAISpawn;
        PowerupPoint.OnGrabPoint += OnPowerupPickup;
    }
    private void OnPowerupPickup(int value)
    {
        AddScore(value);
    }

    private void AIInstanceOnEventAISpawn(AIInstance aiinstance)
    {
        aiinstance.eventAIDeath += AiinstanceOneventAIDeath;
        aiinstance.eventAIHit += AiinstanceOneventAIHit;
    }

    private void AiinstanceOneventAIHit(AIInstance aiinstance)
    {
        AddScore(aiinstance.AIData.ScoreHit);
    }

    private void AiinstanceOneventAIDeath(AIInstance aiinstance)
    {
        AddScore(aiinstance.AIData.ScoreDead);
    }

    private void LevelManagerOneventResetSession()
    {
        _currentScore = 0;
        textComponentScore.text = "Score : " + _currentScore;
    }

    private void LevelManager_eventEndgame()
    {
        highscoreTable.RegisterScore(CurrentScore);
    }

    public void AddScore(int amount)
    {
        if (LevelManager.Players.Count > 0)
        {
            _currentScore += amount;
            textComponentScore.text = "Score : " + _currentScore;
            UIPointsPlusPanel.CreateUIPointsPlus(FindObjectOfType<Canvas>().gameObject, transform.position, amount);
        }
    }
}