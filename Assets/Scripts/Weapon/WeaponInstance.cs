using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WeaponInstance : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    public float cooldown;

    public bool IsHot => cooldown > 0;
    
    public float FireRate => weaponData.fireRate;
    public GameObject PrefabProjectile => weaponData.prefabProjectile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldown > 0)
            cooldown -= Time.deltaTime;
    }

    /// <summary>
    /// Return true if the fire is a success
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool DoFire(GameObject target)
    {
        if (IsHot) return false;
        
        var direction = (target.transform.position - transform.position).normalized;
        var projectileInstance= Instantiate(PrefabProjectile, transform.position, Quaternion.identity,transform);
        if (projectileInstance.TryGetComponent<Rigidbody>(out var _rigidbody))
        {
            _rigidbody.AddForce(direction * 100, ForceMode.Impulse);
            projectileInstance.transform.LookAt(target.transform.position);
        }

        cooldown = FireRate;
        return true;
    }

    public void DoGrenade(GameObject target)
    {
        
    }
}
