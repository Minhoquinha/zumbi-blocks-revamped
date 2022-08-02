using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerStats))]

public class PlayerControlOldV2 : MonoBehaviour
{
    [Header("Main Movement")]
    public bool DefaultGravity = true;
    public float Gravity = 10.0f;
    public bool AllowAirControl = true;
    public bool AllowJumping = true;

    private float XIntent = 0f;
    private float ZIntent = 0f;
    private bool JumpIntent = false;
    public float JumpCooldown = 0.2f;
    private float CurrentJumpCooldown = 0.2f;
    private bool SprintIntent = false;
    private bool CrouchIntent = false;

    [HideInInspector]
    public float CurrentSpeed = 10.0f;
    private float Acceleration = 10.0f;
    private float JumpHeight = 4.0f;

    [Header("Ground Checking")]
    public Transform GroundCheck;
    public Vector3 GroundCheckSize;
    public float SlopeLimit = 0.48f; //How vertical a slope can be for the player to climb. 1f means player can only move on perfect flat terrain, 0f means player can climb walls.//
    private bool Colliding = false;
    private bool Grounded = false;

    [Header("Main References")]
    [Space(50)]
    public Transform LookTransform;
    private PlayerStats Player;
    private Rigidbody PlayerRigidbody;
    public LayerMask SolidLayer;

    private GameManager GameManagerScript;
    private ControlsManager ControlsManagerScript;

    void Awake()
    {
        GameManagerScript = FindObjectOfType<GameManager>();
        ControlsManagerScript = GameManagerScript.GetComponent<ControlsManager>();
        Player = GetComponent<PlayerStats>();
        PlayerRigidbody = GetComponent<Rigidbody>();
        PlayerRigidbody.freezeRotation = true;
        PlayerRigidbody.useGravity = false;
        if (DefaultGravity)
        {
            Gravity = Physics.gravity.y;
        }
    }

    void Start()
    {
        CurrentSpeed = Player.WalkingSpeed;
        Acceleration = Player.Acceleration;
        JumpHeight = Player.JumpHeight;
        XIntent = 0;
        ZIntent = 0;
        Grounded = true;
        JumpIntent = false;
        Debug.Log(this.name + " loaded;");
    }

    void Update()
    {
        if (GameManagerScript.GameStatus != GameManager.GameState.InGame)
        {
            return;
        }

        if (Player.Dead)
        {
            XIntent = 0;
            ZIntent = 0;
            CrouchIntent = false;
            SprintIntent = false;
            return;
        }

        CrouchIntent = Input.GetKey(ControlsManagerScript.ControlDictionary["Crouch"]);
        SprintIntent = Input.GetKey(ControlsManagerScript.ControlDictionary ["Sprint"]);
        if (AllowJumping)
        {
            JumpIntent = Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["Jump"]);
        }

        XIntent = 0;
        ZIntent = 0;
        if (Colliding || AllowAirControl)
        {
            if (Input.GetKey(ControlsManagerScript.ControlDictionary ["MoveRight"]))
            {
                XIntent = 1;
            }
            else if (Input.GetKey(ControlsManagerScript.ControlDictionary ["MoveLeft"]))
            {
                XIntent = -1;
            }

            if (Input.GetKey(ControlsManagerScript.ControlDictionary ["MoveForward"]))
            {
                ZIntent = 1;
            }
            else if (Input.GetKey(ControlsManagerScript.ControlDictionary ["MoveBackward"]))
            {
                ZIntent = -1;
            }
        }

        if (CurrentJumpCooldown > 0f)
        {
            CurrentJumpCooldown -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        /*
        if (Physics.CheckBox(GroundCheck.position, GroundCheckSize, GroundCheck.rotation))
        {
            Grounded = true;
        }
        else
        {
            Grounded = false;
        }
        */
        if (Colliding || AllowAirControl)
        {
            // Calculate how fast we want to be moving, normalized//
            Vector3 TargetVelocity = new Vector3 (XIntent, 0f, ZIntent);

            if (LookTransform)
            {
                Quaternion RotateToLocal = Quaternion.Euler(0, LookTransform.eulerAngles.y, 0);
                TargetVelocity = RotateToLocal * TargetVelocity;
            }

            if (Colliding)
            {
                if (CrouchIntent)
                {
                    CurrentSpeed = Player.CrouchingSpeed;
                    Player.PlayerMovementStatus = PlayerStats.PlayerState.Crouching;
                }
                else if (SprintIntent && Player.CurrentStamina > 0f)
                {
                    if ((Player.CurrentStamina >= Player.MinStamina) || (Player.PlayerMovementStatus == PlayerStats.PlayerState.Sprinting))
                    {
                        CurrentSpeed = Player.SprintingSpeed;
                        Player.PlayerMovementStatus = PlayerStats.PlayerState.Sprinting;
                    }

                    if (Player.CurrentStamina <= 0f)
                    {
                        CurrentSpeed = Player.WalkingSpeed;
                        Player.PlayerMovementStatus = PlayerStats.PlayerState.Walking;
                    }
                }
                else
                {
                    CurrentSpeed = Player.WalkingSpeed;
                    Player.PlayerMovementStatus = PlayerStats.PlayerState.Walking;
                }
            }

            //Main movement shenanigans//
            TargetVelocity *= CurrentSpeed;

            Vector3 Velocity = PlayerRigidbody.velocity;
            Vector3 VelocityChange = (TargetVelocity - Velocity);

            VelocityChange.x = Mathf.Clamp(VelocityChange.x, -Acceleration, Acceleration);
            VelocityChange.z = Mathf.Clamp(VelocityChange.z, -Acceleration, Acceleration);
            VelocityChange.y = 0f;

            PlayerRigidbody.AddForce(VelocityChange, ForceMode.VelocityChange);
            //Main movement shenanigans//

            if (JumpIntent && CurrentJumpCooldown <= 0f && Colliding && Grounded && Player.CurrentStamina >= Player.MinStamina)
            {
                Velocity = PlayerRigidbody.velocity;
                Velocity.y = CalculateJump();
                PlayerRigidbody.velocity = Velocity;

                if (Player.CurrentNoise < Player.JumpingNoise)
                {
                    Player.CurrentNoise = Player.JumpingNoise;
                }
                Player.CurrentStamina -= Player.JumpingStaminaCost;
                Player.PlayerMovementStatus = PlayerStats.PlayerState.Jumping;
                CurrentJumpCooldown = JumpCooldown;
            }

            if (Velocity == Vector3.zero)
            {
                Player.PlayerMovementStatus = PlayerStats.PlayerState.Standing;
            }
            else if (Velocity.y < 0f && !Grounded)
            {
                Player.PlayerMovementStatus = PlayerStats.PlayerState.Falling;
            }
        }

        PlayerRigidbody.AddForce(0f, Gravity, 0f, ForceMode.Acceleration);
        Colliding = false;
    }
    void OnCollisionStay (Collision ObjectColliding)
    {
        Colliding = true;

        EvaluateCollision(ObjectColliding);
    }

    void EvaluateCollision (Collision ObjectColliding)
    {
        for (int i = 0; i < ObjectColliding.contactCount; i++)
        {
            Vector3 ObjectNormal = ObjectColliding.GetContact(i).normal;

            if (ObjectNormal.y >= SlopeLimit)
            {
                Grounded = true;
            }
            else
            {
                Grounded = false;
            }
        }
    }

    float CalculateJump()
    {
        return Mathf.Sqrt(2 * JumpHeight * -Gravity);
    }

    Vector3 ProjectOnPlane(Vector3 Vector, Vector3 Plane)
    {
        return Vector - Plane * Vector3.Dot(Vector, Plane);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(GroundCheck.position, GroundCheckSize);
    }
}
