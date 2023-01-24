using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupInstance : MonoBehaviour
{
    [SerializeField] private WeaponScriptableObject weaponGift;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if ( other.transform.parent != null && other.transform.parent.CompareTag("Player") )
        {
            var gameObjectPlayer = other.transform.parent;
            if (gameObjectPlayer.TryGetComponent<PlayerInstance>(out var playerInstance))
            {
                playerInstance.GiveWeapon(weaponGift);
                Destroy(gameObject);
            }
        }
    }
}
