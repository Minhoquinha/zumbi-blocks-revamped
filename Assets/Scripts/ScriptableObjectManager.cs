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

    void Awake()
    {
        ItemCollectionScript.SetID();
    }

    void Update()
    {
        
    }
}
