using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponentScore;

    
    private int _currentScore = 0;
    public int CurrentScore => _currentScore;
    // Start is called before the first frame update
    void Start()
    {
        AIInstance.eventGlobalAIScore += AddScore;
    }

    private void AddScore(int amount)
    {
        _currentScore += amount;
        textComponentScore.text = "Score : " + _currentScore;
    }
}