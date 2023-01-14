using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        LevelManager.eventSessionStart += LevelManager_eventSessionStart;
        LevelManager.eventEndgame += LevelManager_eventEndgame;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void LevelManager_eventEndgame()
    {
        gameObject.SetActive(false);
    }

    private void LevelManager_eventSessionStart()
    {
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
