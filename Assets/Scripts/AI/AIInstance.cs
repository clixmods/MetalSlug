using System.Collections.Generic;
using AudioAliase;
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
    public static event CallbackAIDamage eventGlobalAIDeath;
    
    public event CallbackAIDamage eventAIDeath;
    public static event CallbackAIDamage eventGlobalAIHit;

    public static event CallbackAIScore eventGlobalAIScore;
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
    
    private FXManager _fxDeath;
    private FXManager _fxAmbiant;
    private FXManager _fxHit;
    private FXManager _fxDamaged;
    private FXManager _fxLowHealth;
    private FXManager _fxMove;
    #endregion
    
    [SerializeField] private AIScriptableObject aiScriptableObject;
    

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
        InitFXInstance();
    }

    private void SpawnWeaponInstance()
    {
        _currentWeapon = aiScriptableObject.primaryWeapon.CreateWeaponInstance(gameObject);
        if(_characterViewmodel.rightHand != null)
            _currentWeapon.transform.parent = _characterViewmodel.rightHand.transform;

        _currentWeapon.eventWeaponFire += () =>
        {
            _characterViewmodel.Play(AnimState.Fire);
        };
        
        _currentWeapon.transform.localPosition = Vector3.zero;
        _grenadeWeapon = aiScriptableObject.grenadeWeapon.CreateWeaponInstance(gameObject);
        if(_characterViewmodel.leftHand != null)
            _grenadeWeapon.transform.parent = _characterViewmodel.leftHand.transform;
        
        _grenadeWeapon.transform.localPosition = Vector3.zero;

        _grenadeWeapon.eventWeaponFire += () =>
        {
            _characterViewmodel.Play(AnimState.Grenade);
        };
    }

    private void InitFXInstance()
    {
        _fxDeath = FXManager.InitFX(aiScriptableObject.FXDeath,transform.position);
        _fxAmbiant= FXManager.InitFX(aiScriptableObject.FXLoopAmbiant,transform.position);
        if (_characterViewmodel.skinnedMeshRenderer != null)
        {
            _fxHit= FXManager.InitFX(aiScriptableObject.FXHit,transform.position,gameObject, _characterViewmodel.skinnedMeshRenderer);
            _fxDamaged= FXManager.InitFX(aiScriptableObject.FXLoopDamaged,transform.position,gameObject, _characterViewmodel.skinnedMeshRenderer);
            _fxLowHealth= FXManager.InitFX(aiScriptableObject.FXLoopLowHealth,transform.position,gameObject, _characterViewmodel.skinnedMeshRenderer);
        }
        else
        {
            _fxHit= FXManager.InitFX(aiScriptableObject.FXHit,transform.position,gameObject);
            _fxDamaged= FXManager.InitFX(aiScriptableObject.FXLoopDamaged,transform.position,gameObject);
            _fxLowHealth= FXManager.InitFX(aiScriptableObject.FXLoopLowHealth,transform.position,gameObject);
        }
        _fxMove= FXManager.InitFX(aiScriptableObject.FXMove,transform.position);
    }
    // Start is called before the first frame update
    void Start()
    {
        _health = aiScriptableObject.Health;
        transform.PlayLoopSound(aiScriptableObject.AliasOnAmbiant, ref LoopAmbiant);
        AIInstances.Add(this);
        _attackCooldown = 3;
    }

    private void OnDestroy()
    {
        OnDown(true);
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
            if (_target != null && !_target.GetComponent<PlayerInstance>().IsAlive)
            {
                _target = null;
            }    
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

            newPosition.z = 0;
            _rigidbody.MovePosition(newPosition);
            _characterViewmodel.Play(AnimState.Move);
            transform.PlayLoopSound(aiScriptableObject.AliasOnMove, ref audioPlayerMove);
        }
        else
        {
            _characterViewmodel.Play(AnimState.Idle);
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
            if (aiScriptableObject.canReajustAim)
            {
                _currentWeapon.DoFire(_target);
            }
            else
            {
                var direction = (_target.transform.position - transform.position).normalized;
                direction.x = (int) direction.x;
                direction.y = (int) direction.y;
                _currentWeapon.DoFire(direction);
            }
            
        }
        else
        {
            if (aiScriptableObject.canReajustAim)
            {
                _grenadeWeapon.DoFire(_target);
            }
            else
            {
                var direction = (_target.transform.position - transform.position).normalized;
                direction.x = (int) direction.x;
                direction.y = (int) direction.y;
                _grenadeWeapon.DoFire(direction);
            }
            
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
    
    const float WorldToViewportPointValueNegative = -0.5f;
    const float WorldToViewportPointValuePositive = 1.5f;
    private bool IsInAttackRange()
    {
        // Check if the object is out of the camera
        Vector3 position = Camera.main.WorldToViewportPoint(transform.position);
        
        bool isOutCameraNegative = position.x < WorldToViewportPointValueNegative || position.y < WorldToViewportPointValueNegative;
        bool isOutCameraPositive =  position.x > WorldToViewportPointValuePositive || position.y > WorldToViewportPointValuePositive;
        Debug.Log($"Hello  {isOutCameraNegative} : {isOutCameraPositive}" );
        if(isOutCameraNegative || isOutCameraPositive )
        {
            return false;
        }
        
        
        var direction = (_target.transform.position - transform.position).normalized;
        float angle = Mathf.Abs(direction.y);
        return angle < aiScriptableObject.angleAim;
    }

  
    public TeamEnum Team => aiScriptableObject.team;
    public int Health => _health;
    public void DoDamage(int amount)
    {
        _health -= amount;
        eventGlobalAIHit?.Invoke(this);
        FXManager.PlayFX(_fxHit,transform.position,BehaviorAfterPlay.Nothing);

        if (aiScriptableObject.Health * 0.6f > _health)
        {
            FXManager.PlayFX(_fxDamaged,transform.position,BehaviorAfterPlay.Nothing);

        }
        if (aiScriptableObject.Health * 0.2f > _health)
        {
            FXManager.PlayFX(_fxLowHealth,transform.position,BehaviorAfterPlay.Nothing);

        }
        
        _characterViewmodel.Play(AnimState.Damaged);
        if (_health <= 0)
        {
            OnDown();
            return;
        }
        eventGlobalAIScore?.Invoke(aiScriptableObject.ScoreHit);
    }

    public void OnDown()
    {
        OnDown(false);
    }
    public void OnDown(bool noDestroy = false)
    {
        // Do shit before death
        gameObject.PlaySoundAtPosition(aiScriptableObject.AliasOnDeath);
        AudioManager.StopLoopSound(ref audioPlayerMove);
        eventGlobalAIDeath?.Invoke(this);
        eventGlobalAIScore?.Invoke(aiScriptableObject.ScoreDead);
        eventAIDeath?.Invoke(this);
        if (aiScriptableObject.EarthquakeOnDeath)
        {
            // CinemachineCameraShake.SetNoisier(1,2);
            // CameraShake.ShakeMe();
        }
        FXManager.PlayFX(_fxDeath,transform.position,BehaviorAfterPlay.DestroyAfterPlay);
        if (LevelManager.Instance.players.Count > 0)
        {
            UIPointsPlusPanel.CreateUIPointsPlus(FindObjectOfType<Canvas>().gameObject, transform.position, ScoreDead);
        }
        if(!noDestroy)
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

    private void PlayFX()
    {
        
    }
}
