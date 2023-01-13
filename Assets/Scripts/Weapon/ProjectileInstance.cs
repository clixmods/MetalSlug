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
    
    
    private void Awake()
    {
        gameObject.layer = IndexLayerProjectile;
        _collider = GetComponent<Collider>();

    
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
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
        Vector3 position = Camera.main.WorldToViewportPoint(transform.position);
        
        bool isOutCameraNegative = position.x < WorldToViewportPointValueNegative || position.y < WorldToViewportPointValueNegative;
        bool isOutCameraPositive =  position.x > WorldToViewportPointValuePositive || position.y > WorldToViewportPointValuePositive;
        if(isOutCameraNegative || isOutCameraPositive )
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
            // var casts = Physics.SphereCastAll(transform.position, 10, Vector3.zero, 10, 0, QueryTriggerInteraction.Collide);
            // foreach (var cast in casts)
            // {
            //     if (cast.collider.gameObject.layer == 6 && fromWeapon.Owner != cast.transform.gameObject)
            //     {
            //         var actor = cast.transform.GetComponent<IActor>();
            //         if (actor.Team != teamEnum)
            //         {
            //             cast.collider.GetComponent<IActor>().DoDamage(fromWeapon.weaponData.damage);
            //         }
            //     }
            //         
            // }
            //
            //

            // if (casts.Length != 0)
            // {
            //     
            //     Destroy(gameObject);
            // }
                
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
            Vector3 position = Camera.main.WorldToViewportPoint(transform.position);
            Handles.Label(transform.position, $" WorldToScreenPoint{position }");
            // if (_target != null)
            // {
            //     var direction = (_target.transform.position - transform.position).normalized;
            //     float angle = Mathf.Abs(direction.y);
            //     Handles.Label(transform.position, 
            //         $"Angle {angle }");
            //     if (IsInAttackRange())
            //     {
            //
            //         Debug.DrawLine(transform.position, _target.transform.position ,Color.green);
            //     }
            //     else
            //     {
            //
            //         Debug.DrawLine(transform.position, _target.transform.position,Color.red);
            //     }
            // }
            
        }
    }
#endif
}
