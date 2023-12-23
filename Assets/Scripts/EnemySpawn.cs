using UnityEngine;

public class EnemySpawn : MonoBehaviour 
{
    [Header("Main")]
    public bool Assigned;
    public Transform SpawnPoint;
    [HideInInspector]
    public ZombieTextureCollection ZombieTextureCollectionScript;
    private ZombieTextures ZombieTexturesScript;

    void Awake () 
	{
        SpawnPoint.position = transform.position;
    }

    public void Spawn(Transform Zombie, bool ZombieAIActive)
    {
        SpawnPoint.localPosition = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));

        Transform CurrentZombie = Instantiate(Zombie, SpawnPoint.position, Random.rotation);
        Renderer CurrentZombieRenderer = CurrentZombie.GetComponentInChildren<Renderer>();
        string CurrentZombieName = CurrentZombie.name;

        foreach (ZombieTextures CurrentZombieTextures in ZombieTextureCollectionScript.ZombieTexturesArray)
        {
            if (CurrentZombieName.Contains(CurrentZombieTextures.Name))
            {
                ZombieTexturesScript = CurrentZombieTextures;
                break;
            }
        }

        if (ZombieTexturesScript != null)
        {
            Texture [] CurrentPrimaryTextureArray = ZombieTexturesScript.PrimaryTextureArray;
            Texture [] CurrentSecondaryTextureArray = ZombieTexturesScript.SecondaryTextureArray;
            Texture [] CurrentTertiaryTextureArray = ZombieTexturesScript.TertiaryTextureArray;
            Texture [] CurrentQuaternaryTextureArray = ZombieTexturesScript.QuaternaryTextureArray;

            if (CurrentPrimaryTextureArray != null && CurrentPrimaryTextureArray.Length > 0)
            {
                int CurrentPrimaryTexture = Random.Range(0, CurrentPrimaryTextureArray.Length);
                if (CurrentPrimaryTextureArray [CurrentPrimaryTexture] != null && CurrentZombieRenderer.materials [0] != null)
                {
                    CurrentZombieRenderer.materials [0].mainTexture = CurrentPrimaryTextureArray [CurrentPrimaryTexture];
                }
            }

            if (CurrentSecondaryTextureArray != null && CurrentSecondaryTextureArray.Length > 0)
            {
                int CurrentSecondaryTexture = Random.Range(0, CurrentSecondaryTextureArray.Length);
                if (CurrentSecondaryTextureArray [CurrentSecondaryTexture] != null && CurrentZombieRenderer.materials [1] != null)
                {
                    CurrentZombieRenderer.materials [1].mainTexture = CurrentSecondaryTextureArray [CurrentSecondaryTexture];
                }
            }

            if (CurrentTertiaryTextureArray != null && CurrentTertiaryTextureArray.Length > 0)
            {
                int CurrentTertiaryTexture = Random.Range(0, CurrentTertiaryTextureArray.Length);
                if (CurrentTertiaryTextureArray [CurrentTertiaryTexture] != null && CurrentZombieRenderer.materials [2] != null)
                {
                    CurrentZombieRenderer.materials [2].mainTexture = CurrentTertiaryTextureArray [CurrentTertiaryTexture];
                }
            }

            if (CurrentQuaternaryTextureArray != null && CurrentQuaternaryTextureArray.Length > 0)
            {
                int CurrentQuaternaryTexture = Random.Range(0, CurrentQuaternaryTextureArray.Length);
                if (CurrentQuaternaryTextureArray [CurrentQuaternaryTexture] != null && CurrentZombieRenderer.materials [3] != null)
                {
                    CurrentZombieRenderer.materials [3].mainTexture = CurrentQuaternaryTextureArray [CurrentQuaternaryTexture];
                }
            }
        }
        else
        {
            Debug.LogError(CurrentZombieName + " at [" + SpawnPoint.position + "] spawns with no texture;");
        }

        EnemyMovement CurrentZombieMovement = CurrentZombie.GetComponent<EnemyMovement>();
        if (CurrentZombieMovement != null)
        {
            CurrentZombieMovement.AIActive = ZombieAIActive;
        }

        Debug.Log("Spawned " + CurrentZombieName + " at [" + SpawnPoint.position + "]");
    }
}
