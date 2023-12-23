using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyInventory : MonoBehaviour
{
    [Header("Inventory Properties")]
    public Item [] InventorySlots;
    public Vector3 DropForce; //How far the player drops an item//
    public Vector3 DropSpin; //How much a dropped item spins around//
    public bool RandomDropSpin;

    [Header("Main References")]
    [Space(50)]
    public EnemyStats Enemy;
    public EnemyMovement Movement;
    public GameObject DefaultItem;
    private GameObject CurrentDefaultItem;
    private GameManager GameManagerScript;

    //Each number represents a type of item//
    private const int GunItemType = 0;
    private const int MeleeItemType = 1;
    private const int ThrowableItemType = 2;
    private const int PlaceableItemType = 3;
    private const int SingleUseItemType = 4;
    private const int AmmunitionItemType = 5;

    void Awake()
    {
        GameManagerScript = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        Movement = GetComponent<EnemyMovement>();
        Enemy = GetComponent<EnemyStats>();

        if (CurrentDefaultItem != null)
        {
            CurrentDefaultItem = Instantiate(DefaultItem, transform);
            CurrentDefaultItem.transform.position = transform.position;
            CurrentDefaultItem.transform.rotation = transform.rotation;

            AddItem(CurrentDefaultItem.GetComponent<Item>());
        }

        StoreAll();
    }

    void Update()
    {
        if (Enemy.Dead)
        {
            return;
        }
    }

    void StoreAll ()
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            if (InventorySlots [i] != null)
            {
                InventorySlots [i] = Instantiate(InventorySlots [i].gameObject, transform).GetComponent<Item>();
                Debug.Log(InventorySlots [i] + " loaded as the " + i + " slot of an enemy inventory of " + gameObject.name);

                StoreItem(i);
            }
        }
    }

    public void StoreItem(int ItemSlot)
    {
        Item CurrentItem = InventorySlots [ItemSlot];

        if (CurrentItem != null)
        {
            GameObject ItemObject = CurrentItem.gameObject;
            ItemObject.SetActive(false);

            CurrentItem = ItemObject.GetComponent<Item>();
            CurrentItem.UIActive = false;

            Collider ItemCollider = ItemObject.GetComponent<Collider>();
            Rigidbody ItemRigidbody = ItemObject.GetComponent<Rigidbody>();
            Guns ItemGun = CurrentItem.GetComponent<Guns>();
            Melee ItemMelee = CurrentItem.GetComponent<Melee>();
            UsableItem ItemUsable = CurrentItem.GetComponent<UsableItem>();

            if (ItemRigidbody != null)
            {
                ItemRigidbody.useGravity = false;
                ItemRigidbody.velocity = new Vector3(0f, 0f, 0f);
                ItemRigidbody.angularVelocity = new Vector3(0f, 0f, 0f);
            }
            if (ItemCollider != null)
            {
                ItemCollider.enabled = false;
            }

            if (ItemGun != null)
            {
                ItemGun.Unequip();
                ItemGun.enabled = false;
            }
            else if (ItemMelee != null)
            {
                ItemMelee.Unequip();
                ItemMelee.enabled = false;
            }
            else if (ItemUsable != null)
            {
                ItemUsable.Unequip();
                ItemUsable.enabled = false;
            }
        }
    }

    public void AddItem(Item CurrentItem)
    {
        GameObject ItemObject = CurrentItem.gameObject;
        UsableItem Usable = ItemObject.GetComponent<UsableItem>();

        if (CurrentItem != null)
        {
            switch (CurrentItem.ItemType)
            {
                case GunItemType:
                    Guns Gun = ItemObject.GetComponent<Guns>();

                    if (Gun != null)
                    {
                        for (int i = 0; i < InventorySlots.Length; i++)
                        {
                            if (InventorySlots [i] == null)
                            {
                                InventorySlots [i] = CurrentItem;
                                StoreItem(i);

                                Debug.Log(gameObject.name + " obtained " + CurrentItem.name + ";");
                                break;
                            }
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
                        for (int i = 0; i < InventorySlots.Length; i++)
                        {
                            if (InventorySlots [i] == null)
                            {
                                InventorySlots [i] = CurrentItem;
                                StoreItem(i);

                                Debug.Log(gameObject.name + " obtained " + CurrentItem.name + ";");
                                break;
                            }
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
                        for (int i = 0; i < InventorySlots.Length; i++)
                        {
                            if (InventorySlots [i] != null)
                            {
                                if (InventorySlots [i].ID == CurrentItem.ID)
                                {
                                    InventorySlots [i].ItemAmount += CurrentItem.ItemAmount;
                                    Destroy(ItemObject);
                                    StoreItem(i);

                                    Debug.Log(gameObject.name + " obtained " + CurrentItem.name + ";");
                                    break;
                                }
                            }
                            else
                            {
                                InventorySlots [i] = CurrentItem;
                                StoreItem(i);

                                Debug.Log(gameObject.name + " obtained " + CurrentItem.name + ";");
                                break;
                            }
                        }
                    }
                    break;

                case PlaceableItemType:
                    if (Usable != null)
                    {
                        for (int i = 0; i < InventorySlots.Length; i++)
                        {
                            if (InventorySlots [i] != null)
                            {
                                if (InventorySlots [i].ID == CurrentItem.ID)
                                {
                                    InventorySlots [i].ItemAmount += CurrentItem.ItemAmount;
                                    Destroy(ItemObject);
                                    StoreItem(i);

                                    Debug.Log(gameObject.name + " obtained " + CurrentItem.name + ";");
                                    break;
                                }
                            }
                            else
                            {
                                InventorySlots [i] = CurrentItem;
                                StoreItem(i);

                                Debug.Log(gameObject.name + " obtained " + CurrentItem.name + ";");
                                break;
                            }
                        }
                    }
                    break;

                case SingleUseItemType:
                    if (Usable != null)
                    {
                        for (int i = 0; i < InventorySlots.Length; i++)
                        {
                            if (InventorySlots [i] != null)
                            {
                                if (InventorySlots [i].ID == CurrentItem.ID)
                                {
                                    InventorySlots [i].ItemAmount += CurrentItem.ItemAmount;
                                    Destroy(ItemObject);
                                    StoreItem(i);

                                    Debug.Log(gameObject.name + " obtained " + CurrentItem.name + ";");
                                    break;
                                }
                            }
                            else
                            {
                                InventorySlots [i] = CurrentItem;
                                StoreItem(i);

                                Debug.Log(gameObject.name + " obtained " + CurrentItem.name + ";");
                                break;
                            }
                        }
                    }
                    break;

                case AmmunitionItemType:
                    for (int i = 0; i < InventorySlots.Length; i++)
                    {
                        if (InventorySlots [i] != null)
                        {
                            if (InventorySlots [i].ID == CurrentItem.ID)
                            {
                                InventorySlots [i].ItemAmount += CurrentItem.ItemAmount;
                                Destroy(ItemObject);
                                StoreItem(i);

                                Debug.Log(gameObject.name + " obtained " + CurrentItem.name + ";");
                                break;
                            }
                        }
                        else
                        {
                            InventorySlots [i] = CurrentItem;
                            StoreItem(i);

                            Debug.Log(gameObject.name + " obtained " + CurrentItem.name + ";");
                            break;
                        }
                    }
                    break;

                default:
                    Debug.LogError(gameObject.name + "is an invalid item;");
                    Destroy(ItemObject);
                    break;
            }
        }
    }

    public void DropItem(int ItemSlot, Quaternion DropRotation)
    {
        if (InventorySlots[ItemSlot] != null)
        {
            Item CurrentItem = InventorySlots [ItemSlot];
            GameObject ItemObject = CurrentItem.gameObject;

            Debug.Log(gameObject.name + " drops " + ItemObject.name + ";");

            if (ItemObject != null)
            {
                InventorySlots [ItemSlot] = null;

                Collider ItemCollider = ItemObject.GetComponent<Collider>();
                Rigidbody ItemRigidbody = ItemObject.GetComponent<Rigidbody>();

                CurrentItem.UIActive = true;
                ItemObject.SetActive(true);
                ItemObject.transform.parent = null;
                ItemObject.transform.position = transform.position;
                ItemObject.transform.rotation = transform.rotation;

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
            }
        }
    }

    public void DeathDrop()
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            if (InventorySlots [i] != null)
            {
                Vector3 DropRotation;
                DropRotation.x = Random.Range(0f, 360f);
                DropRotation.y = Random.Range(0f, 360f);
                DropRotation.z = Random.Range(0f, 360f);

                DropItem(i, Quaternion.Euler(DropRotation));
            }
        }
    }
}
