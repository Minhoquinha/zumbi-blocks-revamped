using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTextureZombieSpawn : MonoBehaviour
{
    public GameObject ZombieOriginal; //Prefab of the zombie
    public Texture[] SkinTexture; //List of all the skin textures of the zombie
    public Texture[] ShirtTexture; //List of all the shirt textures of the zombie
    public Texture[] PantsTexture; //List of all the pants textures of the zombie
    public Transform SpawnPoint;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) //Spawn zombies with the space key
        {
            SpawnZombie();
        }
    }

    public void SpawnZombie() //Spawns zombies with random textures
    {
        GameObject ZombieClone = Instantiate(ZombieOriginal, SpawnPoint.transform.position , transform.rotation); //This part makes a clone of the original prefab at the SpawnPoint position
        Renderer ZombieRenderer = ZombieClone.GetComponentInChildren<Renderer>(); //This part accesses the Renderer of the clone to be able to change the textures

        int randomSkinTexture = Random.Range(0, SkinTexture.Length); //Choose randomly one of the textures from the list
        ZombieRenderer.materials[2].mainTexture = SkinTexture[randomSkinTexture]; //Set the texture to the zombie

        int randomShirtTexture = Random.Range(0, ShirtTexture.Length); //Choose randomly one of the textures from the list
        ZombieRenderer.materials[0].mainTexture = ShirtTexture[randomShirtTexture]; //Set the texture to the zombie

        int randomPantsTexture = Random.Range(0, PantsTexture.Length); //Choose randomly one of the textures from the list
        ZombieRenderer.materials[1].mainTexture = PantsTexture[randomPantsTexture]; //Set the texture to the zombie
    }
}
