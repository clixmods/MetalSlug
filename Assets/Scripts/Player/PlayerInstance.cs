using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerInstance : MonoBehaviour , IActor
{
    #region Events
    //Event executed when player join
    public delegate void CallbackOnJoin( PlayerInstance newPlayer);
    public static event CallbackOnJoin eventPlayerJoin;
    
    public delegate void CallbackOnDisconnect( PlayerInstance newPlayer);
    public static event CallbackOnDisconnect eventPlayerDisconnect;

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
 
    private int _health;

    #region Properties

    // Todo : need to be implemented
    public bool IsAlive => true;


    #endregion

    // BOOL
    private bool groundedPlayer;
    private bool jumped = false;
    private bool shooted;

    // start
    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        eventPlayerJoin?.Invoke(  this);
        
        weaponInstance.Owner = gameObject;
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
                    // If the player is on the ground and pressing S, don't shoot.
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
                else if (lastDirection < 0)
                {
                    // If the player's last movement direction was to the left, shoot left.
                    weaponInstance.DoFire(Vector2.left);
                }
                else if (lastDirection > 0)
                {
                    // If the player's last movement direction was to the right, shoot right.
                    weaponInstance.DoFire(Vector2.right);
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
        }

        // move the player
        Vector3 move = new Vector3(movementInput.x, 0, 0);
        controller.Move(move.normalized * Time.deltaTime * playerSpeed);

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


    [SerializeField] private TeamEnum _team;
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

    public void OnDeath()
    {
        throw new NotImplementedException();
    }
}