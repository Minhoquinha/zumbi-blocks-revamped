using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsManager : MonoBehaviour
{
    [Header("Main")]
    public ControlsCollection ControlsCollectionScript;
    public Dictionary<string, KeyCode> ControlDictionary = new Dictionary<string, KeyCode>();
    public float MouseSensitivity;
    public Control [] ControlArray;
    [HideInInspector]
    public bool Binding = false;
    public string BindingString = "PRESS KEY";
    [HideInInspector]
    public Control CurrentBindingControl;
    public Button [] MenuButtons;
    public Slider MouseSensitivitySlider;

    void Awake()
    {
        Binding = false;

        SetDefaultControls();
        LoadControls();
        SaveControls();
    }

    void Update()
    {
        if (Binding)
        {
            ChangeControls(CurrentBindingControl.MenuButton);
        }
    }

    public void ChangeControls(Button CurrentButton)
    {
        if (!Binding)
        {
            foreach (Control CurrentControl in ControlArray)
            {
                if (CurrentButton == CurrentControl.MenuButton)
                {
                    CurrentControl.MenuButton = CurrentButton;
                    CurrentBindingControl = CurrentControl;
                    break;
                }
            }
        }

        Binding = true;
        CurrentButton.GetComponentInChildren<Text>().text = BindingString;

        foreach (KeyCode Key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(Key))
            {
                Binding = false;
                CurrentBindingControl.Key = Key;
                CurrentButton.GetComponentInChildren<Text>().text = Key.ToString();
                break;
            }
        }

        SaveControls();
    }

    public void SetMouseSensitivity (float TargetMouseSensitivity)
    {
        MouseSensitivity = TargetMouseSensitivity;

        SaveControls();
    }

    public void ResetAllControls()
    {
        //Load ControlsCollectionScript.ControlArray from a .json file//
        if (File.Exists(Application.dataPath + "/defaultcontrols.json"))
        {
            string DefaultControlsJSON = File.ReadAllText(Application.dataPath + "/defaultcontrols.json");
            JsonUtility.FromJsonOverwrite(DefaultControlsJSON, ControlsCollectionScript);

            ControlArray = ControlsCollectionScript.ControlArray;
            MouseSensitivity = ControlsCollectionScript.MouseSensitivity;
        }
        else
        {
            SetDefaultControls();
            LoadControls();
        }
        //Load ControlsCollectionScript.ControlArray from a .json file//

        foreach (Button CurrentButton in MenuButtons)
        {
            if (CurrentButton != null)
            {
                foreach (Control CurrentControl in ControlArray)
                {
                    if (CurrentButton.name == CurrentControl.Name + "Button")
                    {
                        CurrentControl.MenuButton = CurrentButton;
                        CurrentControl.MenuButton.GetComponentInChildren<Text>().text = CurrentControl.Key.ToString();
                    }
                }
            }
        }

        if (MouseSensitivitySlider != null)
        {
            MouseSensitivitySlider.value = MouseSensitivity;
        }

        SaveControls();
    }

    void SetDefaultControls()
    {
        MouseSensitivity = 6f;

        ControlArray [0].Name = "MoveForward";
        ControlArray [0].Key = KeyCode.W;
        ControlArray [1].Name = "MoveLeft";
        ControlArray [1].Key = KeyCode.A;
        ControlArray [2].Name = "MoveBackward";
        ControlArray [2].Key = KeyCode.S;
        ControlArray [3].Name = "MoveRight";
        ControlArray [3].Key = KeyCode.D;

        ControlArray [4].Name = "Jump";
        ControlArray [4].Key = KeyCode.Space;
        ControlArray [5].Name = "Crouch";
        ControlArray [5].Key = KeyCode.LeftControl;
        ControlArray [6].Name = "Sprint";
        ControlArray [6].Key = KeyCode.LeftShift;

        ControlArray [7].Name = "BasicFire";
        ControlArray [7].Key = KeyCode.Mouse0;
        ControlArray [8].Name = "Aim";
        ControlArray [8].Key = KeyCode.Mouse1;
        ControlArray [9].Name = "Reload";
        ControlArray [9].Key = KeyCode.R;
        ControlArray [10].Name = "Inspect";
        ControlArray [10].Key = KeyCode.F;
        ControlArray [11].Name = "AltFire";
        ControlArray [11].Key = KeyCode.Mouse1;

        ControlArray [12].Name = "ItemInteract";
        ControlArray [12].Key = KeyCode.E;
        ControlArray [13].Name = "ItemDrop";
        ControlArray [13].Key = KeyCode.G;
        ControlArray [14].Name = "ItemSlot1";
        ControlArray [14].Key = KeyCode.Alpha1;
        ControlArray [15].Name = "ItemSlot2";
        ControlArray [15].Key = KeyCode.Alpha2;
        ControlArray [16].Name = "ItemSlot3";
        ControlArray [16].Key = KeyCode.Alpha3;
        ControlArray [17].Name = "ItemSlot4";
        ControlArray [17].Key = KeyCode.Alpha4;
        ControlArray [18].Name = "ItemSlot5";
        ControlArray [18].Key = KeyCode.Alpha5;
        ControlArray [19].Name = "ItemSlot6";
        ControlArray [19].Key = KeyCode.Alpha6;
        ControlArray [20].Name = "ItemSlot7";
        ControlArray [20].Key = KeyCode.Alpha7;
        ControlArray [21].Name = "ItemSlot8";
        ControlArray [21].Key = KeyCode.Alpha8;
        ControlArray [22].Name = "ItemSlot9";
        ControlArray [22].Key = KeyCode.Alpha9;

        ControlArray [23].Name = "BasicAttack";
        ControlArray [23].Key = KeyCode.Mouse0;
        ControlArray [24].Name = "AltAttack";
        ControlArray [24].Key = KeyCode.Mouse1;

        ControlArray [25].Name = "BasicThrow";
        ControlArray [25].Key = KeyCode.Mouse0;
        ControlArray [26].Name = "AltThrow";
        ControlArray [26].Key = KeyCode.Mouse1;
        ControlArray [27].Name = "BasicPlace";
        ControlArray [27].Key = KeyCode.Mouse0;
        ControlArray [28].Name = "AltPlace";
        ControlArray [28].Key = KeyCode.Mouse1;
        ControlArray [29].Name = "BasicUse";
        ControlArray [29].Key = KeyCode.Mouse0;
        ControlArray [30].Name = "AltUse";
        ControlArray [30].Key = KeyCode.Mouse1;

        ControlArray [31].Name = "SpectatorForward";
        ControlArray [31].Key = KeyCode.W;
        ControlArray [32].Name = "SpectatorLeft";
        ControlArray [32].Key = KeyCode.A;
        ControlArray [33].Name = "SpectatorBackward";
        ControlArray [33].Key = KeyCode.S;
        ControlArray [34].Name = "SpectatorRight";
        ControlArray [34].Key = KeyCode.D;
        ControlArray [35].Name = "SpectatorUp";
        ControlArray [35].Key = KeyCode.Space;
        ControlArray [36].Name = "SpectatorDown";
        ControlArray [36].Key = KeyCode.V;
        ControlArray [37].Name = "SpectatorFast";
        ControlArray [37].Key = KeyCode.LeftShift;
        ControlArray [38].Name = "SpectatorSlow";
        ControlArray [38].Key = KeyCode.LeftControl;

        foreach (Button CurrentButton in MenuButtons)
        {
            if (CurrentButton != null)
            {
                foreach (Control CurrentControl in ControlArray)
                {
                    if (CurrentButton.name == CurrentControl.Name + "Button")
                    {
                        CurrentControl.MenuButton = CurrentButton;
                        CurrentControl.MenuButton.GetComponentInChildren<Text>().text = CurrentControl.Key.ToString();
                    }
                }
            }
        }

        ControlsCollectionScript.ControlArray = ControlArray;
        ControlsCollectionScript.MouseSensitivity = MouseSensitivity;

        //Save ControlArray to a .json file//
        string DefaultControlsJSON = JsonUtility.ToJson(ControlsCollectionScript);
        File.WriteAllText(Application.dataPath + "/defaultcontrols.json", DefaultControlsJSON);

        if (!File.Exists(Application.dataPath + "/controls.json"))
        {
            File.WriteAllText(Application.dataPath + "/controls.json", DefaultControlsJSON);
        }
        //Save ControlArray to a .json file//
    }

    public void LoadControls()
    {
        //Load ControlsCollectionScript from a .json file//
        if (File.Exists(Application.dataPath + "/controls.json"))
        {
            string ControlsJSON = File.ReadAllText(Application.dataPath + "/controls.json");
            JsonUtility.FromJsonOverwrite(ControlsJSON, ControlsCollectionScript);

            ControlArray = ControlsCollectionScript.ControlArray;
            MouseSensitivity = ControlsCollectionScript.MouseSensitivity;
        }
        else
        {
            ResetAllControls();
        }
        //Load ControlsCollectionScript from a .json file//

        foreach (Button CurrentButton in MenuButtons)
        {
            if (CurrentButton != null)
            {
                foreach (Control CurrentControl in ControlArray)
                {
                    if (CurrentButton.name == CurrentControl.Name + "Button")
                    {
                        CurrentControl.MenuButton = CurrentButton;
                        CurrentControl.MenuButton.GetComponentInChildren<Text>().text = CurrentControl.Key.ToString();
                    }
                }
            }
        }

        if (MouseSensitivitySlider != null)
        {
            MouseSensitivitySlider.value = MouseSensitivity;
        }
    }

    public void SaveControls()
    {
        ControlDictionary.Clear();

        foreach (Control CurrentControl in ControlArray)
        {
            if (!ControlDictionary.ContainsKey(CurrentControl.Name))
            {
                ControlDictionary.Add(CurrentControl.Name, CurrentControl.Key);
            }
        }

        ControlsCollectionScript.ControlArray = ControlArray;
        ControlsCollectionScript.MouseSensitivity = MouseSensitivity;

        //Save ControlsCollectionScript to a .json file//
        string ControlsJSON = JsonUtility.ToJson(ControlsCollectionScript);
        File.WriteAllText(Application.dataPath + "/controls.json", ControlsJSON);
        //Save ControlsCollectionScript to a .json file//
    }
}

