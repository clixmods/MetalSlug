using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioAliase;
using UnityEngine;
using Random = UnityEngine.Random;


public class RoundMasterManager : MonoBehaviour
{
    [SerializeField] private List<RoundManager> _roundManagers;
    [SerializeField] private GameObject[] bossEnemies;
    
    private CameraMotor _cameraMotor;
    [Header("Aliases")]
    [SerializeField][Aliase] private string RoundBossStart;
    [SerializeField][Aliase] private string RoundBossEnd;
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
        
        _roundManagers[roundBoss].eventRoundTriggered += OneventRoundTriggered;
    }

    private void OneventRoundTriggered(RoundManager roundmanager)
    {
        AudioManager.PlaySoundAtPosition(RoundBossStart);
        roundmanager.eventRoundTriggered -= OneventRoundTriggered;
    }


    private void BossOneventAIDeath(AIInstance aiinstance)
    {
        _cameraMotor.ResetCamera(false, true);
        AudioManager.PlaySoundAtPosition(RoundBossEnd);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
