using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableItem : MonoBehaviour
{
    [Header("Main")]
    public string ItemName = "TestItem";
    public int ItemType; //Each number represents the type of item this is//
    private const int GunItemType = 0;
    private const int MeleeItemType = 1;
    private const int ThrowableItemType = 2;
    private const int PlaceableItemType = 3;
    private const int SingleUseItemType = 4;
    private const int AmmunitionItemType = 5;
    public float UseDelay; //How much time the player must wait after using this item once//
    private float CurrentUseDelay;
    public float EquipDelay = 0.4f; //Time it takes to equip this item//
    public bool RemoteController; //Use the alternate use button to remotely control objects//

    [Header("Animations")]
    private Animator AnimatorController;
    public string IdleAnimationName = "Idle";
    public string UseAnimationName = "Use";
    public string ThrowAnimationName = "Throw";
    public string PlaceAnimationName = "Place";
    public string EquipAnimationName = "Equip";
    private bool Equipped;
    public GameObject ModelUnequipped; //The gun when loose on the ground//
    public GameObject ModelEquipped; //The gun when equipped by player//

    [Header("Throwable")]
    public GameObject ThrowableItemPrefab;
    public float ThrowableItemLifetime; //How much time the throwable item stays alive before vanishing from existence//
    public Vector3 ThrowForce;
    public Vector3 ThrowSpin;

    [Header("Placeable")]
    public GameObject PlaceableItemPrefab;
    public float PlaceableDistance; //How far can the placeable object be placed from the player//

    [Header("Single Use")]
    public float HealingPower;
    public bool CureBleeding;
    public float StaminaPower;
    private Explosive[] RemoteExplosiveArray = new Explosive [32];
    public bool Detonator; //Detonates explosives released by this item//

    [Header("Main References")]
    [Space(50)]
    public LayerMask SolidLayer;
    private GameManager GameManagerScript;
    private ControlsManager ControlsManagerScript;
    private Item ItemScript;
    private PlayerStats Player;
    private PlayerHUD HUD;
    private Camera FPCamera;
    public GameObject DroppedItemPrefab;
    public float DroppedItemLifetime; //How much time the dropped item stays alive before vanishing from existence//
    public Vector3 DropForce;
    public Vector3 DropSpin;

    void Awake()
    {
        RemoteExplosiveArray = new Explosive [32];

        GameManagerScript = FindObjectOfType<GameManager>();
        ControlsManagerScript = GameManagerScript.GetComponent<ControlsManager>();

        if (ModelEquipped != null)
        {
            ModelEquipped.SetActive(true);
        }
        AnimatorController = GetComponentInChildren<Animator>();

        ItemScript = GetComponent<Item>();
        ItemScript.CheckItemType();
        ItemType = ItemScript.ItemType;

        Unequip();
        enabled = false;
    }

    void Start()
    {
        Load();

        if (ItemName == null)
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

        HUD.Hitmarker.rectTransform.position = HUD.DynamicCrosshair.rectTransform.position;

        if (CurrentUseDelay > 0f)
        {
            CurrentUseDelay -= Time.deltaTime;
        }

        bool TryBasicThrow;
        bool TryBasicPlace;
        bool TryBasicUse;
        bool TryAltUse;

        if (Player.PlayerMovementStatus != PlayerStats.PlayerState.Sprinting)
        {
            TryBasicThrow = Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["BasicThrow"]);
            TryBasicPlace = Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["BasicPlace"]);
            TryBasicUse = Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["BasicUse"]);
            TryAltUse = Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["AltUse"]);

            if (CurrentUseDelay <= 0f)
            {
                switch (ItemType)
                {
                    case ThrowableItemType:
                        if (TryBasicThrow)
                        {
                            ThrowStart();
                        }
                        break;

                    case PlaceableItemType:
                        if (TryBasicPlace)
                        {
                            PlaceStart();
                        }
                        break;

                    case SingleUseItemType:
                        if (TryBasicUse)
                        {
                            UseStart(false);
                        }
                        break;

                    default:
                        Debug.LogError(ItemName + "is an invalid item;");
                        Destroy(gameObject);
                        break;
                }

                if (RemoteController && TryAltUse)
                {
                    UseStart(true);
                }
            }
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

    public void Load()
    {
        FPCamera = GetComponentInParent<Camera>();
        Player = FPCamera.GetComponentInParent<PlayerStats>();
        HUD = FPCamera.GetComponentInParent<PlayerHUD>();

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
            HUD.Hitmarker.enabled = false;
            HUD.WeaponTag.text = ItemName;
            HUD.AmmoCounter.text = ItemScript.ItemAmount.ToString();
        }

        CurrentUseDelay = EquipDelay;

        if (AnimatorController != null)
        {
            AnimatorController.CrossFadeInFixedTime(EquipAnimationName, 0f);
        }
    }

    public void Unequip()
    {
        Equipped = false;

        StopAllCoroutines();

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

    void ThrowStart ()
    {
        CurrentUseDelay = UseDelay;

        if (ItemScript.ItemAmount <= 0)
        {
            if (!RemoteController)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (AnimatorController != null)
            {
                AnimatorController.CrossFadeInFixedTime(ThrowAnimationName, 0f);
            }

            if (ThrowableItemPrefab != null)
            {
                StartCoroutine(Throw());
            }
        }
    }

    IEnumerator Throw ()
    {
        yield return new WaitForSeconds(UseDelay);

        Rigidbody ThrownObject = Instantiate(ThrowableItemPrefab, FPCamera.transform.position, FPCamera.transform.rotation).GetComponent<Rigidbody>();
        if (ThrownObject != null)
        {
            Debug.Log(gameObject.name + " throws " + ItemName + ";");

            Quaternion RotateToLocal = FPCamera.transform.rotation;
            Vector3 CurrentThrowForce = RotateToLocal * ThrowForce;
            Vector3 CurrentThrowSpin = RotateToLocal * ThrowSpin;

            ThrownObject.AddForce(CurrentThrowForce, ForceMode.Impulse);
            ThrownObject.angularVelocity = CurrentThrowSpin;

            Explosive ThrownExplosive = ThrownObject.GetComponent<Explosive>();
            if (ThrownExplosive != null)
            {
                ThrownExplosive.ExplosiveOwner = Player;

                for (int i = 0; i < RemoteExplosiveArray.Length; i++)
                {
                    if (RemoteExplosiveArray [i] == null)
                    {
                        RemoteExplosiveArray [i] = ThrownExplosive;
                        break;
                    }
                }
            }

            Destroy(ThrownObject.gameObject, ThrowableItemLifetime);
        }

        ItemScript.ItemAmount--;
        HUD.AmmoCounter.text = ItemScript.ItemAmount.ToString();

        if (ItemScript.ItemAmount <= 0 && !RemoteController)
        {
            Destroy(gameObject);
        }

        Equip();
    }

    void PlaceStart ()
    {
        CurrentUseDelay = UseDelay;

        if (ItemScript.ItemAmount <= 0)
        {
            if (!RemoteController)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (AnimatorController != null)
            {
                AnimatorController.CrossFadeInFixedTime(PlaceAnimationName, 0f);
            }

            StartCoroutine(Place());
        }
    }

    IEnumerator Place()
    {
        yield return new WaitForSeconds(UseDelay);

        Vector3 PlacePath = FPCamera.transform.forward;
        RaycastHit AimPoint;

        if (Physics.Raycast(FPCamera.transform.position, PlacePath, out AimPoint, PlaceableDistance, SolidLayer))
        {
            Quaternion PlacedObjectRotation = Quaternion.Euler(0f, FPCamera.transform.eulerAngles.y, 0f);

            if (PlaceableItemPrefab != null)
            {
                Vector3 PlacedObjectPosition = new Vector3(AimPoint.point.x, AimPoint.point.y + (transform.lossyScale.y / 2f), AimPoint.point.z);
                Transform PlacedObject = Instantiate(PlaceableItemPrefab, PlacedObjectPosition, PlacedObjectRotation).transform;

                if (PlacedObject != null)
                {
                    Explosive PlacedExplosive = PlacedObject.GetComponent<Explosive>();
                    if (PlacedExplosive != null)
                    {
                        PlacedExplosive.ExplosiveOwner = Player;

                        for (int i = 0; i < RemoteExplosiveArray.Length; i++)
                        {
                            if (RemoteExplosiveArray [i] == null)
                            {
                                RemoteExplosiveArray [i] = PlacedExplosive;
                                break;
                            }
                        }
                    }

                    Debug.Log(gameObject.name + " places " + ItemName + ";");
                }
            }

            ItemScript.ItemAmount--;
            HUD.AmmoCounter.text = ItemScript.ItemAmount.ToString();

            if (ItemScript.ItemAmount <= 0 && !RemoteController)
            {
                Destroy(gameObject);
            }
        }
    }

    public void UseStart (bool Alt)
    {
        CurrentUseDelay = UseDelay;

        if (!Alt)
        {
            if (ItemScript.ItemAmount <= 0)
            {
                if (!RemoteController)
                {
                    Unequip();
                    Destroy(gameObject);
                }
            }
            else
            {
                if (AnimatorController != null)
                {
                    AnimatorController.CrossFadeInFixedTime(UseAnimationName, 0f);
                }

                StartCoroutine(BasicUse());
            }
        }
        else
        {
            //AltUse animation//

            if (ItemScript.ItemAmount <= 0 && !RemoteController)
            {
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(AltUse());
            }
        }
    }

    IEnumerator BasicUse()
    {
        yield return new WaitForSeconds(UseDelay);

        if (Player.CurrentHealth < Player.Health)
        {
            Player.Heal(HealingPower, CureBleeding);
        }
        if (Player.CurrentStamina < Player.Stamina)
        {
            Player.GiveStamina(StaminaPower);
        }

        Explosive UseExplosive = GetComponent<Explosive>();
        if (UseExplosive != null)
        {
            UseExplosive.ExplosiveOwner = Player;

            for (int i = 0; i < RemoteExplosiveArray.Length; i++)
            {
                if (RemoteExplosiveArray [i] == null)
                {
                    RemoteExplosiveArray [i] = UseExplosive;
                    break;
                }
            }
        }

        ItemScript.ItemAmount--;
        HUD.AmmoCounter.text = ItemScript.ItemAmount.ToString();

        if (DroppedItemPrefab != null)
        {
            Trash();
        }

        if (ItemScript.ItemAmount <= 0 && !RemoteController)
        {
            Destroy(gameObject);
        }

        Equip();
    }

    IEnumerator AltUse()
    {
        yield return new WaitForSeconds(UseDelay);

        if (Detonator)
        {
            foreach (Explosive RemoteExplosive in RemoteExplosiveArray)
            {
                if (RemoteExplosive != null)
                {
                    StartCoroutine(RemoteExplosive.Explode());
                }
            }
        }
    }

    void Trash ()
    {
        Rigidbody DroppedObject = Instantiate(DroppedItemPrefab, FPCamera.transform.position, FPCamera.transform.rotation).GetComponent<Rigidbody>();
        if (DroppedObject != null)
        {
            Debug.Log(gameObject.name + " trashes " + ItemName + ";");

            Quaternion RotateToLocal = FPCamera.transform.rotation;
            Vector3 CurrentDropForce = RotateToLocal * DropForce;
            Vector3 CurrentDropSpin = RotateToLocal * DropSpin;

            DroppedObject.AddForce(CurrentDropForce, ForceMode.Impulse);
            DroppedObject.angularVelocity = CurrentDropSpin;

            Destroy(DroppedObject.gameObject, DroppedItemLifetime);
        }
    }
}
