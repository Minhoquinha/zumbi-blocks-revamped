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

    [Header("Passive Stats")]
    public float DestructionTime;
    [HideInInspector]
    public bool Dead;

    [Header("Main References")]
    [Space(50)]
    public ImpactEffectCollection ImpactEffectCollectionScript;
    private GameManager GameManagerScript;
    private int ObjectDestructionIndex;

    void Awake()
    {
        GameManagerScript = FindObjectOfType<GameManager>();

        ObjectDestructionIndex = ImpactEffectCollectionScript.ObjectDestructionIndex;
    }

    void Start()
    {
        CurrentHealth = Health;
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

        GameObject DestructionEffect = Instantiate(ImpactEffectCollectionScript.ImpactEffectArray [ObjectDestructionIndex], transform.position, Quaternion.Euler(transform.up));

        Destroy(DestructionEffect, 2f);
        Destroy(gameObject, DestructionTime);
    }
}
