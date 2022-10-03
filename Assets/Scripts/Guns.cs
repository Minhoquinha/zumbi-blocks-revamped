using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Guns : MonoBehaviour
{
    [Header("Main")]
    public string GunName = "TestGun";

    [Header("Main Stats")]
    public bool FullAuto; //Determines if this weapon is automatic//
    public bool ProjectileBased; //Determines if this weapon uses projectiles instead of hit scans//
    public int BulletCount; //Determines the amount of bulles this gun fires with each shot, normally used for shotguns//
    private int NextBullet; //The number of the next bullet the gun is going to fire out of a burst, normally used for shotguns//
    public float Damage;
    private float CurrentDamage; //The amount of damage that the bullet is going to inflict after calculating penetration and bullet dropoff (DOES NOT account for damage multipliers and armor)//
    public float Penetration = 100f; //How easily can the bullet pierce through multiple targets and armor//
    public float DamageDropoffResistance = 100f; //How much damage is preserved with large distances between the gun and the target// 
    public float MaxRange = 100f; //Anything beyond this is will not be affected by Raycast//
    public float FireRate; //Number of bullets per second//
	[HideInInspector]
	public float CurrentFireDelay; //Time it takes to fire again//

    [Header("Damage multipliers")]
    public float HeadshotDamageMultiplier = 2f;
    public float BodyshotDamageMultiplier = 1f;
    public float LegshotDamageMultiplier = 0.75f;
    public float ArmshotDamageMultiplier = 0.75f;
    private float CurrentDamageMultiplier = 1f;

    [Header("Spread")]
    public float AimingSpreadDivisor = 2f; //How much the spread is reduced when the player aims down the sight of this gun (divisor)//
    private bool AimingDownSight; //Checks if player is currently aiming//
    public float AimingFieldOfView; //Player's field of view while aiming//
    public float StandingSpread; //How much the bullets diverge from the intended path when the player is standing up//
    public float WalkingSpread; //How much the bullets diverge from the intended path when the player is walking//
    public float CrouchingSpread; //How much the bullets diverge from the intended path when the player is crouching//
    public float AerialSpread; //How much the bullets diverge from the intended path when the player is jumping or falling//
    [HideInInspector]
	public float CurrentSpread; //How much the bullets will diverge from the intended path//

    [Header("Recoil")]
    public float RecoilResetTime = 0.5f; //How much time it takes for the gun to go back to it's original position after being fired//
    [HideInInspector]
    public float LastShotTime = 0f; //Game's time when the last shot was fired//
    public Vector3[] RecoilPattern = new Vector3[50]; //How this gun reacts to being fired in each shot//
    [HideInInspector]
    public int CurrentRecoilIndex = 0;
    [HideInInspector]
    public Vector3 RecoilDivergence = Vector3.zero;

    [Header("Ammo Stats")]
    public float ReloadDelay; //Time it takes to reload this gun//
	public int TotalAmmo; //How many bullets the player has in the pocket for this gun//
    public int AmmoCapacity; //How many bullets this gun can fire before having to reload//
    public int AmmoBoxCapacity = 10; //How many bullets the Player gets when opening an ammo box with this gun//
    [HideInInspector]
    public int Ammo; //How many bullets this gun can currently fire before having to reload//
    public bool SingleBulletReload; //If true this gun has to be reloaded bullet by bullet, normally used for shotguns//
    private bool Reloading = false; //Checks if the player is currently reloading the gun//

    [Header("Passive Stats")]
    public bool HandGun; //Hand guns can be carried in the second slot of the inventory instead of the first//
    public float Loudness; //How loud this weapon is for enemies//
    public bool MultiMuzzle; //If this gun has multiple muzzles//
    public float EquipDelay = 0.4f; //Time it takes to equip this gun//
    private RaycastHit AimPoint;
    private bool AimingAtObject; //Checks if the player is aiming at something in range, otherwise some actions are unnecessary//
    public float SpeedDivisor = 1.1f; //How much movement speed the player loses while holding this weapon (divisor)//
    public float AimingSpeedDivisor = 2f; //How much movement speed the player loses while aiming with this weapon (divisor)//
    private float CurrentSpeedDivisor; //How much movement speed the player is currently losing while holding this weapon (divisor)//

    [Header("Projectile Based")]
    public GameObject ProjectilePrefab;
    public float ProjectileLifetime; //How much time the projectile stays alive before vanishing from existence//
    public Vector3 ProjectileForce;

    [Header("Animations")]
    public bool HasBoltLockBack;
    private Animator AnimatorController;
    public string IdleAnimationName = "Idle";
    public string FireAnimationName = "Fire";
    public string FireADSAnimationName = "Fire_ADS";
    public string ReloadAnimationName = "ReloadEmpty";
    public string EquipAnimationName = "Equip";
    public string InspectAnimationName = "Inspect";
    private bool Equipped;
    public GameObject ModelUnequipped; //The gun when loose on the ground//
    public GameObject ModelEquipped; //The gun when equipped by player//

    [Header("Casing Ejection")]
    public bool CasingEjection; //Bool that verify if the weapon must eject casings when firing (Useful to avoid spawn casings with Revolvers or Double-barreled Shotguns when firing)//
    public GameObject CasingPrefab; //Prefab of the casing to spawn//
    public Transform CasingEjectionTransform; //Empty GameObject from which casings spawn//
    public float EjectSpeed = 12f; //Casing ejection speed (I recommend setting it to 12 or above this value)//
    public float DestroyTime = 3f; //Time it takes for the spawned casing to disappear in seconds//
    public float CasingDelay; //Delay in seconds of casings spawn (Useful for shotguns or bolt rifles) - Setting 0 means that the Casing spawns automatically when shoots//

    [Header("Debugging")]
    public bool PrintDamageFactors = false;
    public bool ShowImpacts = false;
    public bool InfiniteAmmo = false;
    public bool NoSpread = false;
    public bool NoRecoil = false;

    [Header("Main References")]
    [Space(50)]
    public GameObject [] MuzzleArray = new GameObject [5]; //Array for muzzles if the gun has multiple muzzles//
    private ParticleSystem [] MuzzleFlashArray = new ParticleSystem [5];
    private AudioSource [] MuzzleNoiseArray = new AudioSource [5];
    private Transform [] MuzzleTransformArray = new Transform [5];
    private int CurrentMuzzle = 0;
    public GameObject Muzzle;
    private ParticleSystem MuzzleFlash;
    private AudioSource MuzzleNoise;
    private Transform MuzzleTransform;

    public AudioSource ReloadSound;
    public AudioSource NoAmmoSound;
    private Item ItemScript;
    private PlayerStats Player;
	private PlayerHUD HUD;
    private Collider PlayerBody;
    private Camera FPCamera;
    private float DefaultFieldOfView;
    private MouseController MouseControllerScript;

    private GameManager GameManagerScript;
    private ControlsManager ControlsManagerScript;
    private ScriptableObjectManager ScriptableObjectManagerScript;
    private PhysicMaterialCollection PhysicMaterialCollectionScript;
    private ImpactEffectCollection ImpactEffectCollectionScript;

    void Awake()
    {
        if (MultiMuzzle)
        {
            MuzzleFlashArray = new ParticleSystem [MuzzleArray.Length];
            MuzzleNoiseArray = new AudioSource [MuzzleArray.Length];
            MuzzleTransformArray = new Transform [MuzzleArray.Length];
        }

        GameManagerScript = FindObjectOfType<GameManager>();
        ControlsManagerScript = GameManagerScript.GetComponent<ControlsManager>();
        ScriptableObjectManagerScript = GameManagerScript.GetComponent<ScriptableObjectManager>();
        PhysicMaterialCollectionScript = ScriptableObjectManagerScript.PhysicMaterialCollectionScript;
        ImpactEffectCollectionScript = ScriptableObjectManagerScript.ImpactEffectCollectionScript;

        if (ModelEquipped != null)
        {
            ModelEquipped.SetActive(true);
        }
        AnimatorController = GetComponentInChildren<Animator>();

        ItemScript = GetComponent<Item>();
        ItemScript.CheckItemType();

        Ammo = AmmoCapacity;

        if (Penetration <= 0f)
        {
            Penetration = 100f;
        }

        if (DamageDropoffResistance <= 0f)
        {
            DamageDropoffResistance = 100f;
        }

        Unequip();
        enabled = false;
    }

    void Start()
    {
        Load();

        if (GunName == null)
        {
            Debug.LogWarning(this.name + " has no name in script;");
        }
    }

    void Update()
    {
        if (GameManagerScript.GameStatus != GameManager.GameState.InGame)
        {
            return;
        }

        if (Player.Dead)
        {
            Unequip();
            return;
        }

        UpdateCrosshair();

        if (CurrentFireDelay > 0f)
		{
			CurrentFireDelay -= Time.deltaTime;
        }

        bool TryFire = false;
        bool TryReload = false;
        bool TryAim = false;
        AimingDownSight = false;
        bool TryInspect = false;

        if (Player.PlayerMovementStatus != PlayerStats.PlayerState.Sprinting)
        {
            if (FullAuto)
            {
                TryFire = Input.GetKey(ControlsManagerScript.ControlDictionary ["BasicFire"]);
            }
            else
            {
                TryFire = Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["BasicFire"]);
            }

            if (SingleBulletReload)
            {
                TryReload = Input.GetKey(ControlsManagerScript.ControlDictionary ["Reload"]);
            }
            else
            {
                TryReload = Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["Reload"]);
            }

            TryAim = Input.GetKey(ControlsManagerScript.ControlDictionary ["Aim"]);

            TryInspect = Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["Inspect"]);

            if (TryAim && !Reloading)
            {
                AimingDownSight = true;
            }
            else
            {
                AimingDownSight = false;
            }

            if (CurrentFireDelay <= 0f)
            {
                if (!NoSpread)
                {
                    float SpreadDivisor = 1000f;

                    if (AimingDownSight)
                    {
                        SpreadDivisor *= AimingSpreadDivisor;
                    }

                    switch (Player.PlayerMovementStatus)
                    {
                        case PlayerStats.PlayerState.Standing:
                            CurrentSpread = StandingSpread / SpreadDivisor;
                            break;

                        case PlayerStats.PlayerState.Walking:
                            CurrentSpread = WalkingSpread / SpreadDivisor;
                            break;

                        case PlayerStats.PlayerState.Crouching:
                            CurrentSpread = CrouchingSpread / SpreadDivisor;
                            break;

                        case PlayerStats.PlayerState.Sprinting:
                            CurrentSpread = AerialSpread / SpreadDivisor;
                            break;

                        case PlayerStats.PlayerState.Falling:
                            CurrentSpread = AerialSpread / SpreadDivisor;
                            break;

                        case PlayerStats.PlayerState.Jumping:
                            CurrentSpread = AerialSpread / SpreadDivisor;
                            break;
                    }
                }
                else
                {
                    CurrentSpread = 0f;
                }

                if (TryFire)
                {
                    Fire();
                }

                if (TryReload && Ammo < AmmoCapacity && !Reloading)
                {
                    StartReload();
                }

                if (TryInspect && !Reloading && !AimingDownSight)
                {
                    Inspect();
                }

                if (HasBoltLockBack)
                {
                    if (Ammo == 0 && Reloading == false)
                    {
                        AnimatorController.SetLayerWeight(1, 1);
                    }
                    else if (Ammo > 0 && Reloading == true)
                    {
                        AnimatorController.SetLayerWeight(1, 0);
                    }
                }
            }
        }

        if (AimingDownSight)
        {
            HUD.CenterCrosshair.CrossFadeAlpha(0f, 0f, true);
            HUD.DynamicCrosshair.CrossFadeAlpha(0f, 0f, true);

            if (AnimatorController != null)
            {
                AnimatorController.SetBool("ADS", true);
            }

            MouseControllerScript.Zoom(AimingFieldOfView);
        }
        else
        {
            HUD.CenterCrosshair.CrossFadeAlpha(1f, 0f, true);
            HUD.DynamicCrosshair.CrossFadeAlpha(1f, 0f, true);

            if (AnimatorController != null)
            {
                AnimatorController.SetBool("ADS", false);
            }

            MouseControllerScript.Zoom(DefaultFieldOfView);
        }

        if (!TryFire && Time.time - LastShotTime >= RecoilResetTime)
        {
            Vector3 TargetDirection = Vector3.Lerp(RecoilDivergence, FPCamera.transform.forward, Time.deltaTime * 2f);
            SetGunRotation(TargetDirection);
        }

        switch (Player.PlayerMovementStatus)
        {
            case PlayerStats.PlayerState.Standing:
                if (AnimatorController != null)
                {
                    AnimatorController.SetBool("Walk", false);
                    AnimatorController.SetBool("Sprint", false);
                }
                break;

            case PlayerStats.PlayerState.Walking:
                if (AnimatorController != null)
                {
                    AnimatorController.SetBool("Walk", true);
                    AnimatorController.SetBool("Sprint", false);
                }
                break;

            case PlayerStats.PlayerState.Crouching:
                if (AnimatorController != null)
                {
                    AnimatorController.SetBool("Walk", false);
                    AnimatorController.SetBool("Sprint", false);
                }
                break;

            case PlayerStats.PlayerState.Sprinting:
                if (AnimatorController != null)
                {
                    AnimatorController.SetBool("Walk", false);
                    AnimatorController.SetBool("Sprint", true);
                }
                break;

            case PlayerStats.PlayerState.Falling:
                if (AnimatorController != null)
                {
                    AnimatorController.SetBool("Walk", false);
                    AnimatorController.SetBool("Sprint", false);
                }
                break;

            case PlayerStats.PlayerState.Jumping:
                if (AnimatorController != null)
                {
                    AnimatorController.SetBool("Walk", false);
                    AnimatorController.SetBool("Sprint", false);
                }
                break;

            default:
                if (AnimatorController != null)
                {
                    AnimatorController.SetBool("Walk", false);
                    AnimatorController.SetBool("Sprint", false);
                }
                break;
        }

    }

    public void Load ()
    {
        //Loads everything necessary for this component. It's only necessary to load if it's picked up//

        FPCamera = GetComponentInParent<Camera>();
        Player = FPCamera.GetComponentInParent<PlayerStats>();
        PlayerBody = Player.GetComponentInChildren<Collider>();
        HUD = FPCamera.GetComponentInParent<PlayerHUD>();
        MouseControllerScript = FPCamera.GetComponent<MouseController>();

        if (MultiMuzzle)
        {
            for (int i = 0; i < MuzzleArray.Length; i++)
            {
                MuzzleFlashArray[i] = MuzzleArray[i].GetComponent<ParticleSystem>();
                MuzzleNoiseArray[i] = MuzzleArray[i].GetComponent<AudioSource>();
                MuzzleTransformArray[i] = MuzzleArray[i].transform;
            }
        }
        else
        {
            MuzzleFlash = Muzzle.GetComponent<ParticleSystem>();
            MuzzleNoise = Muzzle.GetComponent<AudioSource>();
            MuzzleTransform = Muzzle.transform;
        }

        DefaultFieldOfView = FPCamera.fieldOfView;
        if (AimingFieldOfView == 0f)
        {
            AimingFieldOfView = DefaultFieldOfView;
        }

        RecoilDivergence = FPCamera.transform.forward;

        CurrentFireDelay = 1f / FireRate;
        CurrentSpread = StandingSpread;
        CurrentDamage = Damage;
        if (BulletCount < 1)
        {
            BulletCount = 1;
        }

        Equip();

        Debug.Log(this.name + " loaded;");
    }

    public void Equip()
    {
        Equipped = true;

        if (ModelUnequipped != null)
        {
            ModelUnequipped.SetActive(false);
        }
        if (ModelEquipped != null)
        {
            ModelEquipped.SetActive(true);
        }

        if (HUD != null)
        {
            HUD.CenterCrosshair.enabled = true;
            HUD.DynamicCrosshair.enabled = true;
            HUD.Hitmarker.enabled = true;
            HUD.Hitmarker.CrossFadeAlpha(0f, 0f, true);
            HUD.Hitmarker.rectTransform.position = HUD.DynamicCrosshair.rectTransform.position;
            HUD.CenterCrosshair.CrossFadeAlpha(255f, 0f, true);
            HUD.WeaponTag.text = GunName;
            HUD.AmmoCounter.text = Ammo + "/" + TotalAmmo;
        }

        CurrentFireDelay = EquipDelay;

        if (AnimatorController != null)
        {
            AnimatorController.CrossFadeInFixedTime(EquipAnimationName, 0f);
        }
    }

    public void Unequip()
    {
        Equipped = false;
        Reloading = false;
        StopCoroutine(Reload());

        if (AnimatorController != null)
        {
            AnimatorController.CrossFadeInFixedTime(EquipAnimationName, 0f);
        }

        if (ModelUnequipped != null)
        {
            ModelUnequipped.SetActive(true);
        }
        if (ModelEquipped != null)
        {
            ModelEquipped.SetActive(false);
        }
    }

    void Fire()
    {
        if (Ammo > 0)
        {
            if (MultiMuzzle)
            {
                MuzzleFlashArray [CurrentMuzzle].Play();
                MuzzleNoiseArray [CurrentMuzzle].Play();
            }
            else
            {
                MuzzleFlash.Play();
                MuzzleNoise.Play();
            }

            if (Player.CurrentNoise < Loudness)
            {
                Player.CurrentNoise = Loudness;
            }

            CurrentFireDelay = 1f / FireRate;

            if (!InfiniteAmmo)
            {
                Ammo--;
            }

            if (HUD != null)
            {
                HUD.AmmoCounter.text = Ammo + "/" + TotalAmmo;
            }

            if (AnimatorController != null)
            {
                if (AnimatorController.GetBool("ADS"))
                {
                    AnimatorController.CrossFadeInFixedTime(FireADSAnimationName, 0.125f);
                }
                else
                {
                    AnimatorController.CrossFadeInFixedTime(FireAnimationName, 0.125f);
                }
            }

            if (CasingEjection)
            {
                StartCoroutine(CasingSpawn());
            }

            for (NextBullet = 1; NextBullet <= BulletCount; NextBullet++)
            {
                Vector3 BulletPath = transform.forward;

                if (CurrentSpread > 0f)
                {
                    BulletPath += Random.insideUnitSphere * CurrentSpread;
                }

                if (!ProjectileBased)
                {

                    if (AimingAtObject)
                    {
                        RaycastHit [] HitObjects = Physics.RaycastAll(MuzzleTransform.position, BulletPath, MaxRange);

                        //Use insertion sort to sort the array of hit objects by ascending order of distance to the gun//
                        for (int i = 1; i < HitObjects.Length; i++)
                        {
                            RaycastHit Hit = HitObjects [i];

                            for (int j = i - 1; j >= 0; j--)
                            {
                                if (HitObjects [j].collider == PlayerBody)
                                {
                                    HitObjects [i] = HitObjects [j];
                                    HitObjects [j] = Hit;
                                }
                                else if (Hit.distance < HitObjects [j].distance)
                                {
                                    HitObjects [i] = HitObjects [j];
                                    HitObjects [j] = Hit;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        //Use insertion sort to sort the array of hit objects by ascending order of distance to the gun//

                        for (int i = 0; i < HitObjects.Length; i++)
                        {
                            bool HitRepetition = false; //True if this hit object was already hit by this weapon with another collider and possess an EnemyStats or PlayerStats script//
                            EnemyStats Enemy = HitObjects [i].transform.GetComponentInParent<EnemyStats>();
                            PlayerStats OtherPlayer = HitObjects [i].transform.GetComponentInParent<PlayerStats>();

                            if (OtherPlayer != Player)
                            {
                                for (int j = 0; j < HitObjects.Length; j++)
                                {
                                    if (i > j)
                                    {
                                        EnemyStats SecondEnemy = HitObjects [j].transform.GetComponentInParent<EnemyStats>();
                                        PlayerStats SecondOtherPlayer = HitObjects [j].transform.GetComponentInParent<PlayerStats>();

                                        if (SecondEnemy != null && SecondEnemy == Enemy)
                                        {
                                            HitRepetition = true;
                                        }
                                        else if (SecondOtherPlayer != null && SecondOtherPlayer == OtherPlayer)
                                        {
                                            HitRepetition = true;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (i * 100f / Penetration < 1f && !HitRepetition)
                                {
                                    BulletHit(HitObjects [i], i);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (ProjectilePrefab != null)
                    {
                        Debug.Log(gameObject.name + " fires " + ProjectilePrefab.name + ";");

                        Rigidbody Projectile;

                        Quaternion RotateToLocal;
                        if (MultiMuzzle)
                        {
                            RotateToLocal = MuzzleTransformArray [CurrentMuzzle].rotation;
                        }
                        else
                        {
                            RotateToLocal = MuzzleTransform.rotation;
                        }
                        Quaternion ProjectileRotation = Quaternion.Euler(BulletPath) * RotateToLocal;

                        if (MultiMuzzle)
                        {
                            Projectile = Instantiate(ProjectilePrefab, MuzzleTransformArray [CurrentMuzzle].position, MuzzleTransformArray [CurrentMuzzle].rotation).GetComponent<Rigidbody>();
                        }
                        else
                        {
                            Projectile = Instantiate(ProjectilePrefab, MuzzleTransform.position, MuzzleTransform.rotation).GetComponent<Rigidbody>();
                        }

                        if (Projectile != null)
                        {
                            Vector3 CurrentProjectileForce = RotateToLocal * ProjectileForce;
                            Projectile.AddForce(CurrentProjectileForce, ForceMode.Impulse);

                            Destroy(Projectile.gameObject, ProjectileLifetime);
                        }
                    }
                }

                if (!NoRecoil)
                {
                    HandleRecoil();
                }

                LastShotTime = Time.time;
            }

            if (MultiMuzzle)
            {
                if (CurrentMuzzle < MuzzleArray.Length - 1)
                {
                    CurrentMuzzle++;
                }
                else
                {
                    CurrentMuzzle = 0;
                }
            }
        }
        else
        {
            StartReload();
        }

    }

    void BulletHit (RaycastHit HitObject, int HitOrder)
    {
        int HitEffectIndex;
        Debug.Log(HitObject.transform.name + " was hit by a bullet;");

        EnemyStats Enemy = HitObject.transform.GetComponentInParent<EnemyStats>();
        PlayerStats OtherPlayer = HitObject.transform.GetComponentInParent<PlayerStats>();
        Destructible DestructibleObject = HitObject.transform.GetComponentInParent<Destructible>();
        PhysicMaterial HitObjectMaterial = HitObject.transform.GetComponentInParent<Collider>().sharedMaterial;
        GameObject Impact;

        if (Enemy != null)
        {
            if (!Enemy.Dead)
            {
                /*
                Penetration values:
                1f to 100f -> Goes only through the first target
                101f to 200f -> Goes only through the first and second target
                201f to 300f -> Goes only through the first, second and third target
                [...]
             
                DamageDropoffResistance values
                100f -> Low resistance; Distance of 100 units reduces damage by 50%
                200f -> Average resistance; Distance of 100 units reduces damage by 25%
                500f -> Medium resistance; Distance of 100 units reduces damage by 10%
                1000f -> High resistance; Distance of 100 units reduces damage by 5%
                */

                float WallResistance = Mathf.Max(Damage / 1000f * (HitOrder * 500f - Penetration), 0f);

                float DamageDropoff = Mathf.Max(Damage / 1000f * (HitObject.distance * 500f / DamageDropoffResistance), 0f);

                CurrentDamage = Damage - (WallResistance + DamageDropoff);

                if (PrintDamageFactors)
                {
                    print("CurrentDamage:" + CurrentDamage);
                    print("WallResistance:" + WallResistance);
                    print("DamageDropoff:" + DamageDropoff);
                }

                if (CurrentDamage < 0f)
                {
                    return;
                }

                string HitRegionName = HitObject.transform.name;
                bool Headshot = false;

                switch (HitRegionName)
                {
                    case "Head":
                        CurrentDamageMultiplier = HeadshotDamageMultiplier;
                        Headshot = true;
                        break;

                    case "Spine":
                        CurrentDamageMultiplier = BodyshotDamageMultiplier;
                        break;

                    case "Thigh":
                        CurrentDamageMultiplier = LegshotDamageMultiplier;
                        break;

                    case "Leg":
                        CurrentDamageMultiplier = LegshotDamageMultiplier;
                        break;

                    case "Shoulder":
                        CurrentDamageMultiplier = ArmshotDamageMultiplier;
                        break;

                    case "Arm":
                        CurrentDamageMultiplier = ArmshotDamageMultiplier;
                        break;

                    case "Hand":
                        CurrentDamageMultiplier = ArmshotDamageMultiplier;
                        break;

                    default:
                        CurrentDamageMultiplier = BodyshotDamageMultiplier;
                        break;
                }

                Enemy.Hurt(CurrentDamage, Penetration, CurrentDamageMultiplier, Headshot);

                if (HUD != null)
                {
                    HUD.Hitmarker.CrossFadeAlpha(1f, 0f, false);
                    HUD.Hitmarker.CrossFadeAlpha(0f, HUD.HitmarkerDuration, false);
                }
            }
        }
        else if (OtherPlayer != null)
        {
            if (!OtherPlayer.Dead)
            {
                OtherPlayer.Hurt(CurrentDamage);

                if (HUD != null)
                {
                    HUD.Hitmarker.CrossFadeAlpha(1f, 0f, false);
                    HUD.Hitmarker.CrossFadeAlpha(0f, HUD.HitmarkerDuration, false);
                }
            }
        }
        else if (DestructibleObject != null)
        {
            if (!DestructibleObject.Dead)
            {
                DestructibleObject.Hurt(Damage, Penetration);

                if (HUD != null)
                {
                    HUD.Hitmarker.CrossFadeAlpha(1f, 0f, false);
                    HUD.Hitmarker.CrossFadeAlpha(0f, HUD.HitmarkerDuration, false);
                }
            }
        }

        HitEffectIndex = PhysicMaterialCollectionScript.SetImpact(HitObjectMaterial);

        Impact = Instantiate(ImpactEffectCollectionScript.ImpactEffectArray [HitEffectIndex], HitObject.point, Quaternion.LookRotation(HitObject.normal));
        Destroy(Impact, 2f);

        if (ShowImpacts)
        {
            GameObject DebugImpact;
            DebugImpact = Instantiate(ImpactEffectCollectionScript.ImpactEffectArray [ImpactEffectCollection.DebugEffectIndex], HitObject.point, Quaternion.LookRotation(HitObject.normal));
            Destroy(DebugImpact, 120f);
        }
    }

    void StartReload ()
    {
		if (TotalAmmo > 0)
		{
            Reloading = true;

            if (AnimatorController != null)
            {
                AnimatorController.CrossFadeInFixedTime(ReloadAnimationName, 0f);
            }

            AnimatorController.SetLayerWeight(1, 0);

            StartCoroutine(Reload());

            CurrentFireDelay = ReloadDelay;
        }
		else
		{
			NoAmmoSound.Play ();
		}
    }

    IEnumerator Reload ()
    {
        yield return new WaitForSeconds(ReloadDelay);

        ReloadSound.Play();

        if (SingleBulletReload)
        {
            TotalAmmo--;
            Ammo++;
            Reloading = false;
        }
        else
        {
            int ReloadingAmount = AmmoCapacity - Ammo;

            if (TotalAmmo > ReloadingAmount)
            {
                TotalAmmo -= ReloadingAmount;
                Ammo += ReloadingAmount;
            }
            else
            {
                Ammo += TotalAmmo;
                TotalAmmo = 0;
            }

            Reloading = false;
        }

        if (HUD != null)
        {
            HUD.AmmoCounter.text = Ammo + "/" + TotalAmmo;
        }

        CurrentRecoilIndex = 0;
    }

    void Inspect ()
    {
        if (AnimatorController != null)
        {
            AnimatorController.CrossFadeInFixedTime(InspectAnimationName, 0.2f);
        }
    }

    void UpdateCrosshair ()
    {
        if (MultiMuzzle)
        {
            if (Physics.Raycast(MuzzleTransformArray [CurrentMuzzle].position, transform.forward, out AimPoint, MaxRange)) //Raycast is called every frame to update crosshair position//
            {
                AimingAtObject = true;
                Vector2 ScreenPos = FPCamera.WorldToScreenPoint(AimPoint.point);

                if (HUD != null)
                {
                    HUD.DynamicCrosshair.rectTransform.position = ScreenPos;
                    HUD.Hitmarker.rectTransform.position = ScreenPos;
                }
            }
            else
            {
                AimingAtObject = false;

                if (HUD != null)
                {
                    HUD.DynamicCrosshair.rectTransform.position = HUD.CenterCrosshair.rectTransform.position;
                }
            }
        }
        else
        {
            if (Physics.Raycast(MuzzleTransform.position, transform.forward, out AimPoint, MaxRange)) //Raycast is called every frame to update crosshair position//
            {
                AimingAtObject = true;
                Vector2 ScreenPos = FPCamera.WorldToScreenPoint(AimPoint.point);

                if (HUD != null)
                {
                    HUD.DynamicCrosshair.rectTransform.position = ScreenPos;
                    HUD.Hitmarker.rectTransform.position = ScreenPos;
                }
            }
            else
            {
                AimingAtObject = false;

                if (HUD != null)
                {
                    HUD.DynamicCrosshair.rectTransform.position = HUD.CenterCrosshair.rectTransform.position;
                }
            }
        }

        if (HUD != null)
        {
            HUD.Hitmarker.rectTransform.position = HUD.DynamicCrosshair.rectTransform.position;
        }
    }

    void HandleRecoil ()
    {
        if (Time.time - LastShotTime >= RecoilResetTime)
        {
            SetGunRotation(RecoilDivergence + RecoilPattern[0]);
            CurrentRecoilIndex = 1;
        }
        else
        {
            SetGunRotation(RecoilDivergence + RecoilPattern [CurrentRecoilIndex]);

            if (CurrentRecoilIndex + 1 <= RecoilPattern.Length - 1)
            {
                CurrentRecoilIndex++;
            }
            else
            {
                CurrentRecoilIndex = 0;
            }
        }
    }

    void SetGunRotation (Vector3 Direction)
    {
        RecoilDivergence = Direction;
        transform.localRotation = Quaternion.Euler(RecoilDivergence);
    }

    IEnumerator CasingSpawn()
    {
        yield return new WaitForSeconds(CasingDelay);

        GameObject DuplicatedCasingModel = Instantiate(CasingPrefab, CasingEjectionTransform.position, Random.rotation);
        Rigidbody CasingRigidbody = DuplicatedCasingModel.GetComponent<Rigidbody>();
        CasingRigidbody.AddForce(CasingEjectionTransform.transform.forward * EjectSpeed, ForceMode.VelocityChange);
        Destroy(DuplicatedCasingModel, DestroyTime);
    }
}
