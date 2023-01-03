using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WeaponInstance : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    public GameObject Owner;
    public float cooldown;
    public bool IsHot => cooldown > 0;
    public float FireRate => weaponData.fireRate;
    public GameObject PrefabProjectile => weaponData.prefabProjectile;
    public float ProjectileSpeed => weaponData.projectileSpeed;
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
    public virtual bool DoFire(GameObject target)
    {
        var direction = (target.transform.position - transform.position).normalized;
        return DoFire(direction);
    }

    public virtual bool DoFire(Vector3 direction)
    {
        if (IsHot) return false;
        
        var projectileInstance= Instantiate(PrefabProjectile, transform.position, Quaternion.identity,null);
        if (projectileInstance.TryGetComponent<Rigidbody>(out var _rigidbody))
        {
            _rigidbody.AddForce(direction * ProjectileSpeed, ForceMode.Impulse);
            projectileInstance.transform.LookAt(transform.position + direction);
        }

        var projectileComponent = projectileInstance.GetComponent<ProjectileInstance>();
        projectileComponent.fromWeapon = this;
        projectileComponent.damage = weaponData.damage;
        projectileComponent.teamEnum = Owner.GetComponent<IActor>().Team;
        projectileComponent.DestroyOnHit = weaponData.projectileDestroyOnHit;
        
        cooldown = FireRate;
        return true;
    }
}
