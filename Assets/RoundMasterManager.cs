using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class RoundMasterManager : MonoBehaviour
{
    [SerializeField] private List<RoundManager> _roundManagers;
    [SerializeField] private GameObject[] bossEnemies;
    
    private CameraMotor _cameraMotor;
    private void Awake()
    {
        GetValues();
        _cameraMotor = FindObjectOfType<CameraMotor>();
        LevelManager.eventPostLevelRestart += PostLevelManagerOneventPostLevelRestart;
    }

    private void OnValidate()
    {
        GetValues();
    }

    void GetValues()
    {
        _roundManagers = FindObjectsOfType<RoundManager>().ToList();
    }
    
    private void PostLevelManagerOneventPostLevelRestart()
    {
        int roundBoss = Random.Range(1, _roundManagers.Count);
        var boss = _roundManagers[roundBoss].SpawnBoss(bossEnemies[Random.Range(0, bossEnemies.Length)],false, true);
        boss.eventAIDeath += BossOneventAIDeath;
        _cameraMotor.rightBoundary = boss.transform.position.x -10;
    }

   
  
    private void BossOneventAIDeath(AIInstance aiinstance)
    {
        _cameraMotor.ResetCamera(false, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
