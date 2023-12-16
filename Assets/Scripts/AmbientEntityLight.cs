using System;
using UnityEngine;

[Serializable]
public class AmbientEntityLight
{
    [Header("Main")]
    public bool SwitchedOn = false;
    public bool Flicker = false;
    public float FlickChancePercent = 20f; //Chance the light will flick per frame in percents//
    public Light LightObject;
    public Material MainMaterial;
    public Material AltMaterial;

    public void Switch (bool On)
    {
        LightObject.enabled = On;

        if (On)
        {
            if (MainMaterial != null)
            {
                LightObject.GetComponentInChildren<MeshRenderer>().material.CopyPropertiesFromMaterial(MainMaterial);
            }
        }
        else 
        {
            if (AltMaterial != null)
            {
                LightObject.GetComponentInChildren<MeshRenderer>().material.CopyPropertiesFromMaterial(AltMaterial);
            }
        }
    }

    public void Flick ()
    {
        bool Flick = UnityEngine.Random.value <= (FlickChancePercent / 100f);

        if (Flick)
        {
            Switch(true);
        }
        else
        {
            Switch(false);
        }
    }

    public void SetMainMaterial (Material CurrentMainMaterial)
    {
        if (CurrentMainMaterial != null)
        {
            MainMaterial = CurrentMainMaterial;
        }
    }

    public void SetAltMaterial(Material CurrentAltMaterial)
    {
        if (CurrentAltMaterial != null)
        {
            AltMaterial = CurrentAltMaterial;
        }
    }
}
