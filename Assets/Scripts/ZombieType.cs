using UnityEngine;

[System.Serializable]
public class ZombieType
{
    public Transform ZombiePrefab;
    public bool Default; //Select this as the default zombie type. The default zombie spawns in case no other zombie gets the chance to spawn.//
    public bool Active;
    public float Chance;
}
