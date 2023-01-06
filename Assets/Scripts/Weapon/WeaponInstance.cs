using System;
using AudioAliase;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponInstance : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    internal float _cooldown;
    private FXManager _fxFire;

    private GameObject _owner;
    public GameObject Owner
    {
        get => _owner;
        set
        {
            _owner = value;
            if (_owner != null)
            {
                transform.parent = _owner.transform;
                transform.position = _owner.transform.position;
            }
        }
    }
    public bool IsHot => _cooldown > 0;
    public float FireRate => weaponData.fireRate;
    public GameObject PrefabProjectile => weaponData.prefabProjectile;
    public float ProjectileSpeed => weaponData.projectileSpeed;
    
    private void Start()
    {
        _fxFire = FXManager.InitFX(weaponData.FXFire, transform.position, gameObject);
        transform.PlaySoundAtPosition(weaponData.AliasOnEquip);
    }

    // Update is called once per frame
    void Update()
    {
        if (_cooldown > 0)
            _cooldown -= Time.deltaTime;
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
    /// <summary>
    /// Return true if the fire is a success
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual bool DoFire(Vector3 direction)
    {
        if (IsHot) return false;
        
        var projectileInstance= Instantiate(PrefabProjectile, transform.position, Quaternion.identity,null);
        var projectileComponent = projectileInstance.GetComponent<ProjectileInstance>();
        projectileComponent.fromWeapon = this;
        projectileComponent.damage = weaponData.damage;
        if (projectileInstance.TryGetComponent<Rigidbody>(out var _rigidbody))
        {
            _rigidbody.AddForce(direction * ProjectileSpeed, ForceMode.Impulse);
            projectileInstance.transform.LookAt(transform.position + direction);
        }
        
        _fxFire.Play(transform.position);
        _cooldown = FireRate;
        transform.PlaySoundAtPosition(weaponData.AliasOnFire);
        transform.PlaySoundAtPosition(weaponData.AliasOnAfterFire);
        return true;
    }
}
