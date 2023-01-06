using System.Collections;
using System.Collections.Generic;
using AudioAliase;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(order = 0,fileName = "Weapon Data", menuName = "MetalSlug/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    [Range(0.01f,2)]
    public float fireRate = 0.2f;

    public int startAmmo = -1; 
    [Range(0,5)]
    public int damage = 1;
    [Range(0.01f,20)]
    public float projectileSpeed = 5;

    public bool projectileUseGravity = false;
    public bool projectileDestroyOnHit = true;
    
    [FormerlySerializedAs("prefabWeapon")] public GameObject prefabMeshWeapon;
    public GameObject prefabProjectile;
    [Header("Aliases")] 
    [Aliase] public string AliasOnFire;
    [Aliase] public string AliasOnAfterFire;
    [Aliase] public string AliasOnImpact;
    [Aliase] public string AliasOnDrop;
    [Aliase] public string AliasOnEquip;
    [Header("FX")]
    public GameObject FXFire;

    public WeaponInstance CreateWeaponInstance(GameObject Owner)
    {
        GameObject weaponObject = new GameObject();
        weaponObject.transform.parent = Owner.transform;
        WeaponInstance weaponInstance =  weaponObject.AddComponent<WeaponInstance>();
        GameObject viewModel = null;
        if (prefabMeshWeapon != null)
        {
            Instantiate(prefabMeshWeapon, Owner.transform.position, Quaternion.identity, weaponObject.transform);
        }
        else
        {
            //viewModel = new GameObject();
        }
        
        weaponInstance.weaponData = this;
        weaponInstance.Owner = Owner;
        return weaponInstance;
    }
}
