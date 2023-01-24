using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenuCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        LevelManager.eventSessionStart += () => {
            gameObject.SetActive(false);
        };

        LevelManager.eventEndgame += LevelManager_eventEndgame;
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
