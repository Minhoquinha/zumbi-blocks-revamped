using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour 
{
    [Header("Main")]
    public static PlayerManager Instance;
    public string PlayerDeathText; //Text that shows when a player dies//

    [Header("Default Loadout")]
    public bool GiveLoadout = true;
    public Item [] InventorySlots;

    [Header("Main References")]
    [Space(50)]
    public bool SpawnerFinder = true;
    public PlayerSpawn [] SpawnerArray = new PlayerSpawn [8];
    public GameManager GameManagerScript;
    public UIManager UI;
    public List<Transform> PlayerList = new List<Transform>();
    private int DeadPlayers = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("More than one " + this.name + " loaded;");
            return;
        }

        GameManagerScript = GetComponent<GameManager>();
        UI = GetComponent<UIManager>();

        if (SpawnerArray.Length <= 0 && enabled)
        {
            Debug.LogError("SpawnerArray has 0 elements;");
        }
        else if (SpawnerFinder)
        {
            SpawnerArray = FindObjectsOfType<PlayerSpawn>();
        }
    }

    public void PlayerSpawn (Transform Player)
    {
        int RandomSpawner = Random.Range(0, SpawnerArray.Length);

        if (SpawnerArray [RandomSpawner] != null)
        {
            Transform CurrentPlayer = SpawnerArray [RandomSpawner].Spawn(Player);

            if (GiveLoadout)
            {
                foreach (Item CurrentItem in InventorySlots)
                {
                    StartCoroutine(GiveLoadoutItem(CurrentPlayer, SpawnLoadoutItem(CurrentItem)));
                }
            }

            Debug.Log("Player " + CurrentPlayer.name + " spawned;");
        }
        else
        {
            Debug.LogError("Null Spawner used to spawn Player " + Player.name + ";");
        }
    }

    public Item SpawnLoadoutItem (Item CurrentItem)
    {
        if (CurrentItem != null)
        {
            Item CurrentItemInstance = Instantiate(CurrentItem, transform);
            return CurrentItemInstance;
        }
        else
        {
            return null;
        }
    }

    public IEnumerator GiveLoadoutItem (Transform CurrentPlayer, Item CurrentItemInstance)
    {
        yield return new WaitForSeconds(0.4f);

        if (CurrentItemInstance != null)
        {
            Transform CurrentPlayerCamera = CurrentPlayer.GetComponentInChildren<Camera>().transform;
            PlayerInventory PlayerInventoryScript = CurrentPlayer.GetComponent<PlayerInventory>();
            CurrentItemInstance.transform.position = CurrentPlayerCamera.position;
            CurrentItemInstance.transform.rotation = CurrentPlayerCamera.rotation;

            PlayerInventoryScript.PickUpItem(CurrentItemInstance);
        }
    }

    public void PlayerDeath (Transform Player)
    {
        if (GameManagerScript.GameStatus == GameManager.GameState.InGame)
        {
            string PlayerCappedName = Player.name.ToUpper();

            UI.PCenterTextMessage (PlayerCappedName + PlayerDeathText, 5f);
            Debug.Log(PlayerCappedName + PlayerDeathText);
        }

        DeadPlayers++;

        if (DeadPlayers >= PlayerList.Count)
        {
            GameManagerScript.GameOver();
            Debug.Log("All players died;");
        }
    }
}
