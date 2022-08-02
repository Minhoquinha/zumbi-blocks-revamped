using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerStats))]

public class PlayerMovementOldV2 : MonoBehaviour
{
    [Header("Main Movement")]
    public float MaxAcceleration = 10.0f;
    public float MaxAerialAcceleration = 4.0f;
    [HideInInspector]
    public float CurrentSpeed = 10.0f;
    private float CurrentHeight = 2.5f;
    private float JumpHeight = 4.0f;
    private Vector3 Velocity;
    private Vector3 MovementIntent;
    private float XIntent = 0f;
    private float ZIntent = 0f;
    private bool JumpIntent = false;
    private bool SprintIntent = false;
    private bool CrouchIntent = false;

    [Header("Ground Checking")]
    public float SlopeMinLimit = 0.48f; //How vertical a slope can be for the player to climb. 1f means player can only move on perfect flat terrain, 0f means player can climb walls.//
    private bool Grounded = false;

    [Header("Main References")]
    [Space(50)]
    public Transform LookTransform;
    private PlayerStats Player;
    private Rigidbody PlayerRigidbody;
    public CapsuleCollider PlayerCollider;

    public LayerMask CollisionMask; //A mask that includes all object layers Player does not collide with.//
    private GameManager GameManagerScript;
    private ControlsManager ControlsManagerScript;

    void Awake()
    {
        GameManagerScript = FindObjectOfType<GameManager>();
        ControlsManagerScript = GameManagerScript.GetComponent<ControlsManager>();
        Player = GetComponent<PlayerStats>();
        PlayerRigidbody = GetComponent<Rigidbody>();
        PlayerRigidbody.freezeRotation = true;

        CollisionMask = LayerMaskCollisions.MaskForLayer(gameObject.layer);
    }

    void Start()
    {
        CurrentSpeed = Player.WalkingSpeed;
        CurrentHeight = Player.StandingHeight;
        JumpHeight = Player.JumpHeight;
        MaxAcceleration = Player.Acceleration;
        MaxAerialAcceleration = Player.AerialAcceleration;
        XIntent = 0;
        ZIntent = 0;
        Grounded = true;
        JumpIntent = false;
        Debug.Log(this.name + " loaded;");
    }

    // Update is called once per frame
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

        CrouchIntent = Input.GetKey(ControlsManagerScript.ControlDictionary ["Crouch"]);
        SprintIntent = Input.GetKey(ControlsManagerScript.ControlDictionary ["Sprint"]);
        JumpIntent |= Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["Jump"]);

        XIntent = 0;
        ZIntent = 0;
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

        MovementIntent = new Vector3(XIntent, 0f, ZIntent);
    }

    void FixedUpdate()
    {
        if (Player.Dead)
        {
            PlayerRigidbody.freezeRotation = false;
            return;
        }

        Velocity = PlayerRigidbody.velocity;

        if (LookTransform)
        {
            Quaternion RotateToLocal = Quaternion.Euler(0, LookTransform.eulerAngles.y, 0);
            MovementIntent = RotateToLocal * MovementIntent;
        }

        if (Physics.Raycast(transform.position, -transform.up, (Player.StandingHeight / 2f + 0.1f), CollisionMask))
        {
            Grounded = true;
        }

        if (Grounded)
        {
            if (Physics.Raycast(transform.position, transform.up, (Player.StandingHeight / 2f + 0.1f), CollisionMask))
            {
                CurrentSpeed = Player.CrouchingSpeed;
                CurrentHeight = Player.CrouchingHeight;
                Player.PlayerMovementStatus = PlayerStats.PlayerState.Crouching;
            }
            else if (CrouchIntent)
            {
                CurrentSpeed = Player.CrouchingSpeed;
                CurrentHeight = Player.CrouchingHeight;
                Player.PlayerMovementStatus = PlayerStats.PlayerState.Crouching;
            }
            else if (MovementIntent == Vector3.zero)
            {
                CurrentHeight = Player.StandingHeight;
                Player.PlayerMovementStatus = PlayerStats.PlayerState.Standing;
            }
            else if (SprintIntent && Player.CurrentStamina > 0f)
            {
                if ((Player.CurrentStamina >= Player.MinStamina) || (Player.PlayerMovementStatus == PlayerStats.PlayerState.Sprinting))
                {
                    CurrentSpeed = Player.SprintingSpeed;
                    CurrentHeight = Player.StandingHeight;
                    Player.PlayerMovementStatus = PlayerStats.PlayerState.Sprinting;
                }

                if (Player.CurrentStamina <= 0f)
                {
                    CurrentSpeed = Player.WalkingSpeed;
                    CurrentHeight = Player.StandingHeight;
                    Player.PlayerMovementStatus = PlayerStats.PlayerState.Walking;
                }
            }
            else
            {
                CurrentSpeed = Player.WalkingSpeed;
                CurrentHeight = Player.StandingHeight;
                Player.PlayerMovementStatus = PlayerStats.PlayerState.Walking;
            }
        }
        else
        {
            if (Player.PlayerMovementStatus != PlayerStats.PlayerState.Jumping || Velocity.y < 0f)
            {
                CurrentSpeed = Player.WalkingSpeed;
                Player.PlayerMovementStatus = PlayerStats.PlayerState.Falling;
            }
        }

        if (JumpIntent)
        {
            JumpIntent = false;
            Jump();
        }

        PlayerCollider.height = CurrentHeight;

        //Main movement shenanigans//
        Vector3 TargetVelocity = MovementIntent * CurrentSpeed;
        float CurrentMaxAcceleration = (Grounded ? MaxAcceleration : MaxAerialAcceleration);

        Velocity.x = Mathf.MoveTowards(Velocity.x, TargetVelocity.x, CurrentMaxAcceleration);
        Velocity.z = Mathf.MoveTowards(Velocity.z, TargetVelocity.z, CurrentMaxAcceleration);

        PlayerRigidbody.velocity = Velocity;
        //Main movement shenanigans//

        if (PlayerRigidbody.IsSleeping())
        {
            Grounded = true;
        }
        else
        {
            Grounded = false;
        }
    }

    void Jump ()
    {
        if (Grounded && Player.CurrentStamina >= Player.MinStamina)
        {
            float JumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * JumpHeight);
            if (Velocity.y > 0f)
            {
                JumpSpeed = Mathf.Max(JumpSpeed - Velocity.y, 0f);
            }
            Velocity.y += JumpSpeed;

            Player.CurrentNoise = Mathf.Max(Player.JumpingNoise, Player.CurrentNoise);
            Player.CurrentStamina -= Player.JumpingStaminaCost;
            Player.PlayerMovementStatus = PlayerStats.PlayerState.Jumping;
        }
    }

    void OnCollisionEnter (Collision ObjectCollision)
    {
        EvaluateCollision (ObjectCollision);
    }

    void OnCollisionStay (Collision ObjectCollision)
    {
        EvaluateCollision (ObjectCollision);
    }

    void EvaluateCollision (Collision ObjectCollision)
    {
        for (int i = 0; i < ObjectCollision.contactCount; i++)
        {
            Vector3 ObjectNormal = ObjectCollision.GetContact(i).normal;

            Grounded |= ObjectNormal.y >= SlopeMinLimit;
        }
    }
}
