using System;
using UnityEngine;

[Serializable]
public class AmbientEntityLight
{
    [Header("Main")]
    public bool SwitchedOn = false;
    public bool Flicker = false;
    public float FlickChancePercent = 20f; //Chance the light will flick per frame in percents//
    public Light EntityLight;

    public void Flick ()
    {
        bool Flick = UnityEngine.Random.value <= (FlickChancePercent / 100f);

        if (Flick)
        {
            EntityLight.enabled = true;
        }
        else
        {
            EntityLight.enabled = false;
        }
    }
}
