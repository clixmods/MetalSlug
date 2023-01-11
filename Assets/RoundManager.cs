using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider))]
public class RoundManager : MonoBehaviour
{
    private Collider _triggerBox;
    public bool IsSpawned;
    [SerializeField] private int numberOfEnemiesToSpawn = 5;
    [SerializeField] private float delaySpawn = 1;
    [SerializeField] private List<Transform> spawnPoint = new List<Transform>();
    [SerializeField] private List<Transform> bossSpawnPoint = new List<Transform>();
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject roundBlocker;
    
    private int _needToSpawnAmount = 0;
    private float _currentDelaySpawn = 0;
    public bool IsRoundBoss;
    private bool _noSpawn ;
    
    private void OnValidate()
    {
        GetValues();
    }

    // Start is called before the first frame update
    void Awake()
    {
        GetValues();
        roundBlocker.SetActive(false);
        _triggerBox.isTrigger = true;
        LevelManager.eventPreLevelRestart += PostLevelManagerOneventPostLevelRestart;
    }

    private void PostLevelManagerOneventPostLevelRestart()
    {
        _noSpawn = false;
        IsRoundBoss = false;
        roundBlocker.SetActive(false);
        IsSpawned = false;
    }

    void GetValues()
    {
        _triggerBox = GetComponent<BoxCollider>();
        if (_triggerBox == null)
            _triggerBox = gameObject.AddComponent<BoxCollider>();

        spawnPoint = new List<Transform>();
        bossSpawnPoint = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var tempSpawnPoint = transform.GetChild(i);
            if (tempSpawnPoint.CompareTag("BossSpawnPoint"))
                bossSpawnPoint.Add( transform.GetChild(i));
            else if(tempSpawnPoint.CompareTag("RoundBlocker"))
            {
                roundBlocker = tempSpawnPoint.gameObject;
            }
            else
            {
                spawnPoint.Add(transform.GetChild(i));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.Instance.State == State.Intermission)
            _needToSpawnAmount = 0;
        
        if(_noSpawn) IsSpawned = true;
        if ( _needToSpawnAmount > 0 && !IsSpawned )
        {
            if (_currentDelaySpawn > 0)
            {
                _currentDelaySpawn -= Time.deltaTime;
            }
            else
            {
                var spawnPointRandom = spawnPoint[Random.Range(0, spawnPoint.Count)];
                Instantiate(enemies[Random.Range(0, enemies.Length)], spawnPointRandom.position,Quaternion.identity ) ;
                _currentDelaySpawn = delaySpawn;
                _needToSpawnAmount--;
                if (_needToSpawnAmount <= 0)
                {
                    IsSpawned = true;
                }
            }
        }
        
        
    }

    public AIInstance SpawnBoss(GameObject prefabBoss, bool bossIsAlone, bool useRoundBlocker)
    {
        var pointRandom = bossSpawnPoint[Random.Range(0, bossSpawnPoint.Count)];
        var bossObject = Instantiate(prefabBoss, pointRandom.position,Quaternion.identity ) ;
        if (bossIsAlone)
        {
            _noSpawn = true;
        }

        if (useRoundBlocker)
        {
            roundBlocker.SetActive(true);
        }

        var aiinstance = bossObject.GetComponent<AIInstance>();
        aiinstance.eventAIDeath += instance =>
        {
            roundBlocker.SetActive(false);
        };
        return aiinstance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _needToSpawnAmount = numberOfEnemiesToSpawn;
            foreach (var player in LevelManager.Instance.players)
            {
                if (!player.gameObject.activeSelf)
                {
                    player.gameObject.SetActive(true);
                    player.Teleport(LevelManager.GetRandomAlivePlayers.transform.position);
                }
            }
          
            
        }
        
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        
            foreach (var VARIABLE in spawnPoint)
            {
                Debug.DrawLine(transform.position, VARIABLE.position, Color.magenta);
            }

            foreach (var VARIABLE in bossSpawnPoint)
            {
                Debug.DrawLine(transform.position, VARIABLE.position, Color.red);
            }

    }
#endif
}
