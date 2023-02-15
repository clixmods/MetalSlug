using AudioAliase;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
[RequireComponent(typeof(CharacterController))]
public class PlayerInstance : MonoBehaviour , IActor
{
    #region Events
    public delegate void PlayerEvent(PlayerInstance newPlayer);
    public static event PlayerEvent eventPlayerJoin;
    /// <summary>
    /// Event when the player is death
    /// </summary>
    public static event PlayerEvent eventPlayerDeath;
    /// <summary>
    /// Event when the player fall in last stand (coop only)
    /// </summary>
    public static event PlayerEvent eventPlayerDown;
    public static event PlayerEvent eventPlayerSpawn;
    public static event PlayerEvent eventPlayerRevive;
    public static event PlayerEvent eventPlayerFire;
    public static event PlayerEvent eventIsReviving;
    #endregion

    // REFS DE SCRIPTS
    [Header("Weapon")]
    [SerializeField] private WeaponScriptableObject primaryWeapon;
    [SerializeField] private WeaponScriptableObject grenadeWeapon;
    
    private WeaponInstance _weaponInstance;
    private WeaponInstance _grenadeInstance;
    public WeaponInstance weaponInstance => _weaponInstance;
    public WeaponInstance grenadeInstance => _grenadeInstance;
    
    public HighscoreTable highscoreTable;
    
    private PlayerInstance _playerQueJeSoigne;

    // REFS DE GO
    [SerializeField] private GameObject _cameraTarget;
    [SerializeField] private GameObject _parachute;

    // FLOAT & INT
    [Header("Settings")]
    [SerializeField] private float _playerSpeed = 2.0f;
    [SerializeField] private float _jumpHeight = 3.0f;
    [SerializeField] private float _gravityValue = -9.81f;
    [SerializeField] private float _timerDeath;
    public float TimerDeath => _timerDeath;
    
    private float timerInvulnerability = 0;
    private float _timerDeathCache;
    [SerializeField] public float ctxCached { get; private set; }

    [SerializeField] private int _lastDirection;
    [SerializeField] private int _startHealth = 900;
    [SerializeField] private int _health = 1;


    // RB
    private CharacterController controller;

    // VECTOR
    private Vector3 _playerVelocity;
    private Vector3 _playerTransform;
    private Vector2 _movementInput = Vector2.zero;
    private Vector2 _aimDir;
    private Vector2 _aimDirGrenade = Vector2.zero;
    private Vector2 _currentMovementInput;

    #region Properties

    // Todo : need to be implemented
    public bool IsAlive => !_isLastStand;

    #endregion

    // BOOL
    private bool _groundedPlayer;
    private bool _jumped = false;
    private bool _shooted;
    private bool _shootedGrenade;
    private bool _isCrouching = false;
    private bool _inRange = false;
    private bool _isQuandilsoigne = false;
    private bool _isQuandilsefaitrevive = false;
    private bool _isLastStand = false;
    private bool _isSpawned;
    private bool _firstSpawn = true;
    private bool _isDead;
    public bool testEndGame = false;

    // FX Instancied 
    private FXManager _fxDeath;
    private FXManager _fxAmbiant;
    private FXManager _fxHit;
    private FXManager _fxDamaged;
    private FXManager _fxLowHealth;
    private FXManager _fxMove;
    // FX
    [Header("FX")] 
    public GameObject FXLoopAmbiant;
    public GameObject FXDeath;
    public GameObject FXHit;
    public GameObject FXMove;
    public GameObject FXLoopDamaged;

    [Header("Alias")] 
    [SerializeField] [Aliase] private string AliasOnMoveLoop;
    [SerializeField] [Aliase] private string AliasOnJump;
    [SerializeField] [Aliase] private string AliasOnLand;
    [SerializeField] [Aliase] private string AliasOnLastStand;
    [SerializeField] [Aliase] private string AliasOnLastStandLoop;
    [SerializeField] [Aliase] private string AliasOnDeath;
    public bool IsQuandilsoigne => _isQuandilsoigne;
    public bool IsLastStand => _isLastStand;
    public bool IsQuandilsefaitrevive => _isQuandilsefaitrevive;

    [SerializeField] private TeamEnum _team;
    private CharacterViewmodelManager _characterViewmodel;

    [SerializeField] private LayerMask groundLayerMask;
    float distToGround;
    private void Awake()
    {
        _characterViewmodel = GetComponent<CharacterViewmodelManager>();
        SpawnWeaponInstance();
        Health = _startHealth;
        timerInvulnerability = 3f;
        _timerDeath = 15f;
        InitFXInstance();
        
        
    }
    private void InitFXInstance()
    {
        _fxDeath = FXManager.InitFX(FXDeath,transform.position);
        _fxAmbiant= FXManager.InitFX(FXLoopAmbiant,transform.position);
        if (_characterViewmodel.SkinnedMeshRenderer != null)
        {
            _fxHit= FXManager.InitFX(FXHit,transform.position,gameObject, _characterViewmodel.SkinnedMeshRenderer);
            _fxDamaged= FXManager.InitFX(FXLoopDamaged,transform.position,gameObject, _characterViewmodel.SkinnedMeshRenderer);

        }
        else
        {
            _fxHit= FXManager.InitFX(FXHit,transform.position,gameObject); 
            _fxDamaged= FXManager.InitFX(FXLoopDamaged,transform.position,gameObject);
        }
        _fxMove= FXManager.InitFX(FXMove,transform.position);
    }
    private void SpawnWeaponInstance()
    {
        // Spawn primary weapon
        _weaponInstance = primaryWeapon.CreateWeaponInstance(gameObject);
        if(_characterViewmodel.rightHand != null)
            weaponInstance.transform.parent = _characterViewmodel.rightHand.transform;
        weaponInstance.transform.localPosition = Vector3.zero;
        // Spawn grenade weapon
        _grenadeInstance = grenadeWeapon.CreateWeaponInstance(gameObject);
        if(_characterViewmodel.leftHand != null)
            grenadeInstance.transform.parent = _characterViewmodel.leftHand.transform;
        grenadeInstance.transform.localPosition = Vector3.zero;
    }

    // start
    private void Start()
    {
        // This script will manage IsFalling behavior, so we need to disable isfalling management from _characterViewmodel
        _characterViewmodel.ManageIsFalling = false;
        if (!_isSpawned)
        {
            controller = gameObject.GetComponent<CharacterController>();
       
            eventPlayerJoin?.Invoke(this);
            _isSpawned = true;
            Parachute();
            eventPlayerSpawn?.Invoke(this);
            timerInvulnerability = 3;
        }
        distToGround = (controller.height/2f) + controller.skinWidth ;
    }

    // OnDestroy
    private void OnDestroy()
    {
        eventPlayerDeath?.Invoke(this);
    }

    // OnTriggerEnter 
    private void OnTriggerEnter(Collider other)
    {
        if ( !_isDead && other.gameObject.layer == IndexLayerProjectile)
        {
            var projectile = other.GetComponent<ProjectileInstance>();
            if (projectile.teamEnum != Team)
            {
                DoDamage(projectile.damage);
                projectile.OnHit();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.parent != null && other.transform.parent.CompareTag("Player"))
        {
            _inRange = true;
            var playerInstanceCached = other.transform.parent.GetComponent<PlayerInstance>();
            if (playerInstanceCached._isLastStand)
            {
                _playerQueJeSoigne = playerInstanceCached;
                _playerQueJeSoigne._isQuandilsefaitrevive = _isQuandilsoigne;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent != null && other.transform.parent.CompareTag("Player"))
        {
            var playerInstanceCached = other.transform.parent.GetComponent<PlayerInstance>();
            if (playerInstanceCached._isLastStand && playerInstanceCached == _playerQueJeSoigne )
            {
                _playerQueJeSoigne._isQuandilsefaitrevive = false;
                _playerQueJeSoigne = null;
            }
        }
    }

    // move
    public void OnMove(InputAction.CallbackContext context)
    {
        if (LevelManager.Instance.State != State.InGame)
        {
            _movementInput = Vector2.zero;
            _aimDir = Vector2.right;
            return;
        }
        if (_isQuandilsoigne || _isLastStand)
        {
            return;
        }

        // readValue via InputSystem
        _movementInput = context.ReadValue<Vector2>();
        if (context.phase == InputActionPhase.Performed)
        {
            if (!(_movementInput.x == 0 && _movementInput.y == 0))
            {
                // Update the current movement input.
                _currentMovementInput = _movementInput;

                // Update the last direction the player moved in.
                if (_currentMovementInput.x < 0)
                {
                    _lastDirection = -1;
                }
                else if (_currentMovementInput.x > 0)
                {
                    _lastDirection = 1;
                }
            }
            else
            {
                // If the player is not moving, reset the current movement input.
                _currentMovementInput = Vector2.zero;
            }

            // Update the crouching state.
            if (_currentMovementInput.y < 0 && _groundedPlayer)
            {
                _isCrouching = true;
            }
            else
            {
                _isCrouching = false;
            }
        }
    }

    // jump
    public void OnJump(InputAction.CallbackContext context)
    {
        if (_isQuandilsoigne || _isLastStand)
        {
            return;
        }

        // switch for set true & false the action
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                _jumped = true;
                break;
            case InputActionPhase.Canceled:
                _jumped = false;
                break;
            default:
                break;
        }
    }

    // shoot
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (_isQuandilsoigne || _isLastStand)
        {
            return;
        }
        eventPlayerFire?.Invoke(this);
        switch (context.phase)
        {
            case InputActionPhase.Started:
                
                // Check the direction to shoot based on the player's current movement input and whether they are in the air.
                if (_currentMovementInput.y < 0 && _groundedPlayer)
                {
                    // If the player is on the ground and pressing S, shoot the last direction.
                    if (_lastDirection < 0)
                    {
                        // If the player was moving left, shoot left.
                        if (weaponInstance.DoFire(Vector2.left))
                        {
                            _characterViewmodel.Play(AnimState.Fire);
                        }
                    }
                    else if (_lastDirection > 0)
                    {
                        // If the player was moving right, shoot right.
                        if (weaponInstance.DoFire(Vector2.right))
                        {
                            _characterViewmodel.Play(AnimState.Fire);
                        }
                    }
                    break;
                }
                else if (_currentMovementInput.y < 0 || _aimDir.y < 0)
                {
                    // If the player is in the air and pressing S, or if they are in the air and aiming downwards, shoot downwards.
                    if (weaponInstance.DoFire(Vector2.down))
                    {
                        _characterViewmodel.Play(AnimState.FireDown);
                    }
                }
                else if (_currentMovementInput.y > 0)
                {
                    // If the player is pressing W, shoot upwards.
                    if (weaponInstance.DoFire(Vector2.up))
                    {
                        _characterViewmodel.Play(AnimState.FireUp);
                    }
                }
                else if (_currentMovementInput.x < 0)
                {
                    // If the player is pressing A, shoot left.
                    if (weaponInstance.DoFire(Vector2.left))
                    {
                        _characterViewmodel.Play(AnimState.Fire);
                    }
                }
                else if (_currentMovementInput.x > 0)
                {
                    // If the player is pressing D, shoot right.
                    if (weaponInstance.DoFire(Vector2.right))
                    {
                        _characterViewmodel.Play(AnimState.Fire);
                    }
                }
                else if (_currentMovementInput.y == 0 && _currentMovementInput.x == 0 && _groundedPlayer)
                {
                    // If the player is not moving and on the ground, check the last direction they moved in.
                    if (_lastDirection < 0)
                    {
                        // If the player was moving left, shoot left.
                        if (weaponInstance.DoFire(Vector2.left))
                        {
                            _characterViewmodel.Play(AnimState.Fire);
                        }
                    }
                    else if (_lastDirection > 0)
                    {
                        // If the player was moving right, shoot right.
                        if (weaponInstance.DoFire(Vector2.right))
                        {
                            _characterViewmodel.Play(AnimState.Fire);
                        }
                    }
                }
                else if (_currentMovementInput.y == 0 && _currentMovementInput.x == 0 && !_groundedPlayer)
                {
                    // If the player is not moving and in the air, check the last direction they moved in.
                    if (_lastDirection < 0)
                    {
                        // If the player was moving left, shoot left.
                        if (weaponInstance.DoFire(Vector2.left))
                        {
                            _characterViewmodel.Play(AnimState.Fire);
                        }
                    }
                    else if (_lastDirection > 0)
                    {
                        // If the player was moving right, shoot right.
                        if (weaponInstance.DoFire(Vector2.right))
                        {
                            _characterViewmodel.Play(AnimState.Fire);
                        }
                    }
                }
                break;

            case InputActionPhase.Performed:
                _shooted = true;
                break;

            case InputActionPhase.Canceled:
                _shooted = false;
                break;
        }
    }

    public void OnShootGrenade(InputAction.CallbackContext context)
    {
        if (_isQuandilsoigne || _isLastStand)
        {
            return;
        }

        eventPlayerFire?.Invoke(this);
        switch (context.phase)
        {
            case InputActionPhase.Started:
                if (_lastDirection < 0)
                {
                    // If the player was moving left, shoot the grenade left.
                    _aimDirGrenade = new Vector2(-1+(0.5f*_movementInput.x), 1.25f);
                    if (grenadeInstance.DoFire(_aimDirGrenade))
                    {
                        _characterViewmodel.Play(AnimState.Grenade);
                    }
                }
                else if (_lastDirection > 0)
                {
                    // If the player was moving right, shoot the grenade right.
                    _aimDirGrenade = new Vector2(1+(0.5f*_movementInput.x), 1.25f);
                    if (grenadeInstance.DoFire(_aimDirGrenade))
                    {
                        _characterViewmodel.Play(AnimState.Grenade);
                    }
                }
                break;
            case InputActionPhase.Performed:
                _shootedGrenade = true;
                break;
            case InputActionPhase.Canceled:
                _shootedGrenade = true;
                break;
        }
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (_isLastStand || _playerQueJeSoigne == null)
        {
            _isQuandilsoigne = false;
            return;
        }

        ctxCached = (float)context.time - (float)context.startTime;
        switch (context.phase)
        {
            case InputActionPhase.Started:
                _movementInput = new Vector2(0, 0);
                _isQuandilsoigne = true;
                break;
            case InputActionPhase.Performed:
                if (_playerQueJeSoigne != null)
                {
                    _playerQueJeSoigne.Revive();
                    _isQuandilsoigne = false;
                    _playerQueJeSoigne = null;
                }
                break;
            case InputActionPhase.Canceled:
                _isQuandilsoigne = false;
                break;
        }
    }

    private void Parachute()
    {
        Teleport(new Vector3(RoundManager.PlayerSpawnActive.position.x, 10f, 0f));
        _gravityValue = -2f;
        _parachute.SetActive(true);
        _characterViewmodel.SetAnimatorBool(CharacterViewmodelManager.IsFalling, true);   
        _firstSpawn = false;
    }
    
    void FixedUpdate()
    {
        
    }

    // update
    void Update()
    {
        if (!_isLastStand)
        {
            AudioManager.StopLoopSound(ref audioPlayerLoopLastStand);    
            _isQuandilsefaitrevive = false;
        }
        if (_playerQueJeSoigne != null && !_playerQueJeSoigne._isLastStand)
        {
            _playerQueJeSoigne = null;
            _isQuandilsoigne = false;
        }
        // Check Out Of Bounds, if true go kill the player
        if (transform.position.y < -10)
        {
            OnDeath();
        }
        // Invulnerability timer 
        if (timerInvulnerability > 0)
        {
            timerInvulnerability -= Time.deltaTime;
        }
        else
        {
            timerInvulnerability = 0;
        }
        
        if (_firstSpawn)
        {
            Parachute();
        }

        if (_isLastStand && !_isQuandilsefaitrevive)
        {
            transform.PlayLoopSound(AliasOnLastStandLoop,ref audioPlayerLoopLastStand);       
            _isQuandilsoigne = false;
            _timerDeath -= Time.deltaTime;
        }

        if ((_isLastStand && _timerDeath <= 0) || LevelManager.GetAlivePlayers.Count == 0)
        {
            OnDeath();
            return;
        }

        Vector3 motion = Vector3.zero;
        if (!_isLastStand)
        {
            if (_aimDir.y > 0)
            {
                _characterViewmodel.Play(AnimState.LookUp);
            }
            else if (_aimDir.y < 0)
            {
                _characterViewmodel.Play(AnimState.LookDown);
            }
            
            // check if the player is grounded
            //_groundedPlayer = Physics.Raycast(transform.position, -Vector3.up, distToGround + Physics.defaultContactOffset, groundLayerMask );
            _groundedPlayer = controller.isGrounded;
            if (_groundedPlayer && _playerVelocity.y <= 0)
            { 
                _parachute.SetActive(false);
                // set the velocity to 0
                _playerVelocity.y = 0f;
                _gravityValue = -20f;
                // if the player jump and crouch while in the air and arrive on the ground crouched then isCrouching is set to true
                if (_currentMovementInput.y < 0 )
                {
                    _isCrouching = true;
                }
            }
            // move the player
            Vector3 move = new Vector3(_movementInput.x, 0, 0);

             motion = move.normalized  * _playerSpeed;
            
            var positionWithMotion = transform.position + motion;
            if (!positionWithMotion.IsOutOfCameraVision(-0.1f,1.1f) )
            {
                //controller.Move(motion);
                if (motion.magnitude > 0)
                {
                    transform.PlayLoopSound(AliasOnMoveLoop,ref audioPlayerLoopMove);
                    _characterViewmodel.Play(AnimState.Move);
                }
                else
                {
                    AudioManager.StopLoopSound(ref audioPlayerLoopMove);
                    _characterViewmodel.Play(AnimState.Idle);
                }
            }
            else
            {
                if (positionWithMotion.IsOutCameraNegative(-0.1f))
                {
                    motion += Vector3.right * _playerSpeed;
                   // controller.Move(Vector3.right * Time.deltaTime * _playerSpeed);
                    _characterViewmodel.Play(AnimState.Move);
                }
                if (positionWithMotion.IsOutCameraPositive(1.1f))
                {
                    motion += Vector3.left * _playerSpeed;
                    //controller.Move(Vector3.left * Time.deltaTime * _playerSpeed);
                    _characterViewmodel.Play(AnimState.Move);
                }
            }
            _characterViewmodel.Direction = transform.position + motion;
            // Changes the height position of the player..
            if (_jumped && _groundedPlayer)
            {
                _playerVelocity.y += Mathf.Sqrt(_jumpHeight * -3.0f * _gravityValue);
            }
        }
        // add gravity to the player
        _playerVelocity.y += _gravityValue * Time.deltaTime;
        _playerVelocity.y = Mathf.Clamp(_playerVelocity.y, -20, 20);
        // motion
        motion += _playerVelocity;
        controller.Move(motion * Time.deltaTime);
        if (_groundedPlayer)
        {
            if (_characterViewmodel.GetAnimatorBool(CharacterViewmodelManager.IsFalling))
            {
                _characterViewmodel.SetAnimatorBool(CharacterViewmodelManager.IsFalling, false);
                transform.PlaySoundAtPosition(AliasOnLand);   
            }
            if (_characterViewmodel.GetAnimatorBool(CharacterViewmodelManager.IsFalling))
            {
                Debug.Log("FUCK");
            }
           
        }
        else
        {
            if (!_characterViewmodel.GetAnimatorBool(CharacterViewmodelManager.IsFalling))
            {
                _characterViewmodel.SetAnimatorBool(CharacterViewmodelManager.IsFalling, true);   
                transform.PlaySoundAtPosition(AliasOnJump);   
            }
        }
    }
    
    
    /// <summary>
    /// This method allow the teleportation of the player to prevent charactercontroller restriction.
    /// </summary>
    /// <param name="position"></param>
    public void Teleport(Vector3 position)
    {
        controller.enabled = false;
        transform.position = position;
        controller.enabled = true;
    }

    public void SetSleepCharacterController(bool value)
    {
        if (value)
        {
            controller.enabled = false;
        }
        else
        {
            controller.enabled = true;
        }
    }
  
    /// <summary>
    /// Revive the player and reset some properties to initial values
    /// </summary>
    private void Revive()
    {
        eventPlayerRevive?.Invoke(this);
        _movementInput = Vector2.zero;
        _aimDir = Vector2.right;
        _characterViewmodel.Play(AnimState.Revived);
        _isLastStand = false;
        _jumped = false;
        Health = _startHealth;
        timerInvulnerability = 1.5f;
        if(damagedFx != null)
            Destroy(damagedFx.gameObject);
    }

    public bool IsInvulnerable
    {
        get => timerInvulnerability > 0;
    }
    public TeamEnum Team => _team;
    public int Health
    {
        get => _health;
        private set
        {
            // Health cannot be changed if the player is invulnerable
            if (IsInvulnerable)
            {
                return;
            }
            _health = value;
        }
    }

    public void DoDamage(int amount)
    {  
        Health -= amount;
        if ( Health <= 0 && !_isLastStand)
        {
            FXManager.PlayFX(_fxHit,transform.position,BehaviorAfterPlay.Nothing);
            OnDown();
        }
    }

    private FXManager damagedFx;
    private static readonly int IsFalling = Animator.StringToHash("IsFalling");
    private AudioPlayer audioPlayerLoopMove;
    private AudioPlayer audioPlayerLoopLastStand;

    public void OnDown()
    {
        AudioManager.StopLoopSound(ref audioPlayerLoopMove);    
        AudioManager.StopLoopSound(ref audioPlayerLoopLastStand);        
        if (LevelManager.Players.Count == 1 || LevelManager.Instance.ReviveAmount <= 0)
        {
            OnDeath();
            return;
        }
        transform.PlaySoundAtPosition(AliasOnLastStand);  
        AudioManager.PlayAnnouncer("announcer_player_down");
        damagedFx = FXManager.PlayFX(_fxDamaged,transform.position);
        _characterViewmodel.Play(AnimState.Down);
        _isLastStand = true;
    }

    public void OnDeath()
    {
        AudioManager.StopLoopSound(ref audioPlayerLoopMove);     
        AudioManager.StopLoopSound(ref audioPlayerLoopLastStand);        
        _isDead = true;
        // TODO : Deplacer les evenement announcer dans son propre script
        AudioManager.PlayAnnouncer("announcer_player_eliminated");
        AudioManager.PlaySoundAtPosition(AliasOnDeath,transform.position);    
        FXManager.PlayFX(_fxDeath,transform.position,BehaviorAfterPlay.DestroyAfterPlay);
        if(damagedFx != null)
            Destroy(damagedFx.gameObject);
        Destroy(gameObject);
    }

    private const int IndexLayerProjectile = 7;
    
    #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 position = transform.position.GetPositionInWorldToViewportPointCamera();
            Handles.Label(transform.position, $" WorldToScreenPoint{position }");
        }
    #endif
    
    public void GiveWeapon(WeaponScriptableObject weapon)
    {
        _weaponInstance = weapon.CreateWeaponInstance(gameObject);
        if(_characterViewmodel.rightHand != null)
            weaponInstance.transform.parent = _characterViewmodel.rightHand.transform;
        weaponInstance.transform.localPosition = Vector3.zero;
        if (weapon.startAmmo != -1)
        {
            _weaponInstance.EventNoAmmo += WeaponInstanceOnEventNoAmmo;
        }
    }

    private void WeaponInstanceOnEventNoAmmo()
    {
        GiveWeapon(primaryWeapon);
    }
}