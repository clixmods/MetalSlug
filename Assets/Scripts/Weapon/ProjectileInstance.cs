using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProjectileInstance : MonoBehaviour
{
    private const int IndexLayerProjectile = 7;
    private float currentLifeTime = 5;
    private Rigidbody _rigidbody;
    private Collider _collider;
    public int damage = 0;
    public WeaponInstance fromWeapon;
    public TeamEnum teamEnum; 
    
    
    const float WorldToViewportPointValueNegative = -0.5f;
    const float WorldToViewportPointValuePositive = 1.5f;
    private FXManager _fxImpact;

    public Rigidbody rigidbody => _rigidbody;
    
    private void Awake()
    {
        gameObject.layer = IndexLayerProjectile;
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
    
    }

    private void Start()
    {
        
        _rigidbody.useGravity = fromWeapon.weaponData.projectileUseGravity;
        teamEnum = fromWeapon.Owner.GetComponent<IActor>().Team;
        if (fromWeapon.weaponData.isGrenade)
        {
            _fxImpact = FXManager.InitFX(fromWeapon.weaponData.FXImpact, transform.position, gameObject);
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if(LevelManager.GetAlivePlayers.Count == 0)
            Destroy(gameObject);
        
        if (currentLifeTime > 0)
        {
            currentLifeTime -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
        // Check if the object is out of the camera
        if(transform.position.IsOutOfCameraVision(WorldToViewportPointValueNegative,WorldToViewportPointValuePositive) )
        {
            Destroy(gameObject);
        }

        var Bulletposition = transform.position;
        Bulletposition.z = 0;
        transform.position = Bulletposition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6 && fromWeapon.weaponData.isGrenade)
        {
        }
            
        OnHit();
    }

    public void OnHit()
    {
        if (fromWeapon.weaponData.isGrenade && _fxImpact != null)
        {
            FXManager.PlayFX(_fxImpact, transform.position, BehaviorAfterPlay.DestroyAfterPlay);
        }
        if (fromWeapon.weaponData.projectileDestroyOnHit)
        {
            Destroy(gameObject);
        }
        
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Vector3 position = transform.position.GetPositionInWorldToViewportPointCamera();
            Handles.Label(transform.position, $" WorldToScreenPoint{position }");
        }
    }
#endif
}
