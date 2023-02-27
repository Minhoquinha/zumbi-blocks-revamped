using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectManager : MonoBehaviour
{
    [Header("Main References")]
    [Space(50)]
    public ItemCollection ItemCollectionScript;
    public PhysicMaterialCollection PhysicMaterialCollectionScript;
    public ImpactEffectCollection ImpactEffectCollectionScript;
    public ZombieTextureCollection ZombieTextureCollectionScript;

    void Awake()
    {
        ItemCollectionScript.SetID();
    }

    void Update()
    {
        
    }
}
