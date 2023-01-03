using System;
using System.Collections;
using System.Collections.Generic;
using AudioAliase;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class AIInstance : MonoBehaviour , IActor
{
    #region Constant
    private const string DataDefaultResources = "aiDataDefault";
    private const int IndexLayerProjectile = 7;
    #endregion

    #region CachedVariables
    private Rigidbody _rigidbody;
    private float _minDistanceToKeepWithTarget;
    private int _health;
    private float _speed;
    private GameObject _target;
    private WeaponInstance _currentWeapon;
    private WeaponInstance _grenadeWeapon;
    #endregion
    
    [SerializeField] private AIScriptableObject aiScriptableObject;

    #region Properties
    private float AttackRange => aiScriptableObject.minDistanceToKeepWithTarget;
    #endregion

    /// <summary>
    /// Method where all required variable are check
    /// </summary>
    private void CheckDefaultValues()
    {
        // If aiScriptableObject are not assigned by level designer, we put a default asset from resources folder
        if (aiScriptableObject == null)
        {
            aiScriptableObject = (AIScriptableObject)Resources.Load(DataDefaultResources);
            Debug.LogWarning("AI Instance doesn't have aiScriptableObject variable assigned, please assign it", gameObject);
        }
    }
    private void OnValidate()
    {
        CheckDefaultValues();
    }
    private void Awake()
    {
        CheckDefaultValues();
        _rigidbody = GetComponent<Rigidbody>();
        SpawnWeaponInstance();
        _minDistanceToKeepWithTarget = Random.Range( aiScriptableObject.minDistanceToKeepWithTarget, aiScriptableObject.minDistanceToKeepWithTarget * 1.5f);
        _speed = Random.Range( aiScriptableObject.speed, aiScriptableObject.speed * 1.5f);
    }

    private void SpawnWeaponInstance()
    {
        _currentWeapon = Instantiate(aiScriptableObject.primaryWeapon.prefabWeapon, transform.position, Quaternion.identity,
            transform).GetComponent<WeaponInstance>();
        _currentWeapon.Owner = gameObject;
        _grenadeWeapon = Instantiate(aiScriptableObject.grenadeWeapon.prefabWeapon, transform.position, Quaternion.identity,
            transform).GetComponent<WeaponInstance>();
        _grenadeWeapon.Owner = gameObject;
        _currentWeapon.transform.parent = transform;
        _currentWeapon.transform.position = transform.position;
        _grenadeWeapon.transform.parent = transform;
        _grenadeWeapon.transform.position = transform.position;
    }
    // Start is called before the first frame update
    void Start()
    {
        _health = aiScriptableObject.Health;
    }
    // Update is called once per frame
    void Update()
    {
        if (_target != null)
        {
            ThinkMovement();
            ThinkAttack();
            ThinkTargetPerception();
        }
        else
        {
            _target = GetNearestPlayer();
        }
    }
    private void ThinkTargetPerception()
    {
        float distanceWithTarget = Vector3.Distance(transform.position, _target.transform.position ) ;
        if (distanceWithTarget > aiScriptableObject.LoseSightRadius)
        {
            _target = GetNearestPlayer();
        }
    }
    private void ThinkMovement()
    {
        var targetPosition = _target.transform.position;
        float distanceWithTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceWithTarget > _minDistanceToKeepWithTarget)
        {
            var newPosition = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _speed);
            if (aiScriptableObject.CanFly)
            {
                newPosition.y = Mathf.Clamp(newPosition.y, aiScriptableObject.minY, 10);
            }
            _rigidbody.MovePosition(newPosition);
        }
    }
    private void ThinkAttack()
    {
        if (IsInAttackRange())
        {
            _currentWeapon.DoFire(_target);
        }
        else
        {
            _grenadeWeapon.DoFire(_target);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (_target != null)
            {
                var direction = (_target.transform.position - transform.position).normalized;
                float angle = Mathf.Abs(direction.y);
                Handles.Label(transform.position, 
                    $"Angle {angle }");
                if (IsInAttackRange())
                {
           
                    Debug.DrawLine(transform.position, _target.transform.position ,Color.green);
                }
                else
                {
        
                    Debug.DrawLine(transform.position, _target.transform.position,Color.red);
                }
            }
            
        }
    }
    #endif
    
    private GameObject GetNearestPlayer()
    {
        var alivePlayers = LevelManager.GetAlivePlayers;
        GameObject nearestPlayer = null;
        for (int i = 0; i < alivePlayers.Count; i++)
        {
            float distanceWithAlivePlayer = Vector3.Distance(transform.position, alivePlayers[i].transform.position);
            float distanceWithPotentialTarget = distanceWithAlivePlayer+1;
            if(nearestPlayer != null)
                distanceWithPotentialTarget = Vector3.Distance(transform.position, nearestPlayer.transform.position);

            if (distanceWithAlivePlayer < distanceWithPotentialTarget)
            {
                nearestPlayer = alivePlayers[i].gameObject;
            }
        }

        return nearestPlayer;
    }
    private bool IsInAttackRange()
    {
        var direction = (_target.transform.position - transform.position).normalized;
        float angle = Mathf.Abs(direction.y);
        return angle < aiScriptableObject.angleAim;
    }

  
    public TeamEnum Team => aiScriptableObject.team;
    public int Health => _health;
    public void DoDamage(int amount)
    {
        _health -= amount;
        if (_health <= 0)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        // Do shit before death
        gameObject.PlaySoundAtPosition(aiScriptableObject.AliasOnDeath);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == IndexLayerProjectile)
        {
            var projectile = other.GetComponent<ProjectileInstance>();
            if (projectile.teamEnum != Team)
            {
                DoDamage(projectile.damage);
                projectile.OnHit();
            }
        }
    }
}
