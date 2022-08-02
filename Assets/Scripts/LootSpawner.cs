using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [Header("Main References")]
    [Space(50)]
    public GameManager GameManagerScript;
    public WaveSpawner EnemyWaveSpawner;
    public Loot [] LootArray;
    public LootSpawn [] SpawnerArray;

    [Header("Properties")]
    public bool SpawnLoots;

    void Start()
    {
        GameManagerScript = GetComponent<GameManager>();
        EnemyWaveSpawner = GetComponent<WaveSpawner>();

        if (SpawnerArray.Length <= 0 && enabled)
        {
            Debug.LogError("Spawner Array has 0 elements;");
        }

        for (int SpawnerNum = 0; SpawnerNum < EnemyWaveSpawner.SpawnerArray.Length; SpawnerNum++)
        {
            SpawnerArray [SpawnerNum] = EnemyWaveSpawner.SpawnerArray [SpawnerNum].GetComponent<LootSpawn>();
        }
    }

    void Update()
    {
        if (GameManagerScript.GameStatus != GameManager.GameState.InGame)
        {
            return;
        }

        if (SpawnLoots)
        {
            for (int i = 0; i < LootArray.Length; i++)
            {
                if (LootArray [i].CurrentWaitTime <= 0f)
                {
                    LootArray [i].CurrentWaitTime = LootArray [i].WaitTime;

                    if (LootArray [i].Chance >= Random.Range(0f, 100f))
                    {
                        SpawnLoot(LootArray [i].LootPrefab);
                    }
                }
                else
                {
                    LootArray [i].CurrentWaitTime -= Time.deltaTime;
                }
            }
        }
    }

    void SpawnLoot(Transform Loot)
    {
        int RandomSpawner = Random.Range(0, SpawnerArray.Length);

        if (SpawnerArray[RandomSpawner] != null && Loot != null)
        {
            SpawnerArray [RandomSpawner].Spawn(Loot);
        }
    }
}
