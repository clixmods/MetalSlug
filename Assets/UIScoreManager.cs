using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponentScore;

    
    private int _currentScore = 0;
    // Start is called before the first frame update
    void Start()
    {
        AIInstance.eventAIScore += AddScore;
        
    }

    private void AddScore(int amount)
    {
        _currentScore += amount;
        textComponentScore.text = "Score : " + _currentScore;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
