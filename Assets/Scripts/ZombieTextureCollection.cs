using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ZombieTextureCollection : ScriptableObject
{
    [Header("Main")]
    public ZombieTextures [] ZombieTexturesArray;
    public string Description;

    public ZombieTextures this [int index]
    {
        get
        {
            return ZombieTexturesArray [index];
        }
        set
        {
            ZombieTexturesArray [index] = value;
        }
    }

    public ZombieTextures PickRandom()
    {
        return ZombieTexturesArray [Random.Range(0, ZombieTexturesArray.Length)];
    }

    public int Length
    {
        get
        {
            if (ZombieTexturesArray == null)
            {
                return 0;
            }

            return ZombieTexturesArray.Length;
        }
    }

    public int ConstrainIndex(int index, bool wrap = false)
    {
        if (ZombieTexturesArray.Length < 1)
        {
            throw new System.IndexOutOfRangeException("ZombieTextureCollection.ConstrainIndex(): no items in " + name);
        }

        if (index < 0) return 0;
        if (index >= ZombieTexturesArray.Length)
        {
            if (wrap)
            {
                index = 0;
            }
            else
            {
                index = ZombieTexturesArray.Length - 1;
            }
        }
        return index;
    }
}
