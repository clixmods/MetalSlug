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
    [SerializeField] private Transform[] spawnPoint;
    [SerializeField] private GameObject[] enemies;
    private int _needToSpawnAmount = 0;
    private float _currentDelaySpawn = 0;
    private void OnValidate()
    {
        GetValues();
    }

    // Start is called before the first frame update
    void Awake()
    {
        _triggerBox.isTrigger = true;
        GetValues();
    }

    void GetValues()
    {
        _triggerBox = GetComponent<BoxCollider>();
        if (_triggerBox == null)
            _triggerBox = gameObject.AddComponent<BoxCollider>();

        spawnPoint = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoint[i] = transform.GetChild(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_needToSpawnAmount > 0)
        {
            if (_currentDelaySpawn > 0)
            {
                _currentDelaySpawn -= Time.deltaTime;
            }
            else
            {
                var spawnPointRandom = spawnPoint[Random.Range(0, spawnPoint.Length)];
                Instantiate(enemies[Random.Range(0, enemies.Length)], spawnPointRandom.position,Quaternion.identity ) ;
                _currentDelaySpawn = delaySpawn;
                _needToSpawnAmount--;
                if (_needToSpawnAmount == 0)
                {
                    gameObject.SetActive(false);
                }
            }
        }
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // for (int i = 0; i < numberOfEnemiesToSpawn; i++)
            // {
            //     foreach (var VARIABLE in spawnPoint)
            //     {
            //         if (i >= numberOfEnemiesToSpawn) break;
            //         i++;
            //         Instantiate(enemies[Random.Range(0, enemies.Length)], VARIABLE.position,Quaternion.identity ) ;
            //     }
            // }
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
        
    }
#endif
}
