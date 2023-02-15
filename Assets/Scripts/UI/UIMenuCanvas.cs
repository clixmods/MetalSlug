using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenuCanvas : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private CanvasGroup _canvasGroupTitle;
    [SerializeField] private CanvasGroup _canvasGroupHighscore;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        LevelManager.eventSessionStart += () => {
            gameObject.SetActive(false);
        };

        LevelManager.eventEndgame += LevelManager_eventEndgame;
        LevelManager.EventOnStateChanged += LevelManagerOnEventOnStateChanged;
    }

    private void LevelManagerOnEventOnStateChanged(State state)
    {
        switch (state)
        {
            case State.InGame:
                _animator.enabled = false;
                _animator.playbackTime = 0;
                break;
            case State.Intermission:
                break;
            case State.GameOver:
                _canvasGroupTitle.alpha = 0;
                _canvasGroupHighscore.alpha = 1;
                break;
            case State.Menu:
                _animator.enabled = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void LevelManager_eventEndgame()
    {
        gameObject.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
