using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlOld : MonoBehaviour
{
    [Header("Noise Properties")]

    [SerializeField]
    private float StandingNoise; //How much noise the player makes when standing still//
    [SerializeField]
    private float FallingNoise; //How much noise the player makes when hitting the ground with enough speed//
    [SerializeField]
    private float WalkingNoise; //How much noise the player makes while walking//
    [SerializeField]
    private float JumpingNoise; //How much noise the player makes when jumping//
    [SerializeField]
    private float SprintingNoise; //How much noise the player makes while sprinting//
    [SerializeField]
    private float CrouchingNoise; //How much noise the player makes while crouching//
    private float CurrentNoise; //How much noise the player is making at the moment, changes if player is crouching, sprinting, etc//

    [Header("Spread Properties")]

    [SerializeField]
    private float StandingSpreadMultiplier; //How accurate the player gun is while it is standing still//
    [SerializeField]
    private float FallingSpreadMultiplier; //How accurate the player gun is while it is falling//
    [SerializeField]
    private float WalkingSpreadMultiplier; //How accurate the player gun is while it is walking//
    [SerializeField]
    private float JumpingSpreadMultiplier; //How accurate the player gun is while it is jumping//
    [SerializeField]
    private float SprintingSpreadMultiplier; //How accurate the player gun is while it is sprinting//
    [SerializeField]
    private float CrouchingSpreadMultiplier; //How accurate the player gun is while it is crouching//
    private float CurrentSpreadMultiplier; //How accurate the player gun is at the moment, changes if player is crouching, sprinting, etc//

    [Header("Speed Properties")]

    public float XIntent;
    public float ZIntent;
    public bool JumpIntent;
    public bool AllowAirControl;
    public float MaxVelocityChange;
    public float WalkingSpeed; //The player speed for when the player is just walking//
    public float SprintingSpeed; //The player speed for when the player is sprinting//
    public float CrouchingSpeed; //The player speed for when the player is crouching//
    private float CurrentSpeed = 400f; //The player speed variable being used at the moment, changes if player is crouching, sprinting, etc//
    private Vector3 GeneralVelocity;
    [SerializeField]
    private float HarmfulVelocity = 500f; //Any values of velocity higher than this are going to harm the player//
    [SerializeField]
    private float VelocityDamage; //How much damage the player gets for each + 10 m/s of harmful velocity//

    [Header("Stamina Mechanics")]

    [SerializeField]
    private float MinStamina; //The minimum stamina necessary to start running or jumping//
    [SerializeField]
    private float StandingStaminaGain; //How much stamina is gained while not sprinting//
    [SerializeField]
    private float WalkingStaminaGain; //How much stamina is gained while walking//
    [SerializeField]
    private float CrouchingStaminaGain; //How much stamina is gained while crouching//
    [SerializeField]
    private float SprintingStaminaGain; //How much stamina is gained while sprinting//
    [SerializeField]
    private float JumpingStaminaGain; //How much stamina the player gains when jumping//
    [SerializeField]
    private float FallingStaminaGain; //How much stamina is gained while falling//

    [HideInInspector]
    public float Stamina;
    private float CurrentStamina;

    [Header("Dimensions")]

    [SerializeField]
    private float Radius = 0.6f;
    [SerializeField]
    private float Height = 3f;
    [SerializeField]
    private float CrouchingHeight = 1.8f; //Height of the player while crouching//
    [SerializeField]
    private float JumpingHeight = 1f; //How high the player jumps//

    [SerializeField]
    private float WalkingSlopeLimit = 45.0f; //How low the angle of a slope can be before the player cant climb it anymore//
    [SerializeField]
    private float JumpingSlopeLimit = 100.0f;
    private float CurrentSlopeLimit = 45.0f; //The player slope limit variable being used at the moment, changes if player is jumping, sprinting, etc//

    [SerializeField]
    private float WalkingStepLimit = 0.2f; //How high a step can be before the player cant climb it anymore//
    [SerializeField]
    private float JumpingStepLimit = 0.4f;
    private float CurrentStepLimit = 0.2f; //The player step limit variable being used at the moment, changes if player is jumping, sprinting, etc//

    private Vector3 LastPosition; //This player last position//

    [Header("Ground Checking")] //These variables are used for checking if player is grounded//

    public Transform GroundCheck;
    public LayerMask GroundMask;

    [Header("Main References")]

    public Rigidbody PlayerRigidbody;
    public Transform PlayerBody;
    public PlayerStats Player;
    public Guns PlayerGun;
    public Melee PlayerMelee;

    public enum PlayerState
    {
        Standing, Walking, Sprinting, Crouching, Falling, Jumping
    }
    [SerializeField]
    private PlayerState PlayerStatus;

    void Start()
    {
        Player = GetComponent<PlayerStats>();
        PlayerGun = GetComponentInChildren<Guns>();
        PlayerRigidbody = GetComponent<Rigidbody>();
        PlayerStatus = PlayerState.Standing;
        GeneralVelocity = Vector3.zero;
        CurrentStamina = Player.Stamina;

        Debug.Log(this.name + " loaded;");
    }

    void Update()
    {
        if (Player.Dead)
        {
            return;
        }

        bool Grounded = Physics.CheckSphere(GroundCheck.position, Radius, GroundMask); //Checks if player is currently on the ground or airborne//

        if (Grounded && PlayerRigidbody.velocity.y < 0f)
        {
            if (-PlayerRigidbody.velocity.y >= HarmfulVelocity)
            {
                float FallDamage = VelocityDamage * (-PlayerRigidbody.velocity.y / 10f);
                Player.Hurt(FallDamage);

                if (Player.CurrentNoise < FallingNoise)
                {
                    Player.CurrentNoise = FallingNoise;
                }

                Debug.Log("Player hits the ground with high speed;");
            }
        }

        XIntent = Input.GetAxisRaw("Horizontal");
        ZIntent = Input.GetAxisRaw("Vertical");

        if (Grounded && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift) && PlayerStatus != PlayerState.Jumping)
        {
            Normal();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (Grounded && CurrentStamina > MinStamina && PlayerStatus != PlayerState.Jumping && PlayerStatus != PlayerState.Falling)
            {
                Jump();
            }
        }

        if (Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift))
        {
            if (Grounded)
            {
                Crouch();
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl))
        {
            if (Grounded && CurrentStamina > MinStamina)
            {
                Sprint();
            }
            else if (CurrentSpeed >= SprintingSpeed && PlayerStatus != PlayerState.Sprinting)
            {
                PlayerStatus = PlayerState.Sprinting;
            }
        }

        if (!Grounded && PlayerRigidbody.velocity.y < 0f)
        {
            PlayerStatus = PlayerState.Falling;
            CurrentSpreadMultiplier = FallingSpreadMultiplier;
        }

        switch (PlayerStatus)
        {
            case PlayerState.Standing:
                CurrentStamina += Time.deltaTime * StandingStaminaGain;
                break;

            case PlayerState.Walking:
                CurrentStamina += Time.deltaTime * WalkingStaminaGain;
                break;

            case PlayerState.Crouching:
                CurrentStamina += Time.deltaTime * CrouchingStaminaGain;
                break;

            case PlayerState.Sprinting:
                CurrentStamina += Time.deltaTime * SprintingStaminaGain;
                break;

            case PlayerState.Falling:
                CurrentStamina += Time.deltaTime * FallingStaminaGain;
                break;

            case PlayerState.Jumping:
                CurrentStamina += Time.deltaTime * FallingStaminaGain;
                break;
        }
        if (CurrentStamina >= Stamina)
        {
            CurrentStamina = Stamina;
        }
        else if (CurrentStamina <= 0f)
        {
            CurrentStamina = 0f;
            if (Grounded && PlayerStatus != PlayerState.Jumping)
            {
                Normal();
            }
        }
        Player.CurrentStamina = CurrentStamina;

        LastPosition = transform.position;

        if (Player.CurrentNoise < CurrentNoise)
        {
            Player.CurrentNoise = CurrentNoise;
        }

        if (PlayerGun != null)
        {
            PlayerGun.CurrentSpread *= CurrentSpreadMultiplier;
        }
        else if (PlayerMelee != null)
        {
            PlayerMelee.CurrentSpread *= CurrentSpreadMultiplier;
        }
    }

    void FixedUpdate()
    {
        //Player main movement shenanigans//
        Vector3 TargetVelocity = new Vector3(XIntent, 0, ZIntent);
        TargetVelocity *= CurrentSpeed;

        if (PlayerBody)
        {
            Quaternion rotateControlsToLocal = Quaternion.Euler(0, PlayerBody.eulerAngles.y, 0);
            TargetVelocity = rotateControlsToLocal * TargetVelocity;
        }

        Vector3 MovePosition = transform.right * XIntent + transform.forward * ZIntent;

        Vector3 Velocity = PlayerRigidbody.velocity;
        Vector3 VelocityChange = (TargetVelocity - Velocity);
        VelocityChange.x = Mathf.Clamp(VelocityChange.x, -MaxVelocityChange, MaxVelocityChange);
        VelocityChange.z = Mathf.Clamp(VelocityChange.z, -MaxVelocityChange, MaxVelocityChange);
        VelocityChange.y = 0;

        PlayerRigidbody.AddForce(VelocityChange, ForceMode.VelocityChange);

        PlayerRigidbody.velocity = GeneralVelocity;
        //Player main movement shenanigans//
    }

    void Normal()
    {
        CurrentSpeed = WalkingSpeed;

        if (transform.position == LastPosition)
        {
            PlayerStatus = PlayerState.Standing;
            CurrentNoise = StandingNoise;
            CurrentSpreadMultiplier = StandingSpreadMultiplier;
        }
        else
        {
            PlayerStatus = PlayerState.Walking;
            CurrentNoise = WalkingNoise;
            CurrentSpreadMultiplier = WalkingSpreadMultiplier;
        }
    }

    void Jump()
    {
        CurrentStepLimit = JumpingStepLimit;
        CurrentSlopeLimit = JumpingSlopeLimit;

        if (Player.CurrentNoise < JumpingNoise)
        {
            Player.CurrentNoise = JumpingNoise;
        }

        //Jumping MATH//
        GeneralVelocity = PlayerRigidbody.velocity;
        GeneralVelocity.y = Mathf.Sqrt(2 * JumpingHeight * Physics.gravity.y);
        //Jumping MATH//

        CurrentStamina += JumpingStaminaGain;
        PlayerStatus = PlayerState.Jumping;
        Debug.Log(this.name + " jumps;");
    }

    void Crouch()
    {
        CurrentSpeed = CrouchingSpeed;
        CurrentNoise = CrouchingNoise;
        CurrentSpreadMultiplier = CrouchingSpreadMultiplier;

        if (PlayerStatus != PlayerState.Crouching)
        {
            PlayerStatus = PlayerState.Crouching;
            Debug.Log(this.name + " crouches;");
        }
    }

    void Sprint()
    {
        CurrentSpeed = SprintingSpeed;
        CurrentNoise = SprintingNoise;
        CurrentSpreadMultiplier = SprintingSpreadMultiplier;

        if (PlayerStatus != PlayerState.Sprinting)
        {
            PlayerStatus = PlayerState.Sprinting;
            Debug.Log(this.name + " sprints;");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(GroundCheck.position, Radius);
    }
}
