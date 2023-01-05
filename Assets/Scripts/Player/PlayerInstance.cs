using Cinemachine;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

[RequireComponent(typeof(CharacterController))]
public class PlayerInstance : MonoBehaviour , IActor
{
    #region Events
    public delegate void PlayerEvent(PlayerInstance newPlayer);
    public static event PlayerEvent eventPlayerJoin;
    public static event PlayerEvent eventPlayerDisconnect;
    public static event PlayerEvent eventPlayerDeath;
    public static event PlayerEvent eventPlayerRespawn;
    #endregion

    // REFS DE SCRIPTS
    [SerializeField]
    private WeaponInstance weaponInstance;

    // REFS DE GO
    [SerializeField]
    private GameObject cameraTarget;

    // FLOAT & INT
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 3.0f;
    [SerializeField]
    private float gravityValue = -9.81f;

    [SerializeField]
    private int lastDirection;

    // RB
    private CharacterController controller;

    // VECTOR
    private Vector3 playerVelocity;
    private Vector2 movementInput = Vector2.zero;
    private Vector2 aimDir;

    private Vector2 currentMovementInput;
 
    private int _health = 1;

    #region Properties

    // Todo : need to be implemented
    public bool IsAlive => gameObject.activeSelf;


    #endregion

    // BOOL
    private bool groundedPlayer;
    private bool jumped = false;
    private bool shooted;
    private bool isCrouching = false;
    
    [SerializeField] private TeamEnum _team;
    private bool isSpawned;
    private CharacterViewmodelManager _characterViewmodel;

    private void Awake()
    {
        _characterViewmodel = GetComponent<CharacterViewmodelManager>();
    }

    // start
    private void Start()
    {
        if (!isSpawned)
        {
            controller = gameObject.GetComponent<CharacterController>();
       
            eventPlayerJoin?.Invoke(this);
        
            weaponInstance.Owner = gameObject;
            isSpawned = true;
        }
        
    }

    private void OnDestroy()
    {
        eventPlayerDisconnect?.Invoke(this);
    }

    // move
    public void OnMove(InputAction.CallbackContext context)
    {
        // readValue via InputSystem
        movementInput = context.ReadValue<Vector2>();
        if (context.phase == InputActionPhase.Performed)
        {
            if (!(movementInput.x == 0 && movementInput.y == 0))
            {
                // Update the current movement input.
                currentMovementInput = movementInput;

                // Update the last direction the player moved in.
                if (currentMovementInput.x < 0)
                {
                    lastDirection = -1;
                }
                else if (currentMovementInput.x > 0)
                {
                    lastDirection = 1;
                }
            }
            else
            {
                // If the player is not moving, reset the current movement input.
                currentMovementInput = Vector2.zero;
            }

            // Update the crouching state.
            if (currentMovementInput.y < 0 && controller.isGrounded)
            {
                isCrouching = true;
            }
            else
            {
                isCrouching = false;
            }
           
        }
    }

    // jump
    public void OnJump(InputAction.CallbackContext context)
    {
        // switch for set true & false the action
        switch(context.phase)
        {
            case InputActionPhase.Performed:
                jumped = true;
                break;
            case InputActionPhase.Canceled:
                jumped = false;
                break;
            default:
                break;
        }
    }

    // shoot
    public void OnShoot(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                // Check the direction to shoot based on the player's current movement input and whether they are in the air.
                if (currentMovementInput.y < 0 && controller.isGrounded)
                {
                    // If the player is on the ground and pressing S, shoot the last direction.
                    if (lastDirection < 0)
                    {
                        // If the player was moving left, shoot left.
                        weaponInstance.DoFire(Vector2.left);
                    }
                    else if (lastDirection > 0)
                    {
                        // If the player was moving right, shoot right.
                        weaponInstance.DoFire(Vector2.right);
                    }
                    break;
                }
                else if (currentMovementInput.y < 0 || aimDir.y < 0)
                {
                    // If the player is in the air and pressing S, or if they are in the air and aiming downwards, shoot downwards.
                    weaponInstance.DoFire(Vector2.down);
                }
                else if (currentMovementInput.y > 0)
                {
                    // If the player is pressing W, shoot upwards.
                    weaponInstance.DoFire(Vector2.up);
                }
                else if (currentMovementInput.x < 0)
                {
                    // If the player is pressing A, shoot left.
                    weaponInstance.DoFire(Vector2.left);
                }
                else if (currentMovementInput.x > 0)
                {
                    // If the player is pressing D, shoot right.
                    weaponInstance.DoFire(Vector2.right);
                }
                else if (currentMovementInput.y == 0 && currentMovementInput.x == 0 && controller.isGrounded)
                {
                    // If the player is not moving and on the ground, check the last direction they moved in.
                    if (lastDirection < 0)
                    {
                        // If the player was moving left, shoot left.
                        weaponInstance.DoFire(Vector2.left);
                    }
                    else if (lastDirection > 0)
                    {
                        // If the player was moving right, shoot right.
                        weaponInstance.DoFire(Vector2.right);
                    }
                }
                break;

            case InputActionPhase.Performed:
                shooted = true;
                break;

            case InputActionPhase.Canceled:
                shooted = false;
                break;

            default:
                break;
        }
    }

    // update
    void Update()
    {
        // check if the player is grounded
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            // set the velocity to 0
            playerVelocity.y = 0f;
            // if the player jump and crouch while in the air and arrive on the ground crouched then isCrouching is set to true
            if (currentMovementInput.y < 0 && controller.isGrounded)
            {
                isCrouching = true;
            }
        }

        // move the player
        Vector3 move = new Vector3(movementInput.x, 0, 0);

        var motion = move.normalized * Time.deltaTime * playerSpeed;
        // Check if the object is out of the camera
        Vector3 position = Camera.main.WorldToViewportPoint(transform.position + motion);
        
        bool isOutCameraNegative = position.x < 0.1f || position.y < 0.1f;
        bool isOutCameraPositive =  position.x > 0.9f || position.y > 0.9f;
        if(!(isOutCameraNegative || isOutCameraPositive) )
        {
            controller.Move(motion);
        }
        else
        {
            if (isOutCameraNegative)
            {
                controller.Move(Vector3.right * Time.deltaTime * playerSpeed);
            }
            if (isOutCameraPositive)
            {
                controller.Move(Vector3.left * Time.deltaTime * playerSpeed);
            }
        }
        _characterViewmodel.Direction = transform.position + motion;
        
        

        // Changes the height position of the player..
        if (jumped && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        // add gravity to the player
        playerVelocity.y += gravityValue * Time.deltaTime;
        // motion
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Teleport(Vector3 position)
    {
        controller.enabled = false;
        transform.position = position;
        controller.enabled = true;
    }


 
    public TeamEnum Team => _team;
    public int Health => _health;
    public void DoDamage(int amount)
    {  
        _health -= amount;
        if (_health <= 0)
        {
            OnDeath();
        }
    }

    public void OnEnable()
    {
        _health = 1;
    }
    
    public void OnDeath()
    {
        //gameObject.SetActive(false);
    }
    private const int IndexLayerProjectile = 7;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player hit");
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
    
    #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 position = Camera.main.WorldToViewportPoint(transform.position);
            Handles.Label(transform.position, $" WorldToScreenPoint{position }");
        }
    #endif
}