using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsManager : MonoBehaviour
{
    [Header("Main References")]
    public AudioMixer Mixer;
    public Slider VolumeSlider;
    public Text VolumeText;
    public Resolution[] ResolutionArray;
    public Dropdown ResolutionDropdown;
    public Text ResolutionText;
    [HideInInspector]
    public string [] GraphicsQualityArray;
    public Dropdown GraphicsQualityDropdown;
    public Text GraphicsQualityText;
    public Options OptionsScript;

    void Awake()
    {
        if (ResolutionDropdown != null)
        {
            ResolutionArray = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
            ResolutionDropdown.ClearOptions();

            int CurrentResolutionIndex = 0;
            List<string> DropdownOptions = new List<string>();
            for (int i = 0; i < ResolutionArray.Length; i++)
            {
                string Option = ResolutionArray [i].width + " x " + ResolutionArray [i].height;
                DropdownOptions.Add(Option);

                if (ResolutionArray [i].width == Screen.width && ResolutionArray [i].height == Screen.height)
                {
                    CurrentResolutionIndex = i;
                }
            }


            ResolutionDropdown.AddOptions(DropdownOptions);
            ResolutionDropdown.value = CurrentResolutionIndex;
            ResolutionDropdown.RefreshShownValue();
        }

        if (GraphicsQualityDropdown != null)
        {
            for (int i = 0; i < GraphicsQualityArray.Length; i++)
            {
                GraphicsQualityArray [i] = GraphicsQualityDropdown.options[i].text;
            }
        }
    }

    public void SetResolution (int ResolutionIndex)
    {
        Resolution AppliedResolution = ResolutionArray[ResolutionIndex];
        Screen.SetResolution(AppliedResolution.width, AppliedResolution.height, Screen.fullScreen);

        if (ResolutionText != null)
        {
            ResolutionText.text = "Resolution: " + ResolutionArray [ResolutionIndex].width + " x " + ResolutionArray [ResolutionIndex].height;
        }

        OptionsScript.Resolution = ResolutionIndex;
    }

    public void SetFullscreen (bool Fullscreen)
    {
        Screen.fullScreen = Fullscreen;
        OptionsScript.Fullscreen = Fullscreen;
    }

    public void SetVolume (float Volume)
    {
        Mixer.SetFloat("MasterVolume", Volume);

        if (VolumeSlider != null)
        {
            if (VolumeText != null)
            {
                VolumeText.text = "Volume: " + VolumeSlider.normalizedValue * 100f + "%";
            }
        }

        OptionsScript.Volume = Volume;
    }

    public void SetGraphicsQuality (int Quality)
    {
        QualitySettings.SetQualityLevel(Quality);

        if (GraphicsQualityText != null)
        {
            GraphicsQualityText.text = "Graphics quality: " + GraphicsQualityArray [Quality];
        }

        OptionsScript.GraphicsQuality = Quality;
    }
}
