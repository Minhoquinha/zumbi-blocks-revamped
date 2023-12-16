using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientEntity : MonoBehaviour
{
    [Header("Main")]
    public bool SwitchedOn = false;
    public bool SetMaterials = false;
    private float CurrentDelay = 0f;
    public float AlternateDelay = 1f;
    public enum LightState
    {
        On, Off, Alternating
    }
    public LightState LightMode;
    private int Alternator = 0;

    [Header("Main References")]
    [Space(50)]
    public AmbientEntityLight [] EntityLights = new AmbientEntityLight [16];
    public Material [] EntityMaterials = new Material [8];

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
                EntityLights [i].LightObject = CurrentLight;
            }
            else
            {
                break;
            }
        }

        if (SetMaterials)
        {
            if (EntityMaterials != null && EntityMaterials.Length > 4)
            {
                SetAlternatingAmbientLightsMaterials(EntityMaterials [0], EntityMaterials [1], EntityMaterials [2], EntityMaterials [3]);
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
                            CurrentAmbientEntityLight.Switch(true);
                        }
                        else
                        {
                            CurrentAmbientEntityLight.Switch(false);
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
                        CurrentAmbientEntityLight.Switch(false);
                    }
                }
            }
        }

        Alternator = 0;
    }

    void FixedUpdate()
    {
        if (!SwitchedOn)
        {
            return;
        }

        if (EntityLights != null)
        {
            switch (LightMode)
            {
                case LightState.On:
                    foreach (AmbientEntityLight CurrentAmbientEntityLight in EntityLights)
                    {
                        CurrentAmbientEntityLight.Switch(true);
                    }
                break;

                case LightState.Off:
                    foreach (AmbientEntityLight CurrentAmbientEntityLight in EntityLights)
                    {
                        CurrentAmbientEntityLight.Switch(false);
                    }
                break;

                case LightState.Alternating:
                    if (CurrentDelay <= 0f)
                    {
                        Alternator = AlternateLights();
                    }
                break;

                default:
                break;
            }

            foreach (AmbientEntityLight CurrentAmbientEntityLight in EntityLights)
            {
                if (CurrentAmbientEntityLight.Flicker)
                {
                    CurrentAmbientEntityLight.Flick();
                }
            }
        }

        CurrentDelay -= Time.deltaTime;
    }

    int AlternateLights ()
    {
        int i;

        for (i = Alternator; i < EntityLights.Length; i += 2)
        {
            AmbientEntityLight CurrentAmbientEntityLight = EntityLights [i];
            CurrentAmbientEntityLight.Switch(true);

            if (i + 1 < EntityLights.Length)
            {
                AmbientEntityLight NextAmbientEntityLight = EntityLights [i+1];
                NextAmbientEntityLight.Switch(false);
            }
        }

        CurrentDelay = AlternateDelay;
        return (i+1)%2;
    }

    int SetAlternatingAmbientLightsMaterials (Material MainOne, Material AltOne, Material MainTwo, Material AltTwo)
    {
        int i;

        for (i = Alternator; i < EntityLights.Length; i += 2)
        {
            AmbientEntityLight CurrentAmbientEntityLight = EntityLights [i];
            CurrentAmbientEntityLight.SetMainMaterial(MainOne);
            CurrentAmbientEntityLight.SetAltMaterial(AltOne);

            if (i + 1 < EntityLights.Length)
            {
                AmbientEntityLight NextAmbientEntityLight = EntityLights [i+1];
                NextAmbientEntityLight.SetMainMaterial(MainTwo);
                NextAmbientEntityLight.SetAltMaterial(AltTwo);
            }
        }

        CurrentDelay = AlternateDelay;
        return (i+1) % 2;
    }
}
