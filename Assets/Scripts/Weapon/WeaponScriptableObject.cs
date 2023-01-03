using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(order = 0,fileName = "Weapon Data", menuName = "MetalSlug/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    public float fireRate = 0.2f;
    public float projectileSpeed = 5;
    public GameObject prefabWeapon;
    public GameObject prefabProjectile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
