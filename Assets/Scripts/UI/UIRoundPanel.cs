using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIRoundPanel : MonoBehaviour
{
    private const string StateName = "RoundMovement";
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    private Animator _animator;
    private bool playAnimation;

    private void Awake()
    {
        LevelManager.CallbackOnRoundChange += LevelManagerOnCallbackOnRoundChange;
        LevelManager.eventResetSession += LevelManagerOneventResetSession;
        _animator = GetComponent<Animator>();
        _textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void LevelManagerOneventResetSession()
    {
        _textMeshProUGUI.text = "1";
    }

    private void LevelManagerOnCallbackOnRoundChange(int newRound)
    {
        _animator.Play(StateName , 0);
        _textMeshProUGUI.text = newRound.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
