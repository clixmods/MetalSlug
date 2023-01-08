using System;
using System.Collections;
using System.Collections.Generic;
using AudioAliase;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[SelectionBase]
[RequireComponent(typeof(Rigidbody))]
public class AIInstance : MonoBehaviour , IActor
{
    #region Constant
    private const string DataDefaultResources = "aiDataDefault";
    private const int IndexLayerProjectile = 7;
    #endregion

    public static List<AIInstance> AIInstances = new List<AIInstance>();
    
    #region Events
    public delegate void CallbackAIDamage(AIInstance aiInstance);
    public delegate void CallbackAIScore(int score);
    public static event CallbackAIDamage eventAIDeath;
    public static event CallbackAIDamage eventAIHit;

    public static event CallbackAIScore eventAIScore;
    #endregion
    
    #region CachedVariables
    private Rigidbody _rigidbody;
    private float _minDistanceToKeepWithTarget;
    private int _health;
    private float _speed;
    private GameObject _target;
    private WeaponInstance _currentWeapon;
    private WeaponInstance _grenadeWeapon;
    private CharacterViewmodelManager _characterViewmodel;
    private AudioPlayer audioPlayerMove;
    private AudioPlayer LoopAmbiant;
    private float _attackCooldown;
    #endregion
    
    [SerializeField] private AIScriptableObject aiScriptableObject;
    private FXManager _fxDeath;

    #region Properties
    private float AttackRange => aiScriptableObject.minDistanceToKeepWithTarget;
    public int ScoreDead => aiScriptableObject.ScoreDead;
    public int ScoreHit => aiScriptableObject.ScoreHit;
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

        if (TryGetComponent<CharacterViewmodelManager>(out var component))
        {
            _characterViewmodel = component;
        }
        else
        {
            _characterViewmodel = gameObject.AddComponent<CharacterViewmodelManager>();
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
        _fxDeath = FXManager.InitFX(aiScriptableObject.FXDeath,transform.position);
    }

    private void SpawnWeaponInstance()
    {
        _currentWeapon = aiScriptableObject.primaryWeapon.CreateWeaponInstance(gameObject);
        _grenadeWeapon = aiScriptableObject.grenadeWeapon.CreateWeaponInstance(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        _health = aiScriptableObject.Health;
        transform.PlayLoopSound(aiScriptableObject.AliasOnAmbiant, ref LoopAmbiant);
        AIInstances.Add(this);
    }

    private void OnDestroy()
    {
        AudioManager.StopLoopSound(ref LoopAmbiant, StopLoopBehavior.Direct);
        AIInstances.Remove(this);
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
        if (_target == null) return;
        
        _characterViewmodel.Direction = _target.transform.position;
        float distanceWithTarget = Vector3.Distance(transform.position, _target.transform.position ) ;
        if (distanceWithTarget > aiScriptableObject.LoseSightRadius)
        {
            _target = GetNearestPlayer();
        }
    }
    private void ThinkMovement()
    {
        var targetPosition = _target.transform.position;
        

        if (DistanceWithTarget() > _minDistanceToKeepWithTarget)
        {
            var newPosition = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _speed);
            if (aiScriptableObject.CanFly)
            {
                newPosition.y = Mathf.Clamp(newPosition.y, aiScriptableObject.minY, 10);
            }
            _rigidbody.MovePosition(newPosition);
          
            transform.PlayLoopSound(aiScriptableObject.AliasOnMove, ref audioPlayerMove);
        }
        else
        {
            AudioManager.StopLoopSound(ref audioPlayerMove);
        }
    }
    private void ThinkAttack()
    {
        if (_attackCooldown > 0)
        {
            _attackCooldown -= Time.deltaTime;
            return;
        }
            
        if (IsInAttackRange())
        {
            _currentWeapon.DoFire(_target);
        }
        else
        {
            _grenadeWeapon.DoFire(_target);
        }

        _attackCooldown = Random.Range(aiScriptableObject.attackRate, aiScriptableObject.attackRate * 1.5f);
    }

    private float DistanceWithTarget()
    {
        float distanceWithTarget;
        Vector3 transformPosition = transform.position;
        Vector3 targetPosition = _target.transform.position;
        if (!aiScriptableObject.CanFly)
        {
            transformPosition.y = 0;
            targetPosition.y = 0;
        }
        distanceWithTarget = Vector3.Distance(transformPosition, targetPosition);

        return distanceWithTarget;
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
                    $"Angle {angle } // Distance with target {DistanceWithTarget()}");
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
        eventAIHit?.Invoke(this);
        
        if (_health <= 0)
        {
            OnDeath();
            return;
        }
        eventAIScore?.Invoke(aiScriptableObject.ScoreHit);
    }

    public void OnDeath()
    {
        // Do shit before death
        gameObject.PlaySoundAtPosition(aiScriptableObject.AliasOnDeath);
        AudioManager.StopLoopSound(ref audioPlayerMove);
        eventAIDeath?.Invoke(this);
        eventAIScore?.Invoke(aiScriptableObject.ScoreDead);
       
        _fxDeath.Play(transform.position,BehaviorAfterPlay.DestroyAfterPlay);
        UIPointsPlusPanel.CreateUIPointsPlus(FindObjectOfType<Canvas>().gameObject, transform.position , ScoreDead);
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
