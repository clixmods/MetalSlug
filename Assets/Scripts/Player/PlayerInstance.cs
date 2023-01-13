using AudioAliase;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
[RequireComponent(typeof(CharacterController))]
public class PlayerInstance : MonoBehaviour , IActor
{
    #region Events
    public delegate void PlayerEvent(PlayerInstance newPlayer);
    
    public static event PlayerEvent eventPlayerJoin;
    public static event PlayerEvent eventPlayerDisconnect;
    public static event PlayerEvent eventPlayerDeath;
    public static event PlayerEvent eventPlayerRespawn;
    public static event PlayerEvent eventPlayerRevive;
    public static event PlayerEvent eventPlayerFire;
    public static event PlayerEvent eventIsReviving;
    #endregion

    // REFS DE SCRIPTS
    [Header("Weapon")]
    [SerializeField] private WeaponScriptableObject primaryWeapon;
    [SerializeField] private WeaponScriptableObject grenadeWeapon;
    public WeaponInstance weaponInstance;
    public WeaponInstance grenadeInstance;
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
    public bool IsQuandilsoigne => _isQuandilsoigne;
    public bool IsLastStand => _isLastStand;
    public bool IsQuandilsefaitrevive => _isQuandilsefaitrevive;

    [SerializeField] private TeamEnum _team;
    private CharacterViewmodelManager _characterViewmodel;

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
        if (_characterViewmodel.skinnedMeshRenderer != null)
        {
            _fxHit= FXManager.InitFX(FXHit,transform.position,gameObject, _characterViewmodel.skinnedMeshRenderer);
            _fxDamaged= FXManager.InitFX(FXLoopDamaged,transform.position,gameObject, _characterViewmodel.skinnedMeshRenderer);

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
        weaponInstance = primaryWeapon.CreateWeaponInstance(gameObject);
        if(_characterViewmodel.rightHand != null)
            weaponInstance.transform.parent = _characterViewmodel.rightHand.transform;
        weaponInstance.transform.localPosition = Vector3.zero;
        
        
        grenadeInstance = grenadeWeapon.CreateWeaponInstance(gameObject);
        
        if(_characterViewmodel.leftHand != null)
            grenadeInstance.transform.parent = _characterViewmodel.leftHand.transform;
        
        grenadeInstance.transform.localPosition = Vector3.zero;
        
    }

    // start
    private void Start()
    {
        if (!_isSpawned)
        {
            controller = gameObject.GetComponent<CharacterController>();
       
            eventPlayerJoin?.Invoke(this);
            _isSpawned = true;
            Parachute();
            eventPlayerRespawn?.Invoke(this);
            timerInvulnerability = 3;
        }
    }

    // OnDestroy
    private void OnDestroy()
    {
        eventPlayerDisconnect?.Invoke(this);
    }

    // OnTriggerEnter 
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
            //todo UI active
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
        if (LevelManager.Instance.State != State.Ingame)
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
            if (_currentMovementInput.y < 0 && controller.isGrounded)
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
                if (_currentMovementInput.y < 0 && controller.isGrounded)
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
                else if (_currentMovementInput.y == 0 && _currentMovementInput.x == 0 && controller.isGrounded)
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
                else if (_currentMovementInput.y == 0 && _currentMovementInput.x == 0 && !controller.isGrounded)
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

    public void Parachute()
    {
        Teleport(new Vector3(RoundManager.PlayerSpawnActive.position.x, 10f, 0f));
        _gravityValue = -2f;
        _parachute.SetActive(true);
        _firstSpawn = false;
    }

    // update
    void Update()
    {
        if (!_isLastStand)
        {
            _isQuandilsefaitrevive = false;
        }
        if (_playerQueJeSoigne != null && !_playerQueJeSoigne._isLastStand)
        {
            _playerQueJeSoigne = null;
            _isQuandilsoigne = false;
        }
        if (transform.position.y < -10)
        {
            OnDeath();
        }
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
            _isQuandilsoigne = false;
            _timerDeath -= Time.deltaTime;
        }

        if (_timerDeath <= 0 || LevelManager.GetAlivePlayers.Count == 0)
        {
            OnDeath();
            return;
        }


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
            _groundedPlayer = controller.isGrounded;
            if (_groundedPlayer && _playerVelocity.y < 0)
            {
                _characterViewmodel._animator.SetBool("IsFalling", false);
                _parachute.SetActive(false);
                // set the velocity to 0
                _playerVelocity.y = 0f;
                if(_gravityValue != -20f)
                {
                    _gravityValue = -20f;
                }
                // if the player jump and crouch while in the air and arrive on the ground crouched then isCrouching is set to true
                if (_currentMovementInput.y < 0 && controller.isGrounded)
                {
                    _isCrouching = true;
                }
            }
            else
            {
                _characterViewmodel._animator.SetBool("IsFalling", true);
            }

            // move the player
            Vector3 move = new Vector3(_movementInput.x, 0, 0);

            var motion = move.normalized * Time.deltaTime * _playerSpeed;

            // Check if the object is out of the camera
            Vector3 position = Camera.main.WorldToViewportPoint(transform.position + motion);

            bool isOutCameraNegative = position.x < 0.1f || position.y < 0.1f;
            bool isOutCameraPositive = position.x > 0.9f || position.y > 0.9f;

            if (!(isOutCameraNegative || isOutCameraPositive) )
            {
                controller.Move(motion);
                if (motion.magnitude > 0)
                {
                    _characterViewmodel.Play(AnimState.Move);
                }
                else
                {
                    _characterViewmodel.Play(AnimState.Idle);
                }
            }
            else
            {
                if (isOutCameraNegative)
                {
                    controller.Move(Vector3.right * Time.deltaTime * _playerSpeed);
                    _characterViewmodel.Play(AnimState.Move);
                   
                }
                if (isOutCameraPositive)
                {
                    controller.Move(Vector3.left * Time.deltaTime * _playerSpeed);
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
        // motion
        controller.Move(_playerVelocity * Time.deltaTime);
    }

    public void Teleport(Vector3 position)
    {
        controller.enabled = false;
        transform.position = position;
        controller.enabled = true;
    }

    public void SetSleep(bool value)
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
  

    public void Revive()
    {
        eventPlayerRevive?.Invoke(this);
        _characterViewmodel.Play(AnimState.Revived);
        _isLastStand = false;
        _jumped = false;
        Health = _startHealth;
        timerInvulnerability = 1.5f;
        if(damagedFx != null)
            Destroy(damagedFx.gameObject);
    }
 
    public TeamEnum Team => _team;
    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            if (timerInvulnerability > 0)
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
    public void OnDown()
    {
        if (LevelManager.Instance.players.Count == 1 || LevelManager.Instance.ReviveAmount <= 0)
        {
            OnDeath();
            return;
        }
        AudioManager.PlaySoundAtPosition("announcer_player_down", Vector3.zero);
        damagedFx = FXManager.PlayFX(_fxDamaged,transform.position,BehaviorAfterPlay.Nothing);
        _characterViewmodel.Play(AnimState.Down);
        _isLastStand = true;
    }

    public void OnDeath()
    {
        AudioManager.PlaySoundAtPosition("announcer_player_death", Vector3.zero);
        
        FXManager.PlayFX(_fxDeath,transform.position,BehaviorAfterPlay.DestroyAfterPlay);
        if(damagedFx != null)
            Destroy(damagedFx.gameObject);
        Destroy(gameObject);
    }

    private const int IndexLayerProjectile = 7;
    
    #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 position = Camera.main.WorldToViewportPoint(transform.position);
            Handles.Label(transform.position, $" WorldToScreenPoint{position }");
        }
    #endif
}