using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Melee : MonoBehaviour
{
    [Header("Main")]
    public string MeleeName = "TestMelee";
    public bool PlayerDependant; //Uses damage values in the PlayerStats script//

    [Header("Combat Stats")]
    public float Damage;
    public float Penetration = 100f; //How easily can the weapon pierce through armor//
    public float AttackReach; //Anything beyond this is will not be affected by Raycast//
    public bool Knockback = false; //Whether or not this weapon knocks the zombies back//
    public float AttackSpeed; //Number of attacks per second//
    public float FixedAttackDelay; //Time it takes for the attack to hit after performing an attack//
    [HideInInspector]
    public float CurrentAttackDelay; //Time it takes to attack again//
    public float Spread; //How much the attacks diverge from the intended path//
    [HideInInspector]
    public float CurrentSpread; //How much the attacks will diverge from the intended path//

    [Header("Passive Stats")]
    public float Loudness; //How loud this weapon is for enemies//
    public float EquipDelay = 0.4f; //Time it takes to equip this weapon//

    [Header("Animations")]
    private Animator AnimatorController;
    public string IdleAnimationName = "Idle";
    public string Attack1AnimationName = "Attack1";
    public string Attack2AnimationName = "Attack2";
    public string EquipAnimationName = "Equip";
    public string InspectAnimationName = "Inspect";
    private bool Equipped;
    public GameObject ModelUnequipped; //The weapon when loose on the ground//
    public GameObject ModelEquipped; //The weapon when equipped by player//

    [Header("Main References")]
    [Space(50)]
    private Item ItemScript;
    private PlayerStats Player;
    private PlayerHUD HUD;
    private Collider PlayerBody;
    private Camera FPCamera;

    private GameManager GameManagerScript;
    private ControlsManager ControlsManagerScript;
    private ScriptableObjectManager ScriptableObjectManagerScript;
    private PhysicMaterialCollection PhysicMaterialCollectionScript;
    private ImpactEffectCollection ImpactEffectCollectionScript;

    private RaycastHit AimPoint;
    private RaycastHit HitPoint;

    void Awake()
    {
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

        Unequip();
        enabled = false;
    }

    void Start()
    {
        Load();

        if (MeleeName == null)
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

        if (HUD != null)
        {
            HUD.Hitmarker.rectTransform.position = HUD.DynamicCrosshair.rectTransform.position;
        }

        if (CurrentAttackDelay > 0f)
        {
            CurrentAttackDelay -= Time.deltaTime;
        }

        if (Input.GetKey(ControlsManagerScript.ControlDictionary ["BasicAttack"]) && CurrentAttackDelay <= 0f)
        {
            AttackStart();
        }

        if (Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["Inspect"]))
        {
            Inspect();
        }

        CurrentSpread = Spread / 1000f;

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

    public void Load()
    {
        //Loads everything necessary for this component. It's only necessary to load if it's picked up//

        FPCamera = GetComponentInParent<Camera>();
        Player = FPCamera.GetComponentInParent<PlayerStats>();
        if (PlayerDependant)
        {
            Damage = Player.AttackDamage;
        }
        PlayerBody = Player.GetComponentInChildren<Collider>();
        HUD = FPCamera.GetComponentInParent<PlayerHUD>();

        CurrentAttackDelay = 1f / AttackSpeed;
        CurrentSpread = Spread;

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
            HUD.Hitmarker.rectTransform.position = HUD.DynamicCrosshair.rectTransform.position;
            HUD.Hitmarker.enabled = true;
            HUD.Hitmarker.CrossFadeAlpha(0f, 0f, true);
            HUD.WeaponTag.text = MeleeName;
            HUD.AmmoCounter.text = "";
        }

        CurrentAttackDelay = EquipDelay;

        if (AnimatorController != null)
        {
            AnimatorController.CrossFadeInFixedTime(EquipAnimationName, 0f);
        }
    }

    public void Unequip()
    {
        Equipped = false;

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

    public void AttackStart()
    {
        CurrentAttackDelay = 1f / AttackSpeed;

        if (AnimatorController != null)
        {
            AnimatorController.CrossFadeInFixedTime(Attack1AnimationName, 0f);
        }

        StartCoroutine(BasicAttack());
    }

    IEnumerator BasicAttack()
    {
        yield return new WaitForSeconds (FixedAttackDelay);

        Vector3 AttackPath = FPCamera.transform.forward;
        Vector3 AttackSource = FPCamera.transform.position;

        if (CurrentSpread > 0f)
        {
            AttackPath += Random.insideUnitSphere * CurrentSpread;
        }

        RaycastHit[] HitObjects = Physics.RaycastAll(AttackSource, AttackPath, AttackReach);

        for (int i = HitObjects.Length - 1; i >= 0; i--)
        {
            if (HitObjects [i].collider != PlayerBody)
            {
                bool ShortestNumber = true;

                for (int j = i - 1; j > 0; j--)
                {
                    if (HitObjects [j].collider != PlayerBody)
                    {
                        if (i > 0 && HitObjects [i].distance > HitObjects [j].distance)
                        {
                            ShortestNumber = false;
                            break;
                        }
                    }
                }

                if (i <= 0 || (ShortestNumber && HitObjects [i].distance < HitObjects [0].distance))
                {
                    MeleeHit(HitObjects[i]);
                    break;
                }
            }
        }
    }

    void MeleeHit (RaycastHit HitObject)
    {
        int HitEffectIndex;
        Debug.Log(HitObject.transform.name + " was hit by a melee attack;");

        if (Player.CurrentNoise < Loudness)
        {
            Player.CurrentNoise = Loudness;
        }

        EnemyStats Enemy = HitObject.transform.GetComponentInParent<EnemyStats>();
        PlayerStats OtherPlayer = HitObject.transform.GetComponentInParent<PlayerStats>();
        Destructible DestructibleObject = HitObject.transform.GetComponentInParent<Destructible>();
        PhysicMaterial HitObjectMaterial = HitObject.transform.GetComponentInParent<Collider>().sharedMaterial;
        GameObject Impact;

        if (Enemy != null)
        {
            if (!Enemy.Dead)
            {
                Enemy.Hurt(Damage, Penetration, 1f, false, Knockback);

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
                OtherPlayer.Hurt(Damage);

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
    }
    void Inspect()
    {
        if (AnimatorController != null)
        {
            AnimatorController.CrossFadeInFixedTime(InspectAnimationName, 0f);
        }
    }
}
