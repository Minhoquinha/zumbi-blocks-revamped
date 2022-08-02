using System.Collections;
using UnityEngine;

public class WaveSpawnerOld : MonoBehaviour
{
    [Header("Zombie textures")]
    private Texture [] SkinTextureArray; //List of all the skin textures of the zombie//
    private Texture [] ShirtTextureArray; //List of all the shirt textures of the zombie//
    private Texture [] PantsTextureArray; //List of all the pants textures of the zombie//

    [Header("Cycle Properties")]
    public int CycleNum = 0;
    public float CycleDelay = 30f; //Time between each cycle//
    private float NextCycleDelay = 0f; //Time between the current cycle and the next cycle//
    public float CycleHealthIncrease = 0f; //Increase in zombie's health per cycle//
    public float CycleAttackDamageIncrease = 0f; //Increase in zombie's attack damage per cycle//

    [Header("Wave Properties")]
    public bool SpawnWaves;
    public float WaveDelay = 5f; //Time between each wave//
    private float NextWaveDelay = 0f; //Time between the current wave and the next wave//
    private int NextWaveNum = 0;
    public float ZombieKillPercent; //The percentage of zombies that have to be killed in a wave for the next wave to spawn//
    public string WaveIntroText; //Text that shows when a wave is introducted//
    [HideInInspector]
    public int ZombieDeathCount; //The number of zombies killed in the current wave//
    public bool AllZombiesActive; //Sets all waves with active zombies//

    public enum SpawnerState
    {
        Spawning, Waiting, CountingDown
    }
    private SpawnerState SpawnerStatus;

    [Header("Main References")]
    [Space(50)]
    public Wave [] WaveArray;
    public bool SpawnerFinder;
    public EnemySpawn [] SpawnerArray;
    private GameManager GameManagerScript;
    public UIManager UI;
    private ZombieTextureCollection ZombieTextureCollectionScript;

    void Awake()
    {
        GameManagerScript = GetComponent<GameManager>();
        UI = GetComponent<UIManager>();
        ZombieTextureCollectionScript = GameManagerScript.ZombieTextureCollectionScript;
        SkinTextureArray = ZombieTextureCollectionScript.SkinTextureArray;
        ShirtTextureArray = ZombieTextureCollectionScript.ShirtTextureArray;
        PantsTextureArray = ZombieTextureCollectionScript.PantsTextureArray;

        if (WaveArray.Length <= 0 && enabled)
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
            if (Spawner != null)
            {
                Spawner.SkinTextureArray = SkinTextureArray;
                Spawner.ShirtTextureArray = ShirtTextureArray;
                Spawner.PantsTextureArray = PantsTextureArray;
            }
        }
    }

    void Start()
    {
        NextCycleDelay = 0f;
        NextWaveDelay = WaveDelay;
        SpawnerStatus = SpawnerState.CountingDown;
    }

    void Update()
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

    void WaveUpdate()
    {
        if (SpawnerStatus == SpawnerState.Waiting)
        {
            if (WaveFinished(WaveArray [NextWaveNum]))
            {
                WaveComplete();
            }
            else
            {
                return;
            }
        }

        if (NextWaveDelay <= 0f && NextCycleDelay <= 0f)
        {
            if (SpawnerStatus != SpawnerState.Spawning)
            {
                SpawnWave(WaveArray [NextWaveNum]);
            }
        }
        else
        {
            NextWaveDelay -= Time.deltaTime;
            NextCycleDelay -= Time.deltaTime;
        }
    }

    void WaveComplete()
    {
        Debug.Log("Wave complete;");
        SpawnerStatus = SpawnerState.CountingDown;
        NextWaveDelay = WaveDelay;
        ZombieDeathCount = 0;

        if (NextWaveNum + 1 >= WaveArray.Length)
        {
            CycleComplete();
        }
        else
        {
            NextWaveNum++;
        }
    }

    void CycleComplete()
    {
        CycleNum++;
        NextCycleDelay = CycleDelay;

        NextWaveNum = 0;

        Debug.Log(CycleNum + " cycle(s) complete;");
        Debug.Log("Starting new cycle;");
    }

    bool WaveFinished(Wave CurrentWave)
    {
        float ZombieKillFactor = ZombieKillPercent / 100f;

        if (ZombieDeathCount >= CurrentWave.ZombieAmount * ZombieKillFactor)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void SpawnWave(Wave CurrentWave)
    {
        SpawnerStatus = SpawnerState.Spawning;
        UI.PCenterTextMessage(WaveIntroText + (NextWaveNum + 1), 5f);

        Transform CurrentZombie = CurrentWave.ZombiePrefab;
        EnemyStats CurrentZombieStats = CurrentZombie.GetComponent<EnemyStats>();
        CurrentZombieStats.Health += (CycleHealthIncrease * CycleNum);
        CurrentZombieStats.AttackDamage += (CycleAttackDamageIncrease * CycleNum);

        for (int ZombieNum = 0; ZombieNum < CurrentWave.ZombieAmount; ZombieNum++)
        {
            int RandomSpawner = Random.Range(0, SpawnerArray.Length);

            if (AllZombiesActive)
            {
                CurrentWave.ZombieAIActive = true;
            }

            if (SpawnerArray [RandomSpawner] != null)
            {
                SpawnerArray [RandomSpawner].Spawn(CurrentZombie, CurrentWave.ZombieAIActive);
            }
        }

        SpawnerStatus = SpawnerState.Waiting;

        print("Spawned wave " + NextWaveNum + ";");
    }
}
