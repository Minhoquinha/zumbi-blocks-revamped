using UnityEngine;

public class LootSpawn : MonoBehaviour 
{
	public Transform SpawnPoint;

	void Awake () 
	{
		SpawnPoint.position = transform.position;
	}

	public void Spawn (Transform Loot) 
	{
		SpawnPoint.localPosition = new Vector3 (Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
		Quaternion Rotation = Quaternion.Euler (0f, 0f, Random.Range (0f, 360f));

		Instantiate(Loot, SpawnPoint.position, Rotation);
		Debug.Log("Spawned one " + Loot.name + " at " + SpawnPoint.position);
	}
}
