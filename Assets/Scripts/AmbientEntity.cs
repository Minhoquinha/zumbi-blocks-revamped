using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientEntity : MonoBehaviour
{
    [Header("Main")]
    public bool SwitchedOn = false;

    [Header("Main References")]
    [Space(50)]
    public AmbientEntityLight [] EntityLights = new AmbientEntityLight [16];

    void Awake()
    {
        Light [] Lights = GetComponentsInChildren<Light>();

        for (int i = 0; i < EntityLights.Length; i++)
        {
            if (i >= Lights.Length)
            {
                break;
            }

            Light CurrentLight = Lights[i];

            if (CurrentLight != null)
            {
                EntityLights [i].EntityLight = CurrentLight;
            }
            else
            {
                break;
            }
        }
    }

    void Start()
    {
        if (SwitchedOn)
        {
            if (EntityLights != null)
            {
                foreach (AmbientEntityLight CurrentAmbientEntityLight in EntityLights)
                {
                    if (CurrentAmbientEntityLight != null)
                    {
                        if (CurrentAmbientEntityLight.SwitchedOn)
                        {
                            CurrentAmbientEntityLight.EntityLight.enabled = true;
                        }
                        else
                        {
                            CurrentAmbientEntityLight.EntityLight.enabled = false;
                        }
                    }
                }
            }
        }
        else
        {
            if (EntityLights != null)
            {
                foreach (AmbientEntityLight CurrentAmbientEntityLight in EntityLights)
                {
                    if (CurrentAmbientEntityLight != null)
                    {
                        CurrentAmbientEntityLight.EntityLight.enabled = false;
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!SwitchedOn)
        {
            return;
        }

        if (EntityLights != null)
        {
            foreach (AmbientEntityLight CurrentAmbientEntityLight in EntityLights)
            {
                if (CurrentAmbientEntityLight.Flicker)
                {
                    CurrentAmbientEntityLight.Flick();
                }
            }
        }
    }
}
