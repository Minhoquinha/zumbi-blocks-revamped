using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour 
{
    [Header("Cycle Properties")]
    public int CycleNum = 0;
    public float CycleDelay = 30f; //Time between each cycle//
    private float NextCycleDelay = 0f; //Time between the current cycle and the next cycle//
    public float CycleHealthIncrease = 0f; //Increase in zombie's health per cycle//
    public float CycleAttackDamageIncrease = 0f; //Increase in zombie's attack damage per cycle//

    [Header("Wave Properties")]
    public bool SpawnWaves;
    public string WaveIntroText; //Text that shows when a wave is introducted//
    public float WaveDelay = 5f; //Time between each wave//
    private float NextWaveDelay = 0f; //Time between the current wave and the next wave//
    private int NextWaveNum = 1;
    public bool AllZombiesActive; //Sets all waves with active zombies//
    public ZombieType [] ZombieArray;
    private ZombieType DefaultZombie;
    public int ZombieAmount = 0; //Amount of zombies that spawn per wave//
    public float ZombieKillPercent; //The percentage of zombies that have to be killed in a wave for the next wave to spawn//
    [HideInInspector]
    public int ZombieDeathCount; //The number of zombies killed in the current wave//
    public enum SpawnerState
	{
		Spawning, Waiting, CountingDown
	}
    private SpawnerState SpawnerStatus;

    [Header("Debugging")]
    public bool PrintWaveInfo;

    [Header("Main References")]
    [Space (50)]
    public bool SpawnerFinder;
    public EnemySpawn [] SpawnerArray;
    private GameManager GameManagerScript;
    public UIManager UI;
    private ScriptableObjectManager ScriptableObjectManagerScript;
    private ZombieTextureCollection ZombieTextureCollectionScript;

    void Awake()
    {
        GameManagerScript = GetComponent<GameManager>();
        UI = GetComponent<UIManager>();
        ScriptableObjectManagerScript = GetComponent<ScriptableObjectManager>();
        ZombieTextureCollectionScript = ScriptableObjectManagerScript.ZombieTextureCollectionScript;

        if (ZombieArray.Length <= 0 && enabled)
        {
            this.enabled = false;
        }

        if (SpawnerArray.Length <= 0 && enabled)
        {
            Debug.LogError("SpawnerArray has 0 elements;");
        }
        else if (SpawnerFinder)
        {
            SpawnerArray = FindObjectsOfType<EnemySpawn>();
        }

        foreach (EnemySpawn Spawner in SpawnerArray)
        {
            Spawner.ZombieTextureCollectionScript = ZombieTextureCollectionScript;
        }
    }

    void Start () 
	{
        NextWaveNum = 1;
        NextWaveDelay = WaveDelay;
        SpawnerStatus = SpawnerState.CountingDown;

        DefaultZombie = null;
        for (int i = 0; i < ZombieArray.Length; i++)
        {
            if (ZombieArray [i] != null && ZombieArray [i].Default)
            {
                if (DefaultZombie == null)
                {
                    DefaultZombie = ZombieArray [i];
                }
                else
                {
                    Debug.LogWarning("More than one DefaultZombie, selecting first option by array order;");
                    ZombieArray [i].Default = false;
                    break;
                }
            }
        }
    }
	
	void Update () 
	{
        if (GameManagerScript.GameStatus != GameManager.GameState.InGame)
        {
            return;
        }

        if (SpawnWaves)
        {
            WaveUpdate();
        }
    }

    void WaveUpdate ()
    {
        if (SpawnerStatus == SpawnerState.Waiting)
        {
            if (WaveFinished())
            {
                WaveComplete();
            }
            else
            {
                return;
            }
        }

        if (NextWaveDelay <= 0f)
        {
            if (SpawnerStatus != SpawnerState.Spawning)
            {
                SpawnWave();
            }
        }
        else
        {
            NextWaveDelay -= Time.deltaTime;
        }
    }

    void WaveComplete ()
    {
        Debug.Log("WAVE: " + NextWaveNum);
        SpawnerStatus = SpawnerState.CountingDown;
        NextWaveDelay = WaveDelay;
        ZombieDeathCount = 0;
        NextWaveNum++;
    }

    bool WaveFinished ()
    {
        float ZombieKillFactor = ZombieKillPercent / 100f;

        if (ZombieDeathCount >= ZombieAmount * ZombieKillFactor)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void SpawnWave ()
    {
        SpawnerStatus = SpawnerState.Spawning;
        UI.PCenterTextMessage(WaveIntroText + NextWaveNum, 5f);

        int TotalSpawnedZombies = 0;
        int[] SpawnedZombiesPerType = new int [ZombieArray.Length];
        for (int i = 0; i < SpawnedZombiesPerType.Length; i++)
        {
            SpawnedZombiesPerType [i] = 0;
        }
        int SpawnedDefaultZombies = 0;

        for (int ZombieNum = 0; ZombieNum < ZombieAmount; ZombieNum++)
        {
            Transform CurrentZombie = DefaultZombie.ZombiePrefab;

            for (int i = 0; i < ZombieArray.Length; i++)
            {
                if (ZombieArray [i] != null && ZombieArray[i].Active)
                {
                    if (ZombieArray [i].Chance >= Random.Range(0f, 100f))
                    {
                        CurrentZombie = ZombieArray [i].ZombiePrefab;
                        SpawnedZombiesPerType [i]++;
                        SpawnedDefaultZombies--;
                        break;
                    }
                }
            }

            int RandomSpawner = Random.Range(0, SpawnerArray.Length);

            if (SpawnerArray[RandomSpawner] != null)
            {
                SpawnerArray [RandomSpawner].Spawn(CurrentZombie, AllZombiesActive);
                TotalSpawnedZombies++;
                SpawnedDefaultZombies++;
            }
        }

        SpawnerStatus = SpawnerState.Waiting;

        Debug.Log("WAVE: " + NextWaveNum);

        if (PrintWaveInfo)
        {
            Debug.Log("WAVE INFO:");
            Debug.Log("WAVE NUMBER: " + NextWaveNum);
            Debug.Log("TOTAL ZOMBIE COUNT: " + TotalSpawnedZombies);
            Debug.Log("DEFAULT ZOMBIE COUNT: " + SpawnedDefaultZombies);
            Debug.Log("ZOMBIE TYPES INFO:");
            for (int i = 0; i < SpawnedZombiesPerType.Length; i++)
            {
                string ZombieTypeName = ZombieArray [i].ZombiePrefab.name;
                Debug.Log("ELEMENT NUMBER: " + i);
                Debug.Log("TYPE: " + ZombieTypeName);
                Debug.Log(ZombieTypeName + ": IS ACTIVE: " + ZombieArray [i].Active);
                Debug.Log(ZombieTypeName + ": IS DEFAULT: " + ZombieArray [i].Default);
                Debug.Log(ZombieTypeName + ": COUNT EXCEPT DEFAULT: " + SpawnedZombiesPerType [i]);
            }
        }
    }
}
