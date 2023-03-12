using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightItem : MonoBehaviour
{
    [Header("Main")]
    public Light MainLight;
    public bool LightState;

    private GameManager GameManagerScript;
    private ControlsManager ControlsManagerScript;

    void Awake()
    {
        GameManagerScript = FindObjectOfType<GameManager>();
        ControlsManagerScript = GameManagerScript.GetComponent<ControlsManager>();
        MainLight = GetComponentInChildren<Light>();
    }

    void Start()
    {
        LightState = true;
        MainLight.enabled = LightState;
    }

    void Update()
    {
        if (GameManagerScript.GameStatus != GameManager.GameState.InGame)
        {
            return;
        }

        if (Input.GetKeyDown(ControlsManagerScript.ControlDictionary ["AltUse"]))
        {
            SwitchLight();
        }
    }

    public void SwitchLight ()
    {
        LightState = !LightState;
        MainLight.enabled = LightState;
    }
}
