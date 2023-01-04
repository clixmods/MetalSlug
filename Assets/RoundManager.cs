using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider))]
public class RoundManager : MonoBehaviour
{
    private Collider _triggerBox;
    private void OnValidate()
    {
        _triggerBox = GetComponent<BoxCollider>();
        if (_triggerBox == null)
            _triggerBox = gameObject.AddComponent<BoxCollider>();
    }

    // Start is called before the first frame update
    void Start()
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
        }
        
    }
}
