using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{

    [Header("Visible Values")]
    public float MaxHealth; //Maximum health visible in the UI//
    public float MaxStamina; //Maximum stamina visible in the UI//
    public float MaxNoise; //Maximum noise visible in the UI//
    public float HitmarkerDuration = 1f; //How much time the hitmarker spends on the screen after being cross faded//

    [Header("Main References")]
    [Space(50)]
	public Image DeathScreen;
	public Image HurtScreen;
    public GameObject StatsPanel;
    public GameObject GunPanel;
    public Slider HealthBar;
    public Slider StaminaBar;
    public Slider NoiseBar;
	public Text WeaponTag;
	public Text AmmoCounter;
    public Image CenterCrosshair;
    public Image DynamicCrosshair;
	public Image Hitmarker;
    private Canvas CurrentCanvas;
    private SpectatorMovement SpectatorControlScript;

    void Awake()
    {
        CurrentCanvas = FindObjectOfType<Canvas>();

        DeathScreen = Instantiate(DeathScreen, CurrentCanvas.transform);
        HurtScreen = Instantiate(HurtScreen, CurrentCanvas.transform);
        StatsPanel = Instantiate(StatsPanel, CurrentCanvas.transform);
        Slider [] StatsPanelBarArray = StatsPanel.GetComponentsInChildren<Slider>();
        foreach (Slider Bar in StatsPanelBarArray)
        {
            if (Bar.name.Contains("Health"))
            {
                HealthBar = Bar;
            }

            if (Bar.name.Contains("Stamina"))
            {
                StaminaBar = Bar;
            }

            if (Bar.name.Contains("Noise"))
            {
                NoiseBar = Bar;
            }
        }
        GunPanel = Instantiate(GunPanel, CurrentCanvas.transform);
        Text [] GunPanelTextArray = GunPanel.GetComponentsInChildren<Text>();
        foreach (Text GunText in GunPanelTextArray)
        {
            if (GunText.name.Contains("Weapon"))
            {
                WeaponTag = GunText;
            }

            if (GunText.name.Contains("Ammo"))
            {
                AmmoCounter = GunText;
            }
        }

        CenterCrosshair = Instantiate(CenterCrosshair, CurrentCanvas.transform);
        DynamicCrosshair = Instantiate(DynamicCrosshair, CurrentCanvas.transform);
        Hitmarker = Instantiate(Hitmarker, CurrentCanvas.transform);

        CenterCrosshair.rectTransform.position = new Vector2 (Screen.width / 2f, Screen.height / 2f);

        CenterCrosshair.enabled = true;
        DynamicCrosshair.enabled = true;
        Hitmarker.enabled = true;

        SpectatorControlScript = GetComponent<SpectatorMovement>();
    }
    void Start()
    {
        if (SpectatorControlScript != null)
        {
            if (DeathScreen != null)
            {
                DeathScreen.enabled = false;
            }
            if (HurtScreen != null)
            {
                HurtScreen.enabled = false;
            }

            if (StatsPanel != null)
            {
                StatsPanel.SetActive(false);
            }
            if (GunPanel != null)
            {
                GunPanel.SetActive(false);
            }

            if (CenterCrosshair != null)
            {
                CenterCrosshair.enabled = false;
            }
            if (DynamicCrosshair != null)
            {
                DynamicCrosshair.enabled = false;
            }
        }

        if (DynamicCrosshair != null && CenterCrosshair != null)
        {
            DynamicCrosshair.rectTransform.position = CenterCrosshair.rectTransform.position;
        }

        if (Hitmarker != null)
        {
            Hitmarker.enabled = false;
        }
    }

    public void HealthChange(float Amount)
    {
        if (Amount > MaxHealth)
        {
            Amount = MaxHealth;
        }

        if (HealthBar != null)
        {
            HealthBar.value = Amount / MaxHealth;
        }
    }

    public void StaminaChange(float Amount)
    {
        if (Amount > MaxStamina)
        {
            Amount = MaxStamina;
        }

        if (StaminaBar != null)
        {
            StaminaBar.value = Amount / MaxStamina;
        }
    }

    public void NoiseChange(float Amount)
    {
        if (Amount > MaxNoise)
        {
            Amount = MaxNoise;
        }

        if (NoiseBar != null)
        {
            NoiseBar.value = Amount / MaxNoise;
        }
    }
}
