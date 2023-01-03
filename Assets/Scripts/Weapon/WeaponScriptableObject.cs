using System.Collections;
using System.Collections.Generic;
using AudioAliase;
using UnityEngine;
[CreateAssetMenu(order = 0,fileName = "Weapon Data", menuName = "MetalSlug/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    public float fireRate = 0.2f;
    public float projectileSpeed = 5;
    public int damage = 1;
    public bool projectileDestroyOnHit = true;
    public GameObject prefabWeapon;
    public GameObject prefabProjectile;
    [Header("Aliases")] 
    [Aliase] public string AliasOnFire;
    [Aliase] public string AliasOnDrop;
    [Aliase] public string AliasOnEquip;
    
}
