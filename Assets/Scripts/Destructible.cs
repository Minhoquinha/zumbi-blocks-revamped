using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [Header("Combat Stats")]
    public float Health;
    [HideInInspector]
    public float CurrentHealth;
    public float Armor;
    public bool BlockPath = true;

    [Header("Passive Stats")]
    public float DestructionTime;
    [HideInInspector]
    public bool Dead;

    [Header("Main References")]
    [Space(50)]
    private GameManager GameManagerScript;
    private NavMeshManager NavMeshManagerScript;
    private ScriptableObjectManager ScriptableObjectManagerScript;
    private PhysicMaterialCollection PhysicMaterialCollectionScript;
    private ImpactEffectCollection ImpactEffectCollectionScript;

    void Awake()
    {
        GameManagerScript = FindObjectOfType<GameManager>();
        NavMeshManagerScript = FindObjectOfType<NavMeshManager>();
        ScriptableObjectManagerScript = GameManagerScript.GetComponent<ScriptableObjectManager>();
        PhysicMaterialCollectionScript = ScriptableObjectManagerScript.PhysicMaterialCollectionScript;
        ImpactEffectCollectionScript = ScriptableObjectManagerScript.ImpactEffectCollectionScript;
    }

    void Start()
    {
        CurrentHealth = Health;

        if (BlockPath)
        {
            NavMeshManagerScript.BakeNavMesh();
        }
    }

    public void Hurt(float Damage, float Penetration)
    {
        if (Dead)
        {
            return;
        }

        float ArmorResistance = Mathf.Max(Armor / Penetration, 1f);

        CurrentHealth -= (Damage / ArmorResistance);

        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            Death();
        }
    }

    public void Death()
    {
        Dead = true;
        Debug.Log(this.name + " died;");

        GameObject DestructionEffect = Instantiate(ImpactEffectCollectionScript.ImpactEffectArray [ImpactEffectCollection.ObjectDestructionIndex], transform.position, Quaternion.Euler(transform.up));

        if (BlockPath)
        {
            gameObject.SetActive(false);
            NavMeshManagerScript.BakeNavMesh();
        }

        Destroy(DestructionEffect, 2f);
        Destroy(gameObject, DestructionTime);
    }
}
