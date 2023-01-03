using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerInstance : MonoBehaviour
{
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

    // BOOL
    private bool groundedPlayer;
    private bool jumped = false;
    private bool shooted;

    // start
    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }

    // move
    public void OnMove(InputAction.CallbackContext context)
    {
        // readValue via InputSystem
        movementInput = context.ReadValue<Vector2>();
        if (context.phase == InputActionPhase.Performed)
        {
            // stock the direction on a direction var
            aimDir.x = Mathf.CeilToInt(movementInput.x);
            aimDir.y = Mathf.CeilToInt(movementInput.y);
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
                if (movementInput.x == 0 && lastDirection == 0 && movementInput.y ==0)
                {
                    weaponInstance.DoFire(transform.right);
                }
                else if (movementInput.x == 0 && lastDirection == 1 && movementInput.y == 0)
                {
                    weaponInstance.DoFire(-transform.right);
                }
                else
                {
                    // clamp the direction vector x to 0 if y > 1
                    if (aimDir.y > 0)
                    { aimDir.x = 0; }
                    // shoot
                    weaponInstance.DoFire(aimDir);
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

        // last direction value
        
        if (movementInput.x >= .7f)
        {
            lastDirection = 0;
        }
        else if (movementInput.x <= -.7f)
        {
            lastDirection = 1;
        }
    }
}