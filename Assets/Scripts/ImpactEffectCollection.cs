using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ImpactEffectCollection : ScriptableObject
{
    [Header("Main")]
    public GameObject [] ImpactEffectArray;
    public string Description;

    [Header ("Impact Effects' Indexes")]
    public const int FleshEffectIndex = 0;
    public const int MetalEffectIndex = 1;
    public const int ConcreteEffectIndex = 2;
    public const int WoodEffectIndex = 3;
    public const int PlasticEffectIndex = 4;
    public const int BloodExplosionIndex = 5;
    public const int BloodEruptionIndex = 6;
    public const int ObjectDestructionIndex = 7;

    public const int DebugEffectIndex = 10;

    public GameObject this [int index]
    {
        get
        {
            return ImpactEffectArray [index];
        }
        set
        {
            ImpactEffectArray [index] = value;
        }
    }

    public GameObject PickRandom()
    {
        return ImpactEffectArray [Random.Range(0, ImpactEffectArray.Length)];
    }

    public int Length
    {
        get
        {
            if (ImpactEffectArray == null)
            {
                return 0;
            }
            return ImpactEffectArray.Length;
        }
    }

    public int ConstrainIndex(int index, bool wrap = false)
    {
        if (ImpactEffectArray.Length < 1)
        {
            throw new System.IndexOutOfRangeException("ImpactEffectCollection.ConstrainIndex(): no items in " + name);
        }

        if (index < 0) return 0;
        if (index >= ImpactEffectArray.Length)
        {
            if (wrap)
            {
                index = 0;
            }
            else
            {
                index = ImpactEffectArray.Length - 1;
            }
        }
        return index;
    }
}
