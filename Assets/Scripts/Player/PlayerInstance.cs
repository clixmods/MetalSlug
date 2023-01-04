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
 
    private int _health;

    #region Properties

    // Todo : need to be implemented
    public bool IsAlive => true;


    #endregion

    // BOOL
    private bool groundedPlayer;
    private bool jumped = false;
    private bool shooted;
    private bool isLookingUp = false;

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
                // stock the direction on a direction var
                aimDir.x = Mathf.CeilToInt(movementInput.x);
                aimDir.y = Mathf.CeilToInt(movementInput.y);
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
        // switch for set true & false the action
        switch (context.phase)
        {
            case InputActionPhase.Started:
                // Check if the player is crouching
                if (controller.isGrounded)
                {
                    // The player is crouching, so fire in the last direction they were looking
                    weaponInstance.DoFire(aimDir.x > 0 ? Vector2.right : Vector2.left);
                }
                else
                {
                    // The player is not crouching, so check their current direction
                    if (aimDir.x > 0)
                    {
                        // The player is looking right, so fire to the right
                        if (aimDir.y > 0)
                        {
                            // The player is looking up and moving right, so fire upwards
                            weaponInstance.DoFire(Vector2.up);
                        }
                        else if (aimDir.y < 0)
                        {
                            // The player is jumping and looking down, so fire downwards
                            weaponInstance.DoFire(Vector2.down);
                        }
                        else if (controller.velocity.y > 0)
                        {
                            // The player is jumping and not looking up or down, so fire downwards
                            weaponInstance.DoFire(Vector2.down);
                        }
                        else
                        {
                            // The player is not looking up, down, or left, so fire to the right
                            weaponInstance.DoFire(Vector2.right);
                        }
                    }
                    else if (aimDir.x < 0)
                    {
                        // The player is looking left, so fire to the left
                        if (aimDir.y > 0)
                        {
                            // The player is looking up and moving left, so fire upwards
                            weaponInstance.DoFire(Vector2.up);
                        }
                        else if (aimDir.y < 0)
                        {
                            // The player is jumping and looking down, so fire downwards
                            weaponInstance.DoFire(Vector2.down);
                        }
                        else if (controller.velocity.y > 0)
                        {
                            // The player is jumping and not looking up or down, so fire downwards
                            weaponInstance.DoFire(Vector2.down);
                        }
                        else
                        {
                            // The player is not looking up, down, or right, so fire to the left
                            weaponInstance.DoFire(Vector2.left);
                        }
                    }
                    else if (aimDir.y > 0)
                    {
                        // The player is looking up, so fire upwards
                        weaponInstance.DoFire(Vector2.up);
                        isLookingUp = true;
                    }
                    else
                    {
                        // The player is either looking down or not looking left, right, or up, so check their vertical velocity
                        if (controller.velocity.y > 0 && aimDir.y < 0)
                        {
                            // The player is jumping and looking down, so fire downwards
                            weaponInstance.DoFire(Vector2.down);
                        }
                        else if (isLookingUp)
                        {
                            // The player is on the ground and was previously looking up, so fire in the last direction they were looking
                            weaponInstance.DoFire(aimDir.x > 0 ? Vector2.right : Vector2.left);
                            isLookingUp = false;
                        }
                        else
                        {
                            // The player is on the ground and was not previously looking up, so fire in the direction they are currently looking
                            weaponInstance.DoFire(aimDir.x > 0 ? Vector2.right : Vector2.left);
                        }
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
        if (movementInput.y == 0)
        {
            aimDir.y = 0;
        }
        // check if the player is grounded
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            // set the velocity to 0
            playerVelocity.y = 0f;
        }

        /*if (groundedPlayer && aimDir.y == -1)
        {
            aimDir.y = 0f;
        }*/

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