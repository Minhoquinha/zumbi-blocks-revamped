using UnityEngine;

public class EnemySpawn : MonoBehaviour 
{
    [Header("Main")]
    public bool Assigned;
    public Transform SpawnPoint;

    [Header("Zombie textures")]
    public Texture [] SkinTextureArray; //List of all the skin textures of the zombie
    public Texture [] ShirtTextureArray; //List of all the shirt textures of the zombie
    public Texture [] PantsTextureArray; //List of all the pants textures of the zombie

    void Awake () 
	{
        SpawnPoint.position = transform.position;
    }

    public void Spawn(Transform Zombie, bool ZombieAIActive)
    {
        SpawnPoint.localPosition = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));

        Transform CurrentZombie = Instantiate(Zombie, SpawnPoint.position, Random.rotation);

        Renderer CurrentZombieRenderer = CurrentZombie.GetComponentInChildren<Renderer>();

        int CurrentShirtTexture = Random.Range(0, ShirtTextureArray.Length);
        if (ShirtTextureArray [CurrentShirtTexture] != null && CurrentZombieRenderer.materials [0].mainTexture != null)
        {
            CurrentZombieRenderer.materials [0].mainTexture = ShirtTextureArray [CurrentShirtTexture];
        }
        int CurrentPantsTexture = Random.Range(0, PantsTextureArray.Length);
        if (PantsTextureArray [CurrentPantsTexture] != null && CurrentZombieRenderer.materials [0].mainTexture != null)
        {
            CurrentZombieRenderer.materials [1].mainTexture = PantsTextureArray [CurrentPantsTexture];
        }
        int CurrentSkinTexture = Random.Range(0, SkinTextureArray.Length);
        if (SkinTextureArray [CurrentSkinTexture] != null && CurrentZombieRenderer.materials [0].mainTexture != null)
        {
            CurrentZombieRenderer.materials [2].mainTexture = SkinTextureArray [CurrentSkinTexture];
        }

        EnemyMovement CurrentZombieMovement = CurrentZombie.GetComponent<EnemyMovement>();
        if (CurrentZombieMovement != null)
        {
            CurrentZombieMovement.AIActive = ZombieAIActive;
        }

        print("Spawned one " + CurrentZombie.name + " at [" + SpawnPoint.position + "]");
    }
}
