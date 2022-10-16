using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PhysicMaterialCollection : ScriptableObject
{
    [Header("Main")]
    public PhysicMaterial [] PhysicMaterialArray;
    public string Description;

    [Header("Physic Materials' Indexes")]
    public const int PlayerMaterialIndex = 0;
    public const int EnemyMaterialIndex = 1;
    public const int ItemMaterialIndex = 2;
    public const int GrenadeMaterialIndex = 3;
    public const int StickyMaterialIndex = 4;
    public const int MetalMaterialIndex = 5;
    public const int ConcreteMaterialIndex = 6;
    public const int WoodMaterialIndex = 7;
    public const int PlasticMaterialIndex = 8;
    public const int FabricMaterialIndex = 9;

    public const int DebugMaterialIndex = 10;

    public PhysicMaterial this [int index]
    {
        get
        {
            return PhysicMaterialArray [index];
        }
        set
        {
            PhysicMaterialArray [index] = value;
        }
    }

    public PhysicMaterial PickRandom()
    {
        return PhysicMaterialArray [Random.Range(0, PhysicMaterialArray.Length)];
    }

    public int Length
    {
        get
        {
            if (PhysicMaterialArray == null)
            {
                return 0;
            }
            return PhysicMaterialArray.Length;
        }
    }

    public int ConstrainIndex(int index, bool wrap = false)
    {
        if (PhysicMaterialArray.Length < 1)
        {
            throw new System.IndexOutOfRangeException("PhysicsMaterialCollection.ConstrainIndex(): no items in " + name);
        }

        if (index < 0) return 0;
        if (index >= PhysicMaterialArray.Length)
        {
            if (wrap)
            {
                index = 0;
            }
            else
            {
                index = PhysicMaterialArray.Length - 1;
            }
        }
        return index;
    }

    public int SetImpact(PhysicMaterial HitObjectMaterial)
    {
        int MaterialIndex = PhysicMaterialArray.Length;
        int ImpactIndex;

        for (int i = 0; i < PhysicMaterialArray.Length; i++)
        {
            if (HitObjectMaterial == PhysicMaterialArray[i])
            {
                MaterialIndex = i;
                break;
            }
        }

        switch (MaterialIndex)
        {
            case PlayerMaterialIndex:
                ImpactIndex = ImpactEffectCollection.FleshEffectIndex;
                break;

            case EnemyMaterialIndex:
                ImpactIndex = ImpactEffectCollection.FleshEffectIndex;
                break;

            case ItemMaterialIndex:
                ImpactIndex = ImpactEffectCollection.MetalEffectIndex;
                break;

            case GrenadeMaterialIndex:
                ImpactIndex = ImpactEffectCollection.MetalEffectIndex;
                break;

            case StickyMaterialIndex:
                ImpactIndex = ImpactEffectCollection.PlasticEffectIndex;
                break;

            case MetalMaterialIndex:
                ImpactIndex = ImpactEffectCollection.MetalEffectIndex;
                break;

            case ConcreteMaterialIndex:
                ImpactIndex = ImpactEffectCollection.ConcreteEffectIndex;
                break;

            case WoodMaterialIndex:
                ImpactIndex = ImpactEffectCollection.WoodEffectIndex;
                break;

            case PlasticMaterialIndex:
                ImpactIndex = ImpactEffectCollection.PlasticEffectIndex;
                break;

            case FabricMaterialIndex:
                ImpactIndex = ImpactEffectCollection.PlasticEffectIndex;
                break;

            case DebugMaterialIndex:
                ImpactIndex = ImpactEffectCollection.DebugEffectIndex;
                break;

            default:
                ImpactIndex = ImpactEffectCollection.ConcreteEffectIndex;
                break;
        }

        return ImpactIndex;
    }
}
