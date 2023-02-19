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

    [Header("Health State")]
    [HideInInspector]
    public bool Dead;
    public bool Bleeding = false;
    public float AverageBleedingDuration = 60f; //The average total time the player spends bleeding after it started in seconds//
    private float CurrentBleedingDuration = 60f; //The current time the player will spend bleeding in seconds//
    public float BleedingDurationVariation = 10f; //The variation of time the player spends bleeding in seconds//
    public float BleedingDamage = 2f; //The damage the player receives by bleeding each BleedingRate seconds//
    public float BleedingRate = 0.7f; //The rate at which the player gets damaged by bleeding per second//
    private float NextBleed = 0f; //Time it will take for the next bleeding damage in seconds//

    [Header("Main References")]
    [Space(50)]
    private GameManager GameManagerScript;
    public PlayerManager PlayerManagerScript;
    public PlayerMovement PlayerMovementScript;
    public PlayerHUD HUD;
    public PlayerInventory PlayerInventoryScript;
    public CharacterController PlayerController;
    public AudioSource FleshSound;
    public ParticleSystem BloodEruption;


    void Awake()
    {
        GameManagerScript = FindObjectOfType<GameManager>();
		PlayerManagerScript = GameManagerScript.GetComponent<PlayerManager> ();
        PlayerController = GetComponent<CharacterController>();
        StandingHeight = PlayerController.height;
        CrouchingHeight = StandingHeight / 100f * CrouchHeightPercentage;

        BloodEruption = GetComponentInChildren<ParticleSystem>();
        if (BloodEruption != null)
        {
            ParticleSystem.MainModule BloodEruptionMainModule = BloodEruption.main;
            BloodEruptionMainModule.loop = true;
        }
    }

    void Start()
    {
        Load();

        CurrentStamina = Stamina;
        MinStamina *= Stamina;
        CurrentHealth = Health;
        CurrentSmelliness = Smelliness;
        Dead = false;
        Bleeding = false;
        CurrentBleedingDuration = 0f;
        NextBleed = 0f;
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

            if (CurrentBleedingDuration <= 0f)
            {
                StopBleeding();
            }
            else
            {
                CurrentBleedingDuration -= Time.deltaTime;
            }

            if (Bleeding)
            {
                if  (NextBleed <= 0f)
                {
                    NextBleed = 1f / BleedingRate;
                    Bleed();
                }
                else
                {
                    NextBleed -= Time.deltaTime;
                }
            }
		}
    }

    public void Hurt(float Damage, float BleedChance)
    {
        if (Dead)
        {
            return;
        }

        CurrentHealth -= Damage / (Armor / 100f);
        FleshSound.Play();

        float Bleed = Random.value;

        if (Bleed <= BleedChance)
        {
            float BleedingDuration = AverageBleedingDuration + Random.Range(-BleedingDurationVariation, BleedingDurationVariation);

            CurrentBleedingDuration = BleedingDuration;

            if (!Bleeding)
            {
                Bleeding = true;
                BloodEruption.Play();
            }
        }

        if (HUD != null)
        {
            if (!Dummy)
            {
                HUD.HurtEffect(Damage);
                HUD.HealthChange(CurrentHealth);
            }
        }

        if (CurrentHealth <= 0f)
        {
			CurrentHealth = 0f;
            Death();
        }
    }

    public void Bleed()
    {
        if (Dead)
        {
            return;
        }

        CurrentHealth -= BleedingDamage;

        if (HUD != null)
        {
            if (!Dummy)
            {
                HUD.HurtEffect(BleedingDamage * 5f);
                HUD.HealthChange(CurrentHealth);
            }
        }

        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            Death();
        }
    }

    public void StopBleeding ()
    {
        CurrentBleedingDuration = 0f;
        Bleeding = false;
        BloodEruption.Stop();
    }

    public void Heal(float Amount, bool Stanch)
    {
        if (Dead)
        {
            return;
        }

        CurrentHealth += Amount;

        if (CurrentHealth > Health)
        {
            CurrentHealth = Health;
        }

        if (Stanch)
        {
            StopBleeding();
        }

        if (HUD != null)
        {
            if (!Dummy)
            {
                HUD.HealthChange(CurrentHealth);
            }
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