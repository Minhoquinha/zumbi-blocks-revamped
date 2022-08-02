using System;
using UnityEngine;

[Serializable]
public class Loot
{
    public Transform LootPrefab;
    public bool Active;
    public float Chance;
    public float WaitTime;
    [HideInInspector]
    public float CurrentWaitTime;
}
