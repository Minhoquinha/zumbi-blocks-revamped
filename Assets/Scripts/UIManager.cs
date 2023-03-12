using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Main")]
    public static UIManager Instance;

    [Header("Menu Structures")]
    public RawImage Title;
    public GameObject[] MenuPanels;
    //Each number represents a menu panel//
    [HideInInspector]
    public const int MainMenuPanel = 0;
    [HideInInspector]
    public const int PlayMenuPanel = 1;
    [HideInInspector]
    public const int CharacterMenuPanel = 2;
    [HideInInspector]
    public const int OptionsMenuPanel = 3;
    [HideInInspector]
    public const int CreditsMenuPanel = 4;
    [HideInInspector]
    public const int ServerListPanel = 5;
    [HideInInspector]
    public const int AppearanceMenuPanel = 6;
    [HideInInspector]
    public const int ShopMenuPanel = 7;
    [HideInInspector]
    public const int LoadoutMenuPanel = 8;
    [HideInInspector]
    public const int ControlsMenuPanel = 9;

    public GameObject PauseMenuPanel;

    [Header("Primary In-Game Central Text")]
    public bool UsePCenterText;
    public Text PCenterText;
    public float PCenterTextDefaultTime; //How much time for the text to disable after being enabled//

    [Header("Secondary In-Game Central Text")]
    public bool UseSCenterText;
    public Text SCenterText;
    public float SCenterTextDefaultTime; //How much time for the text to disable after being enabled//

    [Header("Game StopWatch")]
    public bool UseStopWatch;
    public Text StopWatch;
    private float SWTSeconds;
    private int SWTMinutes;
    private int SWTHours;

    [Header("Main References")]
    [Space(50)]
    public GameManager GameManagerScript;
    public Canvas Screen;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("More than one " + this.name + " loaded;");
            return;
        }

        if (UseStopWatch && StopWatch == null)
        {
            UseStopWatch = false;
            Debug.LogWarning("StopWatch not found;");
        }

        if (PauseMenuPanel != null)
        {
            PauseMenuPanel.SetActive(false);
        }

        if (PCenterText != null)
        {
            PCenterText.enabled = false;
        }
        else if (UsePCenterText)
        {
            UsePCenterText = false;
            Debug.LogWarning("PCenterText not found;");
        }

        if (SCenterText != null)
        {
            SCenterText.enabled = false;
        }
        else if (UseSCenterText)
        {
            UseSCenterText = false;
            Debug.LogWarning("SCenterText not found;");
        }
    }
    public void PCenterTextMessage (string Message, float CenterTextDuration)
    {
        if (UsePCenterText)
        {
            PCenterText.enabled = true;
            PCenterText.text = Message;

            if (CenterTextDuration == 0f)
            {
                CenterTextDuration = PCenterTextDefaultTime;
            }
            StartCoroutine(ClosePCenterText(CenterTextDuration));
        }
    }

    public void SCenterTextMessage (string Message, float CenterTextDuration)
    {
        if (UseSCenterText)
        {
            SCenterText.enabled = true;
            SCenterText.text = Message;

            if (CenterTextDuration <= 0f)
            {
                CenterTextDuration = SCenterTextDefaultTime;
            }
            StartCoroutine(CloseSCenterText(CenterTextDuration));
        }
    }

    public IEnumerator ClosePCenterText (float CenterTextDuration)
    {
        yield return new WaitForSeconds(CenterTextDuration);

        PCenterText.enabled = false;
    }

    public IEnumerator CloseSCenterText(float CenterTextDuration)
    {
        yield return new WaitForSeconds(CenterTextDuration);

        SCenterText.enabled = false;
    }

    public void ShowTime(float Amount)
    {
        SWTSeconds = Amount - (SWTMinutes * 60f);

        if (SWTSeconds >= 60f)
        {
            SWTMinutes++;
            SWTSeconds -= 60f;
        }
        if (SWTMinutes == 60)
        {
            SWTHours++;
            SWTMinutes -= 60;
        }

        StopWatch.text = SWTHours + ":" + SWTMinutes + ":" + SWTSeconds.ToString("F2");
    }

    public void Menu (int MenuPanelNum)
    {
        for (int i = 0; i < MenuPanels.Length; i++)
        {
            if (MenuPanels [i] != null)
            {
                MenuPanels [i].SetActive(false);
            }
        }

        if (PauseMenuPanel != null)
        {
            PauseMenuPanel.SetActive(false);
        }

        if (MenuPanels[MenuPanelNum] != null)
        {
            MenuPanels [MenuPanelNum].SetActive(true);
        }

        if (Title != null)
        {
            if (MenuPanelNum == MainMenuPanel || MenuPanelNum == CreditsMenuPanel)
            {
                Title.enabled = true;
            }
            else
            {
                Title.enabled = false;
            }
        }
    }

    public void CloseMenu (bool Pause)
    {
        for (int i = 0; i < MenuPanels.Length; i++)
        {
            if (MenuPanels [i] != null)
            {
                MenuPanels [i].SetActive(false);
            }
        }

        if (PauseMenuPanel != null)
        {
            PauseMenuPanel.transform.SetAsLastSibling();

            if (Pause)
            {
                PauseMenuPanel.SetActive(true);

            }
            else
            {
                PauseMenuPanel.SetActive(false);
            }
        }

        if (Title != null)
        {
            Title.enabled = false;
        }
    }
}
