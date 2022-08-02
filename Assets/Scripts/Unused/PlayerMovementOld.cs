using UnityEngine;

public class PlayerMovementOld : MonoBehaviour
{
    [Header("Main References")]
    public CharacterController Control;
    public Transform PlayerBody;
    public PlayerStats Player;

    [Header("Main Properties")]
    private float CurrentNoise; //How much noise the player is making at the moment, changes if player is crouching, sprinting, etc//
	public float NormalNoise; //How much noise the player makes when standing still//
	public float FallNoise; //How much noise the player makes when hitting the ground with enough speed//
    public float HarmfulSpeed = 10f; //Minimum amount of speed necessary to hurt the player//
    public float SpeedDamage; //How much damage the player gets for each + 10 m/s of harmful speed//

    [Header("Dimensions")]
    public float Height = 3f;
    public float NormalSlopeLimit = 45.0f; //How low the angle of a slope can be before the player cant climb it anymore//
    private float CurrentSlopeLimit = 45.0f; //The player slope limit variable being used at the moment, changes if player is jumping, sprinting, etc//
    public float NormalStepLimit = 0.2f; //How high a step can be before the player cant climb it anymore//
    private float CurrentStepLimit = 0.2f; //The player step limit variable being used at the moment, changes if player is jumping, sprinting, etc//
	private Vector3 LastPosition; //This player last position//

    [Header("Jump Mechanics")]
    public float JumpHeight = 1f;
	private bool RecentJump = false; //Checks if the player recently jumped, to prevent the Jump function from being called many times when pressing//
	public float Gravity = -9.81f;
    public float JumpStepLimit = 0.4f;
    public float JumpSlopeLimit = 100.0f;
    public float JumpNoise; //How much noise the player makes when jumping//
	public float JumpStaminaLoss; //How much stamina the player loses when jumping//

    [Header("Velocity Mechanics")]
    public float NormalSpeed = 12f; //The player speed for when the player is just walking//
    public float WalkNoise; //How much noise the player makes while walking//
	private float CurrentSpeed = 12f; //The player speed variable being used at the moment, changes if player is crouching, sprinting, etc//
	private Vector3 GeneralVelocity;

	[Header("Stamina Mechanics")]
	[HideInInspector]
	public float Stamina;
	private float CurrentStamina;
	public float MinStamina; //The minimum stamina necessary to start running or jumping//
	public float StaminaReductionFactor; //How much stamina is lost while sprinting//
	public float StaminaAdditionFactor; //How much stamina is gained while not sprinting//

    [Header("Sprinting Mechanics")]
    public float SprintSpeed; //The player speed for when the player is sprinting//
    public float SprintNoise; //How much noise the player makes while sprinting//
    private bool Sprinting = false; //Checks if the player is currently sprinting, so Debug.Log doesnt spam that the player is sprinting, and player can walk afterwards//

    [Header("Crouch Mechanics")]
	public float CrouchSpeed = 6f;
    public float CrouchNoise; //How much noise the player makes while crouching//
    public float CrouchHeight = 1.8f; //Height of the player while crouching//
	private bool Crouched = false; //Checks if the player is currently crouching, so Debug.Log doesnt spam that the player is crouching, and player can stand afterwards//

    [Header("Ground Checking")] //These variables are used for checking if player is grounded//
    public Transform GroundCheck;
	public Vector3 GroundHitBox;

	void Start ()
	{
        Player = GetComponent<PlayerStats>();

		RecentJump = false;
		Crouched = false;
		Debug.Log (this.name + " loaded;");
	}

	void Update ()
	{
        if (Player.Dead)
        {
            return;
        }
			
        bool Grounded = Physics.CheckBox (GroundCheck.position, GroundHitBox, transform.rotation); //Checks if player is currently on the ground or airborne//

		if (Grounded && GeneralVelocity.y < 0f)
		{
			if (-GeneralVelocity.y >= HarmfulSpeed)
			{
				float FallDamage = SpeedDamage * (-GeneralVelocity.y / 10f);
				Player.Hurt (FallDamage);

				if (Player.CurrentNoise < FallNoise)
				{
					Player.CurrentNoise = FallNoise;
				}

				Debug.Log ("Player hits the ground with high speed;");
			}
				
			CurrentStepLimit = NormalStepLimit;
			CurrentSlopeLimit = NormalSlopeLimit;
			GeneralVelocity.y = -2f; //Gravity is turned down slowly when player hits ground to prevent glitching through it//
			RecentJump = false; //Resets jump when player hits the ground//
		}

        //Player main movement shenanigans//
            float X = Input.GetAxisRaw ("Horizontal");
		    float Z = Input.GetAxisRaw ("Vertical");
		    Vector3 PlayerMove = transform.right * X + transform.forward * Z;
		    Control.Move (PlayerMove * CurrentSpeed * Time.deltaTime);
		    GeneralVelocity.y += Gravity * Time.deltaTime;
		    Control.Move (GeneralVelocity * Time.deltaTime);
			
            Control.slopeLimit = CurrentSlopeLimit;
            Control.stepOffset = CurrentStepLimit;
        //Player main movement shenanigans//

		if (CurrentStamina > MinStamina && !RecentJump && Grounded && Input.GetKey(KeyCode.Space))
		{
			Jump ();
		}

		if (Grounded && Input.GetKey(KeyCode.LeftControl))
		{
			Crouch ();
		}
		else if (Crouched)
		{
			Walk ();
		}

        if (Grounded && Input.GetKey(KeyCode.LeftShift))
        {
			if (CurrentStamina > MinStamina)
			{
				Sprint ();
				CurrentStamina -= Time.deltaTime * StaminaReductionFactor;
			}
			else if (Sprinting)
			{
				CurrentStamina -= Time.deltaTime * StaminaReductionFactor;
			}

			if (CurrentStamina <= 0f)
			{
				Walk ();
			}
        }
		else if (Sprinting)
        {
            Walk ();
        }

        if (CurrentStamina >= Stamina)
        {
            CurrentStamina = Stamina;
        }
		else if (!Sprinting)
        {
			CurrentStamina += Time.deltaTime * StaminaAdditionFactor;
        }
		Player.CurrentStamina = CurrentStamina;

		if (transform.position == LastPosition)
		{
			CurrentNoise = NormalNoise;
		}
		else if (!Sprinting && !Crouched)
		{
			CurrentNoise = WalkNoise;
		}

		if (Player.CurrentNoise < CurrentNoise)
		{
			Player.CurrentNoise = CurrentNoise;
		}

		LastPosition = transform.position;
    }

	void Jump ()
	{
		if (!RecentJump)
		{
			CurrentStepLimit = JumpStepLimit;
			CurrentSlopeLimit = JumpSlopeLimit;

			if (Player.CurrentNoise < JumpNoise)
			{
				Player.CurrentNoise = JumpNoise;
			}
				
			GeneralVelocity.y = Mathf.Sqrt (JumpHeight * -2f * Gravity); //Jumping MATH//
			CurrentStamina -= JumpStaminaLoss;
			RecentJump = true;
			Debug.Log (this.name + " jumps;");
		}
	}

	void Crouch ()
	{
		Control.height = CrouchHeight;
		CurrentSpeed = CrouchSpeed;
        CurrentNoise = CrouchNoise;

		if (!Crouched)
		{
			Crouched = true;
			Debug.Log (this.name + " crouches;");
		}
	}

	void Walk ()
	{
		Control.height = Height;
		CurrentSpeed = NormalSpeed;
        CurrentNoise = WalkNoise;
        Crouched = false;
        Sprinting = false;
		Debug.Log (this.name + " walks;");
	}

    void Sprint ()
    {
        CurrentSpeed = SprintSpeed;
        CurrentNoise = SprintNoise;

        if (!Sprinting)
        {
            Sprinting = true;
            Debug.Log (this.name + " sprints;");
        }
    }
}
