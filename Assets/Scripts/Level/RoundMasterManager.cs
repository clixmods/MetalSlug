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
    [SerializeField] private RoundManager firstRoundManager;
    [SerializeField] private GameObject[] bossEnemies;
    
    private CameraMotor _cameraMotor;
    [Header("Aliases")]
    //[SerializeField][Aliase] private string RoundBossStart;
    //[SerializeField][Aliase] private string RoundBossEnd;
    
    [SerializeField][Aliase] private string musicBg;
    [SerializeField][Aliase] private string musicBgBoss;
    [SerializeField][Aliase] private string bossAnnouncer;
    private AudioPlayer _audioPlayerBgMusic;
    private AudioPlayer _audioPlayerBgBossMusic;
    private float cachedVolume;
    private void Awake()
    {
        GetValues();
        _cameraMotor = FindObjectOfType<CameraMotor>();
        LevelManager.eventPostLevelRestart += PostLevelManagerOneventPostLevelRestart;
        PlayerInstance.eventPlayerJoin += WaitPlayerToStart;
        LevelManager.eventResetSession += LevelManagerOneventResetSession;
        LevelManager.eventSessionStart  += LevelManagerOneventSessionStart;
    }

    private void LevelManagerOneventSessionStart()
    {
        if (_audioPlayerBgMusic == null)
        {
            AudioManager.PlayLoopSound(musicBg ,Vector3.zero, ref _audioPlayerBgMusic);
            cachedVolume = _audioPlayerBgMusic.Source.volume;
        }
            
    }

    private void LevelManagerOneventResetSession()
    {
        PlayerInstance.eventPlayerJoin += WaitPlayerToStart;
    }

    private void WaitPlayerToStart(PlayerInstance newplayer)
    {
        RoundManager.PlayerSpawnActive = firstRoundManager.PlayerSpawnPoints[0];
        AudioManager.StopLoopSound(ref _audioPlayerBgBossMusic, StopLoopBehavior.Direct);
        AudioManager.StopLoopSound(ref _audioPlayerBgMusic, StopLoopBehavior.Direct);
            
        PlayerInstance.eventPlayerJoin -= WaitPlayerToStart;
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
        AudioManager.PlayLoopSound(musicBg ,Vector3.zero, ref _audioPlayerBgMusic);
        cachedVolume = _audioPlayerBgMusic.Source.volume;
        int roundBoss = Random.Range(1, _roundManagers.Count);
        var boss = _roundManagers[roundBoss].SpawnBoss(bossEnemies[Random.Range(0, bossEnemies.Length)],false, true);
        boss.eventAIDeath += BossOneventAIDeath;
        _cameraMotor.rightBoundary = boss.transform.position.x -10;
        
        _roundManagers[roundBoss].EventRoundTriggered += OneventRoundTriggered;
    }

    private void OneventRoundTriggered(RoundManager roundmanager)
    {
        
        AudioManager.PlayLoopSound(musicBgBoss ,Vector3.zero, ref _audioPlayerBgBossMusic);
        AudioManager.PlaySoundAtPosition(bossAnnouncer);
        
        roundmanager.EventRoundTriggered -= OneventRoundTriggered;
    }


    private void BossOneventAIDeath(AIInstance aiinstance)
    {
        _cameraMotor.ResetCamera(false, true);
        //AudioManager.PlaySoundAtPosition(RoundBossEnd);
        AudioManager.StopLoopSound(ref _audioPlayerBgBossMusic, StopLoopBehavior.Direct);
        
       
    }

    // Update is called once per frame
    void Update()
    {
        if(LevelManager.Instance.State == State.InGame)
        {
            if (_audioPlayerBgBossMusic != null)
            {

                _audioPlayerBgMusic.Source.volume = 0;
            }
            else
            {
                if (_audioPlayerBgMusic != null)
                {
                    _audioPlayerBgMusic.Source.volume = cachedVolume;
                }

            }
        }
       
        
        
    }
}
