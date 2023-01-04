using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GrenadeWeaponInstance : WeaponInstance
{
    /// <summary>
    /// Return true if the fire is a success
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public override bool DoFire(GameObject target)
    {
        if (IsHot) return false;
        
        var projectileInstance= Instantiate(PrefabProjectile, transform.position, Quaternion.identity,null);
        var targetPosition = Vector3.Slerp(transform.position, target.transform.position, 0.5f);
        var direction = (targetPosition - transform.position).normalized;
        
        if (projectileInstance.TryGetComponent<Rigidbody>(out var _rigidbody))
        {
            _rigidbody.AddForce(direction* ProjectileSpeed, ForceMode.Impulse);
           // grenade.transform.LookAt(target.transform.position);
        }
        projectileInstance.GetComponent<ProjectileInstance>().fromWeapon = this;
        projectileInstance.GetComponent<ProjectileInstance>().damage = weaponData.damage;
        cooldown = FireRate;
        return true;
    }
}
