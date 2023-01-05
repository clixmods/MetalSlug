using System.Collections;
using System.Collections.Generic;
using AudioAliase;
using UnityEngine;
[CreateAssetMenu(order = 0,fileName = "Weapon Data", menuName = "MetalSlug/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    [Range(0.01f,2)]
    public float fireRate = 0.2f;
    
    [Range(0,5)]
    public int damage = 1;
    [Range(0.01f,20)]
    public float projectileSpeed = 5;

    public bool projectileUseGravity = false;
    public bool projectileDestroyOnHit = true;
    public GameObject prefabWeapon;
    public GameObject prefabProjectile;
    [Header("Aliases")] 
    [Aliase] public string AliasOnFire;
    [Aliase] public string AliasOnAfterFire;
    [Aliase] public string AliasOnImpact;
    [Aliase] public string AliasOnDrop;
    [Aliase] public string AliasOnEquip;
    [Header("FX")]
    public GameObject FXFire;
}
