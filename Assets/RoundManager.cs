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
    private bool _isSpawned;
    [SerializeField] private Transform[] spawnPoint;
    [SerializeField] private GameObject[] enemies;
    private void OnValidate()
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

    // Start is called before the first frame update
    void Awake()
    {
        _triggerBox.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Spawned struct AI");
            foreach (var VARIABLE in spawnPoint)
            {
                Instantiate(enemies[Random.Range(0, enemies.Length)], VARIABLE.position,Quaternion.identity ) ;
            }

            gameObject.SetActive(false);
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
