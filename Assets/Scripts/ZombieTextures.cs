using System;
using UnityEngine;

[Serializable]
public class ZombieTextures
{
    [Header("Main")]
    public string Name;

    [Header("Textures")]
    public Texture [] PrimaryTextureArray; //List of all the primary textures possible for this zombie//
    public Texture [] SecondaryTextureArray; //List of all the secondary textures possible for this zombie//
    public Texture [] TertiaryTextureArray; //List of all the tertiary textures possible for this zombie//
    public Texture [] QuaternaryTextureArray; //List of all the quaternary textures possible for this zombie//
}
