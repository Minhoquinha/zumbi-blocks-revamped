using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))]

public class PlayerMovement : MonoBehaviour
{

    [Header("Main Movement")]
    [HideInInspector]
    public float CurrentSpeed = 10.0f;
    private float CurrentHeight = 2.5f;
    private float JumpHeight = 4.0f;
    private Vector3 Velocity;
    private Vector3 MovementIntent;
    private bool JumpIntent = false;
    private bool SprintIntent = false;
    private bool CrouchIntent = false;

    [Header("Ground Checking")]
    public Transform GroundCheckerTransform;
    public float GroundCheckerRadius;
    private bool Grounded = false;

    [Header("Main References")]
    [Space(50)]
    public Transform PlayerHeadTransform;
    public PlayerStats Player;
    public CharacterController PlayerController;
    public LayerMask CollisionMask; //A mask that includes all object layers Player collides with.//
    private GameManager GameManagerScript;
    private ControlsManager ControlsManagerScript;

    void Awake ()
    {
        GameManagerScript = FindObjectOfType<GameManager>();
        ControlsManagerScript = GameManagerScript.GetComponent<ControlsManager>();
        Player = GetComponent<PlayerStats>();
        PlayerController = GetComponent<CharacterController>();

        CollisionMask = LayerMaskCollisions.MaskForLayer(gameObject.layer);
    }

    void Start()
    {
        CurrentSpeed = Player.WalkingSpeed;
        CurrentHeight = Player.StandingHeight;
        JumpHeight = Player.JumpHeight;
        MovementIntent = Vector3.zero;
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

        if (Physics.CheckCapsule(GroundCheckerTransform.position, transform.position, GroundCheckerRadius, CollisionMask, QueryTriggerInteraction.Ignore))
        {
            Grounded = true;
        }
        else
        {
            Grounded = false;
        }

        if (!Grounded)
        {
            Velocity.y += Physics.gravity.y * Time.deltaTime;
        }
        else if (Velocity.y < 0f)
        {
            Velocity.y = 0f;
        }

        if (Player.Dead)
        {
            MovementIntent = Vector3.zero;
            CrouchIntent = false;
            SprintIntent = false;
            JumpIntent = false;
            return;
        }

        CrouchIntent = Input.GetKey(ControlsManagerScript.ControlDictionary ["Crouch"]);
        SprintIntent = Input.GetKey(ControlsManagerScript.ControlDictionary ["Sprint"]);
        JumpIntent |= Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["Jump"]);

        float XIntent = 0;
        float ZIntent = 0;
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
        if (PlayerHeadTransform != null)
        {
            Quaternion RotateToLocal = Quaternion.Euler(0f, PlayerHeadTransform.eulerAngles.y, 0f);
            MovementIntent = RotateToLocal * MovementIntent;
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

        PlayerController.height = CurrentHeight;
        Vector3 HeadPosition = new Vector3(transform.position.x, transform.position.y + CurrentHeight / 2f, transform.position.z);
        PlayerHeadTransform.position = HeadPosition;

        PlayerController.Move(CurrentSpeed * Time.deltaTime * MovementIntent);
        PlayerController.Move(Velocity * Time.deltaTime);
    }

    void Jump()
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
}
