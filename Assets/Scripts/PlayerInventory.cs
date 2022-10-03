using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Properties")]
    public Item [] InventorySlots;
    public int SelectedSlot;
    public int PrimarySlot;
    public int SecondarySlot;
    public int MeleeSlot;
    private bool HasGun;

    [Header("Sway Properties")] //Moves the first person hands when the player looks around//
    public float SwayAmount = 0.01f;
    public float SwayRotationAmount = 1f;
    public float SwaySpeed = 5f;
    public float SwayMaxAmount = 0.1f;
    public float SwaySmoothness = 8.0f;
    public Vector3 FPHandsMainPosition;
    private Vector3 FPHandsInitialPosition;

    [Header("Interaction Properties")]
    public LayerMask InteractionMask;
    public float InteractionReach;
    private RaycastHit AimPoint;
    public Vector3 DropForce; //How far the player drops an item//
    public Vector3 DropSpin; //How much a dropped item spins around//
    public bool RandomDropSpin;

    [Header("Main References")]
    [Space(50)]
    public PlayerStats Player;
    public PlayerHUD HUD;
    public PlayerMovement Movement;
    public MouseController MouseControl;
    public Camera FPCamera;
    public AudioSource PickUpSound;
    public GameObject DefaultItem;
    private GameObject CurrentDefaultItem;

    //Each number represents a type of item//
    private const int GunItemType = 0;
    private const int MeleeItemType = 1;
    private const int ThrowableItemType = 2;
    private const int PlaceableItemType = 3;
    private const int SingleUseItemType = 4;
    private const int AmmunitionItemType = 5;


    private GameManager GameManagerScript;
    private ControlsManager ControlsManagerScript;

    void Awake()
    {
        GameManagerScript = FindObjectOfType<GameManager>();
        ControlsManagerScript = GameManagerScript.GetComponent<ControlsManager>();
        SelectedSlot = 1;
    }

    void Start()
    {
        CurrentDefaultItem = Instantiate(DefaultItem, FPCamera.transform);
        CurrentDefaultItem.transform.position = FPCamera.transform.position;
        CurrentDefaultItem.transform.rotation = FPCamera.transform.rotation;

        Melee CurrentDefaultMelee = CurrentDefaultItem.GetComponent<Melee>();
        if (CurrentDefaultMelee != null)
        {
            CurrentDefaultMelee.Load();
            CurrentDefaultMelee.Equip();
        }

        Guns CurrentDefaultGun = CurrentDefaultItem.GetComponent<Guns>();
        if (CurrentDefaultGun != null)
        {
            CurrentDefaultGun.Load();
            CurrentDefaultGun.Equip();
        }

        UsableItem CurrentDefaultUsable = CurrentDefaultItem.GetComponent<UsableItem>();
        if (CurrentDefaultUsable != null)
        {
            CurrentDefaultUsable.Load();
            CurrentDefaultUsable.Equip();
        }

        Movement = GetComponent<PlayerMovement>();
        Player = GetComponent<PlayerStats>();
        MouseControl = GetComponentInChildren<MouseController>();
        if (HUD != null)
        {
            HUD = GetComponent<PlayerHUD>();
        }
        FPCamera = MouseControl.GetComponent<Camera>();
        HasGun = false;

        SelectItem();
    }

    void Update()
    {
        if (GameManagerScript.GameStatus != GameManager.GameState.InGame)
        {
            return;
        }

        if (Player.Dead)
        {
            return;
        }

        if (Input.GetKeyDown(ControlsManagerScript.ControlDictionary["ItemInteract"]))
        {
            Interact();
        }

        if (Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["ItemDrop"]))
        {
            DropItem(FPCamera.transform.rotation);
        }

        if (Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["ItemSlot1"]))
        {
            SelectedSlot = 1;
            SelectItem();
        }

        if (Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["ItemSlot2"]))
        {
            SelectedSlot = 2;
            SelectItem();
        }

        if (Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["ItemSlot3"]))
        {
            SelectedSlot = 3;
            SelectItem();
        }

        if (Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["ItemSlot4"]))
        {
            SelectedSlot = 4;
            SelectItem();
        }

        if (Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["ItemSlot5"]))
        {
            SelectedSlot = 5;
            SelectItem();
        }

        if (Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["ItemSlot6"]))
        {
            SelectedSlot = 6;
            SelectItem();
        }

        if (Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["ItemSlot7"]))
        {
            SelectedSlot = 7;
            SelectItem();
        }

        if (Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["ItemSlot8"]))
        {
            SelectedSlot = 8;
            SelectItem();
        }

        if (Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["ItemSlot9"]))
        {
            SelectedSlot = 9;
            SelectItem();
        }

        Sway(); //Moves the first person hands when the player looks around//
    }

    void SelectItem()
    {
        GameObject SelectedItem;

        if (InventorySlots [SelectedSlot - 1] != null)
        {
            SelectedItem = InventorySlots [SelectedSlot - 1].gameObject;
            UnselectItems(true);
        }
        else
        {
            SelectedItem = CurrentDefaultItem;
            UnselectItems(false);
        }

        if (SelectedItem != null)
        {
            SelectedItem.SetActive(true);
            SelectedItem.transform.parent = FPCamera.transform;
            SelectedItem.transform.localPosition = FPHandsMainPosition;
            FPHandsInitialPosition = FPHandsMainPosition;

            Guns SelectedGun = SelectedItem.GetComponent<Guns>();
            Melee SelectedMelee = SelectedItem.GetComponent<Melee>();
            UsableItem SelectedUsable = SelectedItem.GetComponent<UsableItem>();

            if (SelectedGun != null)
            {
                HasGun = true;
                SelectedGun.enabled = true;
                SelectedGun.Equip();
            }
            else if (SelectedMelee != null)
            {
                SelectedMelee.enabled = true;
                SelectedMelee.Equip();
            }
            else if (SelectedUsable != null)
            {
                SelectedUsable.enabled = true;
                SelectedUsable.Equip();
            }
        }
    }

    void UnselectItems(bool UnselectDefault)
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            if (InventorySlots [i] != InventorySlots [SelectedSlot - 1])
            {
                if (InventorySlots[i] != null)
                {
                    GameObject UnselectedItem = InventorySlots [i].gameObject;
                    Guns UnselectedGun = UnselectedItem.GetComponent<Guns>();
                    Melee UnselectedMelee = UnselectedItem.GetComponent<Melee>();
                    UsableItem UnselectedUsable = UnselectedItem.GetComponent<UsableItem>();

                    if (UnselectedItem != null)
                    {
                        UnselectedItem.SetActive(false);
                    }

                    if (UnselectedGun != null)
                    {
                        HasGun = true;
                        UnselectedGun.Unequip();
                        UnselectedGun.enabled = false;
                    }
                    else if (UnselectedMelee != null)
                    {
                        UnselectedMelee.Unequip();
                        UnselectedMelee.enabled = false;
                    }
                    else if (UnselectedUsable != null)
                    {
                        UnselectedUsable.Unequip();
                        UnselectedUsable.enabled = false;
                    }
                }
            }
        }

        if (UnselectDefault)
        {
            if (CurrentDefaultItem != null)
            {
                Guns UnselectedGun = CurrentDefaultItem.GetComponent<Guns>();
                Melee UnselectedMelee = CurrentDefaultItem.GetComponent<Melee>();
                UsableItem UnselectedUsable = CurrentDefaultItem.GetComponent<UsableItem>();

                if (UnselectedGun != null)
                {
                    HasGun = true;
                    UnselectedGun.Unequip();
                    UnselectedGun.enabled = false;
                }
                else if (UnselectedMelee != null)
                {
                    UnselectedMelee.Unequip();
                    UnselectedMelee.enabled = false;
                }
                else if (UnselectedUsable != null)
                {
                    UnselectedUsable.Unequip();
                    UnselectedUsable.enabled = false;
                }

                CurrentDefaultItem.SetActive(false);
            }
        }

        if (HUD != null)
        {
            HUD.DynamicCrosshair.rectTransform.position = HUD.CenterCrosshair.rectTransform.position;
            HUD.Hitmarker.enabled = false;
        }
    }

    public void PickUpItem(Item PickedItem)
    {
        bool InventoryFull = true;

        GameObject ItemObject = PickedItem.gameObject;
        Collider ItemCollider = ItemObject.GetComponent<Collider>();
        Rigidbody ItemRigidbody = ItemObject.GetComponent<Rigidbody>();
        UsableItem Usable = ItemObject.GetComponent<UsableItem>();

        switch (PickedItem.ItemType)
        {
            case GunItemType:
                Guns Gun = ItemObject.GetComponent<Guns>();

                if (Gun != null)
                {
                    if (!Gun.HandGun)
                    {
                        if (InventorySlots [PrimarySlot - 1] == null)
                        {
                            InventorySlots [PrimarySlot - 1] = PickedItem;
                            SelectedSlot = PrimarySlot;
                            InventoryFull = false;
                        }
                        else
                        {
                            for (int i = 3; i < InventorySlots.Length; i++)
                            {
                                if (InventorySlots [i] == null)
                                {
                                    InventorySlots [i] = PickedItem;
                                    SelectedSlot = i + 1;
                                    InventoryFull = false;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (InventorySlots [SecondarySlot - 1] == null)
                        {
                            InventorySlots [SecondarySlot - 1] = PickedItem;
                            SelectedSlot = SecondarySlot;
                            InventoryFull = false;
                        }
                        else
                        {
                            for (int i = 3; i < InventorySlots.Length; i++)
                            {
                                if (InventorySlots [i] == null)
                                {
                                    InventorySlots [i] = PickedItem;
                                    SelectedSlot = i + 1;
                                    InventoryFull = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (!InventoryFull)
                    {
                        ItemObject.transform.parent = FPCamera.transform;
                        ItemObject.transform.SetPositionAndRotation(FPCamera.transform.position, FPCamera.transform.rotation);
                        Gun.Load();

                        HasGun = true;
                    }
                }
                else
                {
                    Debug.LogError(gameObject.name + "does not have a Gun script;");
                    Destroy(ItemObject);
                }
                break;

            case MeleeItemType:
                Melee Weapon = ItemObject.GetComponent<Melee>();

                if (Weapon != null)
                {
                    if (InventorySlots [MeleeSlot - 1] == null)
                    {
                        InventorySlots [MeleeSlot - 1] = PickedItem;
                        SelectedSlot = MeleeSlot;
                        InventoryFull = false;
                    }
                    else
                    {
                        for (int i = 3; i < InventorySlots.Length; i++)
                        {
                            if (InventorySlots [i] == null)
                            {
                                InventorySlots [i] = PickedItem;
                                SelectedSlot = i + 1;
                                InventoryFull = false;
                                break;
                            }
                        }
                    }

                    if (!InventoryFull)
                    {
                        ItemObject.transform.parent = FPCamera.transform;
                        ItemObject.transform.SetPositionAndRotation(FPCamera.transform.position, FPCamera.transform.rotation);
                        Weapon.Load();
                    }
                }
                else
                {
                    Debug.LogError(gameObject.name + "does not have a Melee script;");
                    Destroy(ItemObject);
                }
                break;

            case ThrowableItemType:
                if (Usable != null)
                {
                    for (int i = 3; i < InventorySlots.Length; i++)
                    {
                        if (InventorySlots [i] != null)
                        {
                            if (InventorySlots [i].ID == PickedItem.ID)
                            {
                                InventorySlots [i].ItemAmount += PickedItem.ItemAmount;
                                Destroy(ItemObject);
                                InventoryFull = false;
                                break;
                            }
                        }
                        else
                        {
                            InventoryFull = false;
                            InventorySlots [i] = PickedItem;
                            break;
                        }
                    }

                    if (!InventoryFull)
                    {
                        ItemObject.transform.parent = FPCamera.transform;
                        ItemObject.transform.SetPositionAndRotation(FPCamera.transform.position, FPCamera.transform.rotation);
                        Usable.Load();
                    }
                }
                break;

            case PlaceableItemType:
                if (Usable != null)
                {
                    for (int i = 3; i < InventorySlots.Length; i++)
                    {
                        if (InventorySlots [i] != null)
                        {
                            if (InventorySlots [i].ID == PickedItem.ID)
                            {
                                InventorySlots [i].ItemAmount += PickedItem.ItemAmount;
                                Destroy(ItemObject);
                                InventoryFull = false;
                                break;
                            }
                        }
                        else
                        {
                            InventoryFull = false;
                            InventorySlots [i] = PickedItem;
                            break;
                        }
                    }

                    if (!InventoryFull)
                    {
                        ItemObject.transform.parent = FPCamera.transform;
                        ItemObject.transform.SetPositionAndRotation(FPCamera.transform.position, FPCamera.transform.rotation);
                        Usable.Load();
                    }
                }
                break;

            case SingleUseItemType:
                if (Usable != null)
                {
                    for (int i = 3; i < InventorySlots.Length; i++)
                    {
                        if (InventorySlots [i] != null)
                        {
                            if (InventorySlots [i].ID == PickedItem.ID)
                            {
                                InventorySlots [i].ItemAmount += PickedItem.ItemAmount;
                                Destroy(ItemObject);
                                InventoryFull = false;
                                break;
                            }
                        }
                        else
                        {
                            InventoryFull = false;
                            InventorySlots [i] = PickedItem;
                            break;
                        }
                    }

                    if (!InventoryFull)
                    {
                        ItemObject.transform.parent = FPCamera.transform;
                        ItemObject.transform.SetPositionAndRotation(FPCamera.transform.position, FPCamera.transform.rotation);
                        Usable.Load();
                    }
                }
                break;

            case AmmunitionItemType:
                if (HasGun)
                {
                    bool GaveAmmo = false;

                    for (int i = 0; i < InventorySlots.Length; i++)
                    {
                        if (SelectedSlot == i + 1)
                        {
                            if (InventorySlots [i] != null)
                            {
                                Guns SelectedGun = InventorySlots [i].GetComponent<Guns>();

                                if (SelectedGun != null)
                                {
                                    SelectedGun.TotalAmmo += (SelectedGun.AmmoBoxCapacity * PickedItem.ItemAmount);
                                    if (HUD != null)
                                    {
                                        HUD.AmmoCounter.text = SelectedGun.Ammo + "/" + SelectedGun.TotalAmmo;
                                    }
                                    GaveAmmo = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!GaveAmmo)
                    {
                        for (int i = 0; i < InventorySlots.Length; i++)
                        {
                            if (InventorySlots [i] != null)
                            {
                                Guns FoundGun = InventorySlots [i].GetComponent<Guns>();

                                if (FoundGun != null)
                                {
                                    FoundGun.TotalAmmo += (FoundGun.AmmoBoxCapacity * PickedItem.ItemAmount);
                                    if (HUD != null)
                                    {
                                        HUD.AmmoCounter.text = FoundGun.Ammo + "/" + FoundGun.TotalAmmo;
                                    }
                                    GaveAmmo = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (GaveAmmo)
                    {
                        InventoryFull = false;
                        Destroy(ItemObject);
                    }
                }
                break;

            default:
                Debug.LogError(gameObject.name + "is an invalid item;");
                Destroy(ItemObject);
                InventoryFull = true;
                break;
        }

        if (!InventoryFull)
        {
            if (PickedItem != null)
            {
                PickedItem.UIActive = false;

                if (PickedItem.ItemUIPrefab != null)
                {
                    PickedItem.UpdateUI();
                }
            }

            if (ItemObject != null)
            {
                ItemObject.SetActive(false);
            }

            if (ItemRigidbody != null)
            {
                ItemRigidbody.isKinematic = true;
                ItemRigidbody.useGravity = false;
                ItemRigidbody.velocity = new Vector3(0f, 0f, 0f);
                ItemRigidbody.angularVelocity = new Vector3(0f, 0f, 0f);
            }
            if (ItemCollider != null)
            {
                ItemCollider.enabled = false;
            }
            PickUpSound.clip = PickedItem.PickUpSoundClip;
            PickUpSound.Play();

            SelectItem();

            Debug.Log(gameObject.name + " picked up " + AimPoint.transform.name + ";");
        }
    }

    public void Interact ()
    {
        Vector3 InteractionPath = FPCamera.transform.forward;

        if (Physics.Raycast(FPCamera.transform.position, InteractionPath, out AimPoint, InteractionReach, InteractionMask))
        {
            Debug.Log(gameObject.name + " interacts with " + AimPoint.transform.name + ";");

            if (AimPoint.transform.CompareTag("Loot"))
            {
                Item PickedItem = AimPoint.transform.GetComponent<Item>();

                if (PickedItem != null)
                {
                    PickUpItem(PickedItem);
                }
            }
        }
    }

    public void DropItem (Quaternion DropRotation)
    {
        if (InventorySlots [SelectedSlot - 1] != null)
        {
            GameObject SelectedItem = InventorySlots [SelectedSlot - 1].gameObject;

            Debug.Log(gameObject.name + " drops " + SelectedItem.name + ";");

            if (SelectedItem != null)
            {
                InventorySlots [SelectedSlot - 1] = null;
                SelectItem();

                Item SelectedItemItem = SelectedItem.GetComponent<Item>();
                Collider ItemCollider = SelectedItem.GetComponent<Collider>();
                Rigidbody ItemRigidbody = SelectedItem.GetComponent<Rigidbody>();
                Guns DroppedGun = SelectedItem.GetComponent<Guns>();
                Melee DroppedMelee = SelectedItem.GetComponent<Melee>();
                UsableItem DroppedUsable = SelectedItem.GetComponent<UsableItem>();

                SelectedItemItem.UIActive = true;
                SelectedItem.SetActive(true);
                SelectedItem.transform.parent = null;
                SelectedItem.transform.position = FPCamera.transform.position;
                SelectedItem.transform.rotation = FPCamera.transform.rotation;

                if (ItemCollider != null)
                {
                    ItemCollider.enabled = true;
                }
                if (ItemRigidbody != null)
                {
                    ItemRigidbody.isKinematic = false;
                    ItemRigidbody.useGravity = true;

                    Quaternion RotateToLocal = DropRotation;
                    Vector3 CurrentDropForce = RotateToLocal * DropForce;
                    Vector3 CurrentDropSpin = RotateToLocal * DropSpin;
                    if (RandomDropSpin)
                    {
                        CurrentDropSpin.x += Random.Range(-1f, 1f);
                        CurrentDropSpin.y += Random.Range(-1f, 1f);
                        CurrentDropSpin.z += Random.Range(-1f, 1f);

                        CurrentDropSpin.x *= Random.Range(-2f, 2f);
                        CurrentDropSpin.y *= Random.Range(-2f, 2f);
                        CurrentDropSpin.z *= Random.Range(-2f, 2f);
                    }

                    ItemRigidbody.AddForce(CurrentDropForce, ForceMode.Impulse);
                    ItemRigidbody.angularVelocity = CurrentDropSpin;
                }
                if (DroppedGun != null)
                {
                    DroppedGun.Unequip();
                    DroppedGun.enabled = false;
                    int GunAmount = 0;

                    for (int i = 0; i < InventorySlots.Length; i++)
                    {
                        if (InventorySlots [i] != null)
                        {
                            Guns FoundGun = InventorySlots [i].GetComponent<Guns>();
                            if (FoundGun != null)
                            {
                                GunAmount++;
                            }
                        }
                    }

                    if (GunAmount > 0)
                    {
                        HasGun = true;
                    }
                    else
                    {
                        HasGun = false;
                    }
                }
                else if (DroppedMelee != null)
                {
                    DroppedMelee.Unequip();
                    DroppedMelee.enabled = false;
                }
                else if (DroppedUsable != null)
                {
                    DroppedUsable.Unequip();
                    DroppedUsable.enabled = false;
                }
            }

            SelectItem();
        }
    }

    public void DeathDrop ()
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            if (InventorySlots [i] != null)
            {
                SelectedSlot = i + 1;
                SelectItem();

                Vector3 DropRotation;
                DropRotation.x = Random.Range(0f, 360f);
                DropRotation.y = Random.Range(0f, 360f);
                DropRotation.z = Random.Range(0f, 360f);

                DropItem(Quaternion.Euler(DropRotation));
            }
        }
    }

    void Sway ()
    {
        Transform SelectedItem;

        if (InventorySlots [SelectedSlot - 1] != null)
        {
            SelectedItem = InventorySlots [SelectedSlot - 1].transform;
        }
        else
        {
            SelectedItem = CurrentDefaultItem.transform;
        }

        Guns SelectedGun = SelectedItem.GetComponent<Guns>();
        Melee SelectedMelee = SelectedItem.GetComponent<Melee>();

        MouseControl.CurrentGun = SelectedGun;
        MouseControl.CurrentMelee = SelectedMelee;

        if (SelectedGun != null && Time.time - SelectedGun.LastShotTime < SelectedGun.RecoilResetTime)
        {
            return;
        }
        else
        {
            float XMovement = -Input.GetAxis("Mouse X") * SwayAmount;
            float YMovement = -Input.GetAxis("Mouse Y") * SwayAmount;
            XMovement = Mathf.Clamp(XMovement, -SwayMaxAmount, SwayMaxAmount);
            YMovement = Mathf.Clamp(YMovement, -SwayMaxAmount, SwayMaxAmount);

            Vector3 FPHandsFinalPosition = new Vector3(XMovement, YMovement, 0);
            SelectedItem.localPosition = Vector3.Lerp(SelectedItem.localPosition, FPHandsFinalPosition + FPHandsInitialPosition, Time.deltaTime * SwaySmoothness);

            float MouseX = Input.GetAxis("Mouse X") * SwayRotationAmount;
            float MouseY = Input.GetAxis("Mouse Y") * SwayRotationAmount;

            Quaternion RotationSpeed = Quaternion.Euler(MouseY, 0, -MouseX);
            SelectedItem.localRotation = Quaternion.Slerp(SelectedItem.localRotation, RotationSpeed, SwaySpeed * Time.deltaTime);
        }
    }
}
