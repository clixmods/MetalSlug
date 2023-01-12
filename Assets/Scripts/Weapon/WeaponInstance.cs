using System;
using AudioAliase;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponInstance : MonoBehaviour
{
    public delegate void WeaponEvent();
    public event WeaponEvent eventWeaponFire;
    
    public WeaponScriptableObject weaponData;
    internal float _cooldown;
    private FXManager _fxFire;

    private GameObject _owner;
    private int _currentAmmo = -1;

    // Burst
    [Header("Burst")] 
    private bool isBursting;
    private int currentIndexBurst = 0;
    private float burstRate;
    private Vector3 _directionCached;

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
    public int CurrentAmmo => _currentAmmo;
    private void Start()
    {
        _fxFire = FXManager.InitFX(weaponData.FXFire, transform.position, gameObject);
        
        transform.PlaySoundAtPosition(weaponData.AliasOnEquip);
        _currentAmmo = weaponData.startAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if ( !isBursting &&_cooldown > 0)
            _cooldown -= Time.deltaTime;

        if (isBursting && currentIndexBurst > 0)
        {
            if (burstRate > 0)
            {
                burstRate -= Time.deltaTime;
            }
            else
            {
                DoFire(_directionCached);
                currentIndexBurst--;
                burstRate = weaponData.delayBurstedBullet;
                if (currentIndexBurst <= 0)
                {
                    isBursting = false;
                    _cooldown = FireRate;
                }
            }
        }
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

        if (_currentAmmo == 0)
        {
            return false;
        }
        eventWeaponFire?.Invoke();

        _directionCached = direction;
        // Apply burst behavior 
        if (weaponData.burst && !isBursting)
        {
            isBursting = true;
            currentIndexBurst = weaponData.bulletPerBurst-1;
            burstRate = weaponData.delayBurstedBullet;
        }
        
        var projectileInstance= Instantiate(PrefabProjectile, transform.position, Quaternion.identity,null);
        var projectileComponent = projectileInstance.GetComponent<ProjectileInstance>();
        projectileComponent.fromWeapon = this;
        projectileComponent.damage = weaponData.damage;
        if (projectileInstance.TryGetComponent<Rigidbody>(out var _rigidbody))
        {
            _rigidbody.AddForce(direction * ProjectileSpeed, ForceMode.Impulse);
            projectileInstance.transform.LookAt(transform.position + direction);
        }
       
        var fxManager = FXManager.PlayFX(_fxFire,transform.position);
       // fxManager.transform.rotation = transform.rotation;
        if(!isBursting)
            _cooldown = FireRate;
        
        transform.PlaySoundAtPosition(weaponData.AliasOnFire);
        transform.PlaySoundAtPosition(weaponData.AliasOnAfterFire);
        
        if( _currentAmmo != -1)
            _currentAmmo--;
        
        return true;
    }
}
