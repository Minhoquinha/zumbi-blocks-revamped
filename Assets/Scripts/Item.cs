using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Item : MonoBehaviour
{
    public GameObject ItemPrefab;
    public string ItemName;
    public int ID;
    public int ItemAmount = 1; //Amount of items in this element//
    public int ItemType; //Each number represent the type of item this is//
    private const int GunItemType = 0;
    private const int MeleeItemType = 1;
    private const int ThrowableItemType = 2;
    private const int PlaceableItemType = 3;
    private const int SingleUseItemType = 4;
    private const int AmmunitionItemType = 5;

    public Image ItemUIPrefab;
    private Image [] ItemUIArray = new Image [99];
    private Text [] ItemTextArray = new Text [99];
    [HideInInspector]
    public bool UIActive;
    public float UIRenderDistance = 50f;
    public AudioClip PickUpSoundClip;
    private PlayerManager PlayerManagerScript;
    private Canvas InGameCanvas;

    void Start()
    {
        PlayerManagerScript = FindObjectOfType<PlayerManager>();
        InGameCanvas = FindObjectOfType<Canvas>();
        UIActive = true;

        if (ItemUIPrefab != null)
        {
            StartUI();
        }
    }

    void Update()
    {
        if (ItemUIPrefab != null)
        {
            UpdateUI();
        }
    }

    void StartUI ()
    {
        int i = 0;

        foreach (Transform Player in PlayerManagerScript.PlayerList)
        {
            Camera PlayerCamera = Player.GetComponentInChildren<Camera>();

            if (PlayerCamera != null)
            {
                ItemUIArray [i] = Instantiate(ItemUIPrefab, PlayerCamera.transform.position, InGameCanvas.transform.rotation, InGameCanvas.transform);
                ItemTextArray [i] = ItemUIArray [i].GetComponentInChildren<Text>();
                ItemTextArray [i].text = ItemName;
                i++;
            }
        }
    }

    public void UpdateUI ()
    {
        int i = 0;

        foreach (Transform Player in PlayerManagerScript.PlayerList)
        {
            if (Player != null && UIActive)
            {
                Camera PlayerCamera = Player.GetComponentInChildren<Camera>();

                if (PlayerCamera != null)
                {
                    if (ItemTextArray [i] != null)
                    {
                        float MinWidth = ItemUIArray [i].GetPixelAdjustedRect().width / 2f;
                        float MaxWidth = Screen.width - MinWidth;
                        float MinHeight = ItemUIArray [i].GetPixelAdjustedRect().height / 2f;
                        float MaxHeight = Screen.height - MinHeight;

                        Vector2 ItemUIPosition = PlayerCamera.WorldToScreenPoint(transform.position);

                        if (Vector3.Dot((transform.position - PlayerCamera.transform.position), PlayerCamera.transform.forward) < 0)
                        {
                            ItemUIArray [i].enabled = false;
                            ItemTextArray [i].enabled = false;
                        }
                        else 
                        {
                            ItemUIArray [i].enabled = true;
                            ItemTextArray [i].enabled = true;
                        }

                        if (Vector3.Distance(transform.position, Player.position) > UIRenderDistance)
                        {
                            ItemUIArray [i].enabled = false;
                            ItemTextArray [i].enabled = false;
                        }

                        ItemUIArray [i].transform.position = ItemUIPosition;
                    }
                    else
                    {
                        ItemUIArray [i] = Instantiate(ItemUIPrefab, PlayerCamera.transform.position, InGameCanvas.transform.rotation, InGameCanvas.transform);
                        ItemTextArray [i] = ItemUIArray [i].GetComponentInChildren<Text>();
                        ItemTextArray [i].text = ItemName;
                    }

                    i++;
                }
            }
            else
            {
                ItemUIArray [i].enabled = false;
                ItemTextArray [i].enabled = false;
            }
        }
    }

    void OnDestroy()
    {
        UIActive = false;
        foreach (Image CurrentItemUI in ItemUIArray)
        {
            if (CurrentItemUI != null)
            {
                Destroy(CurrentItemUI.gameObject);
            }
        }
    }

    public void CheckItemType ()
    {
        Guns GunScript = GetComponent<Guns>();
        Melee MeleeScript = GetComponent<Melee>();
        UsableItem UsableItemScript = GetComponent<UsableItem>();
        int ScriptNum = 0; //How many of the scripts above have returned true, it should be either 0 or 1//

        if (GunScript != null)
        {
            ScriptNum++;
            ItemType = GunItemType;
        }

        if (MeleeScript != null)
        {
            ScriptNum++;
            ItemType = MeleeItemType;
        }

        if (UsableItemScript != null)
        {
            ScriptNum++;
            ItemType = UsableItemScript.ItemType;
        }

        if (ScriptNum != 0 && ScriptNum != 1)
        {
            Debug.LogError(this.name + " is of multiple item types;");
        }
    }
}
