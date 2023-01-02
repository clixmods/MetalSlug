using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeInstance : ProjectileInstance
{
    public Vector3 targetPosition;
    private WeaponScriptableObject _weaponScriptableObject;
    public void InitGrenade(Vector3 position,WeaponScriptableObject weaponData)
    {
        targetPosition = position;
        _weaponScriptableObject = weaponData;
    }
    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        //transform.position = Vector3.Slerp(transform.position, targetPosition,
         //   Time.deltaTime * _weaponScriptableObject.projectileSpeed);
    }
}
