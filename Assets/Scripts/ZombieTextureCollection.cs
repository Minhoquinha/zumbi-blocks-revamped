using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ZombieTextureCollection : ScriptableObject
{
    [Header("Main")]
    public string Description;

    [Header("Zombie textures")]
    public Texture [] SkinTextureArray; //List of all the skin textures of the zombie
    public Texture [] ShirtTextureArray; //List of all the shirt textures of the zombie
    public Texture [] PantsTextureArray; //List of all the pants textures of the zombie

    public Texture this [int index, int ArrayNum]
    {
        get
        {
            switch (ArrayNum)
            {
                case 0:
                    return SkinTextureArray [index];

                case 1:
                    return ShirtTextureArray [index];

                case 2:
                    return PantsTextureArray [index];

                default:
                    return null;
            }
        }
        set
        {
            switch (ArrayNum)
            {
                case 0:
                    SkinTextureArray [index] = value;
                    break;

                case 1:
                    ShirtTextureArray [index] = value;
                    break;

                case 2:
                    PantsTextureArray [index] = value;
                    break;

                default:
                    throw new System.IndexOutOfRangeException("ZombieTextureCollection(): Invalid integer given to access arrays");
            }
        }
    }

    public Texture PickRandom()
    {
        int RandomArrayNum = Random.Range(0, Random.Range(0, 2));
        switch (RandomArrayNum)
        {
            case 0:
                return SkinTextureArray [Random.Range(0, SkinTextureArray.Length)];

            case 1:
                return ShirtTextureArray [Random.Range(0, ShirtTextureArray.Length)];

            case 2:
                return PantsTextureArray [Random.Range(0, PantsTextureArray.Length)];

            default:
                return null;
        }
    }

    public int Length
    {
        get
        {
            int SumArrayLenghts = 0;

            SumArrayLenghts += SkinTextureArray.Length;
            SumArrayLenghts += ShirtTextureArray.Length;
            SumArrayLenghts += PantsTextureArray.Length;

            return SumArrayLenghts;
        }
    }
}
