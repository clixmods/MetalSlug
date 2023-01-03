using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileInstance : MonoBehaviour
{
    private const int IndexLayerProjectile = 7;
    private float currentLifeTime = 5;
    public int damage = 0;
    public WeaponInstance fromWeapon;
    public TeamEnum teamEnum;
    public bool DestroyOnHit;
    private void Awake()
    {
        gameObject.layer = IndexLayerProjectile;
        
    }
    // Update is called once per frame
    public virtual void Update()
    {
        
        if (currentLifeTime > 0)
        {
            currentLifeTime -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
