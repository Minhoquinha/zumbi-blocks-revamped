using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
	public bool Dummy; //Dummy players have some different properties//

    [Header("Combat Stats")]
    public float Health;
    [HideInInspector]
    public float CurrentHealth;
    public float Armor;
    public float AttackDamage;

    [Header("Mobility Stats")]
    public float WalkingSpeed;
    public float SprintingSpeed;
    public float CrouchingSpeed;
    public float Acceleration;
    public float AerialAcceleration;
    public float SlowResistance; //Affects how hard it is for the player to be stunned or slowed down//
    public enum PlayerState
    {
        Standing, Walking, Sprinting, Crouching, Falling, Jumping
    }
    [SerializeField]
    public PlayerState PlayerMovementStatus;
    public float JumpHeight;
    public float CrouchHeightPercentage = 100f; //How many percents of the Player's height the Player has while crouching.//
    [HideInInspector]
    public float StandingHeight;
    [HideInInspector]
    public float CrouchingHeight;

    [Header("Stamina")]
    public float Stamina; //How much time the player can run before wearing out//
    [HideInInspector]
    public float CurrentStamina; //How much time the player can keep running before wearing out//
    [Range(0f, 1f)]
    public float MinStamina; //How much stamina is necessary to jump and sprint//
    public float StandingStaminaGain; //How much stamina the player gets while standing still//
    public float WalkingStaminaGain; //How much stamina the player gets while walking//
    public float SprintingStaminaGain; //How much stamina the player gets while sprinting//
    public float CrouchingStaminaGain; //How much stamina the player gets while crouching//
    public float AerialStaminaGain; //How much stamina the player gets while falling or jumping//
    public float JumpingStaminaCost; //How much stamina is lost on a single jump//

    [Header("Stealth")]
    public float Smelliness; //How smelly this player is, the smellier, the easier for enemies to detect//
    [HideInInspector]
    public float CurrentSmelliness; //How smelly this player currently is, the smellier, the easier for enemies to detect//
    [HideInInspector]
    public float CurrentNoise; //Determines how much noise the player is making at the moment//
    public float StandingNoise; //How much noise the player makes while standing still//
    public float WalkingNoise; //How much noise the player makes while walking//
    public float SprintingNoise; //How much noise the player makes while sprinting//
    public float CrouchingNoise; //How much noise the player makes while crouching//
    public float JumpingNoise; //How much noise is made on a single jump//
    public float NoiseReductionFactor; //How much noise is reduced per unit of time//

    [Header("Main References")]
    [Space(50)]
    private GameManager GameManagerScript;
    public PlayerManager PlayerManagerScript;
    public PlayerMovement PlayerMovementScript;
    public PlayerHUD HUD;
    public PlayerInventory PlayerInventoryScript;

    public CharacterController PlayerController;
    public AudioSource FleshSound;
    [HideInInspector]
    public bool Dead;

    void Awake()
    {
        GameManagerScript = FindObjectOfType<GameManager>();
		PlayerManagerScript = GameManagerScript.GetComponent<PlayerManager> ();
        PlayerController = GetComponent<CharacterController>();
        StandingHeight = PlayerController.height;
        CrouchingHeight = StandingHeight / 100f * CrouchHeightPercentage;
    }

    void Start()
    {
        Load();

        CurrentStamina = Stamina;
        MinStamina *= Stamina;
        CurrentHealth = Health;
        CurrentSmelliness = Smelliness;
        Dead = false;
        if (Armor <= 0f)
        {
            Armor = 100f;
        }

        FleshSound = GetComponent<AudioSource>();
        if (!Dummy)
        {
            PlayerMovementScript = GetComponent<PlayerMovement>();
            PlayerInventoryScript = GetComponent<PlayerInventory>();
            HUD = GetComponent<PlayerHUD>();

            if (HUD != null)
            {
                HUD.MaxHealth = CurrentHealth;
                HUD.MaxStamina = Stamina;

                if (HUD.HealthBar != null)
                {
                    HUD.HealthBar.value = Health;
                }
                if (HUD.StaminaBar != null)
                {
                    HUD.StaminaBar.value = CurrentStamina;
                }
                if (HUD.NoiseBar != null)
                {
                    HUD.NoiseBar.value = CurrentNoise;
                }
                if (HUD.DeathScreen != null)
                {
                    HUD.DeathScreen.enabled = false;
                }
            }
        }
    }

    void Load ()
    {
        PlayerManagerScript.PlayerList.Add(this.transform);

        EnemyMovement [] CurrentZombiesInScene = FindObjectsOfType<EnemyMovement>();

        foreach (EnemyMovement ZombieInScene in CurrentZombiesInScene)
        {
            ZombieInScene.FindPlayers();
        }
    }

    void Update()
    {
        if (GameManagerScript.GameStatus != GameManager.GameState.InGame)
        {
            return;
        }

        if (CurrentNoise > 0f)
        {
            CurrentNoise -= Time.deltaTime * NoiseReductionFactor;
        }

        if (Dead)
        {
            return;
        }

		if (!Dummy)
		{
            switch (PlayerMovementStatus)
            {
                case PlayerState.Standing:
                    if ((CurrentStamina < Stamina && StandingStaminaGain > 0f) || (CurrentStamina > 0f && StandingStaminaGain < 0f))
                    {
                        CurrentStamina += Time.deltaTime * StandingStaminaGain;
                    }
                    CurrentNoise = Mathf.Max(StandingNoise, CurrentNoise);
                    break;

                case PlayerState.Walking:
                    if ((CurrentStamina < Stamina && WalkingStaminaGain > 0f) || (CurrentStamina > 0f && WalkingStaminaGain < 0f))
                    {
                        CurrentStamina += Time.deltaTime * WalkingStaminaGain;
                    }
                    CurrentNoise = Mathf.Max(WalkingNoise, CurrentNoise);
                    break;

                case PlayerState.Crouching:
                    if ((CurrentStamina < Stamina && CrouchingStaminaGain > 0f) || (CurrentStamina > 0f && CrouchingStaminaGain < 0f))
                    {
                        CurrentStamina += Time.deltaTime * CrouchingStaminaGain;
                    }
                    CurrentNoise = Mathf.Max(CrouchingNoise, CurrentNoise);
                    break;

                case PlayerState.Sprinting:
                    if ((CurrentStamina < Stamina && SprintingStaminaGain > 0f) || (CurrentStamina > 0f && SprintingStaminaGain < 0f))
                    {
                        CurrentStamina += Time.deltaTime * SprintingStaminaGain;
                    }
                    CurrentNoise = Mathf.Max(SprintingNoise, CurrentNoise);
                    break;

                case PlayerState.Falling:
                    if ((CurrentStamina < Stamina && AerialStaminaGain > 0f) || (CurrentStamina > 0f && AerialStaminaGain < 0f))
                    {
                        CurrentStamina += Time.deltaTime * AerialStaminaGain;
                    }
                    break;

                case PlayerState.Jumping:
                    if ((CurrentStamina < Stamina && AerialStaminaGain > 0f) || (CurrentStamina > 0f && AerialStaminaGain < 0f))
                    {
                        CurrentStamina += Time.deltaTime * AerialStaminaGain;
                    }
                    break;
            }

            if (CurrentStamina < 0f)
            {
                CurrentStamina = 0f;
            }

            if (HUD != null)
            {
                HUD.StaminaChange(CurrentStamina);
            }

            if (CurrentNoise < 0f)
            {
                CurrentNoise = 0f;
            }

            if (HUD != null)
            {
                HUD.NoiseChange(CurrentNoise);
            }
		}
    }

    public void Hurt(float Damage)
    {
        if (Dead)
        {
            return;
        }

        CurrentHealth -= Damage / (Armor / 100f);
        FleshSound.Play();

        if (HUD != null)
        {
            if (!Dummy)
            {
                HUD.HealthChange(CurrentHealth);
            }
        }

        if (CurrentHealth <= 0f)
        {
			CurrentHealth = 0f;
            Death();
        }
    }

    public void Heal(float Amount)
    {
        if (Dead)
        {
            return;
        }

        CurrentHealth += Amount;

        if (HUD != null)
        {
            if (!Dummy)
            {
                HUD.HealthChange(CurrentHealth);
            }
        }

        if (CurrentHealth > Health)
        {
            CurrentHealth = Health;
        }
    }

    public void GiveStamina(float Amount)
    {
        if (Dead)
        {
            return;
        }

        CurrentStamina += Amount;

        if (HUD != null)
        {
            if (!Dummy)
            {
                HUD.StaminaChange(CurrentStamina);
            }
        }

        if (CurrentStamina > Stamina)
        {
            CurrentStamina = Stamina;
        }
    }
    public void Death()
    {
        if (!Dead)
        {
            Dead = true;
            Debug.Log(this.name + " died;");

            if (HUD != null)
            {
                if (!Dummy)
                {
                    HUD.DeathScreen.enabled = true;
                }
            }

            if (PlayerInventoryScript != null)
            {
                PlayerInventoryScript.DeathDrop();
            }

            PlayerManagerScript.PlayerDeath(this.transform);
        }
    }
}