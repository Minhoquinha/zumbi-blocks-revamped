using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour 
{
    [Header("Main References")]
    [Space(50)]
    public AudioMixerGroup MainAudioMixer;
    public static AudioManager Instance;
    public Sound [] SoundArray;

	void Awake () 
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

		foreach (Sound S in SoundArray)
        {
            S.Source = gameObject.AddComponent<AudioSource>();
            S.Source.outputAudioMixerGroup = MainAudioMixer;
            S.Source.clip = S.Clip;
            S.Source.volume = S.Volume;
            S.Source.pitch = S.Pitch;
			S.Source.loop = S.Loop;
			S.Source.mute = S.Mute;
        }         
	}

    void Start()
    {
        Debug.Log(this.name + " loaded;");
    }

    public void Play (string name) 
	{
        Sound S = Array.Find(SoundArray, sound => sound.Name == name);
        if (S == null)
        {
            Debug.LogWarning("Sound:" + name + " not found;");
            return;
        }
        S.Source.Play();
	}

	public void Stop (string name)
	{
		Sound S = Array.Find(SoundArray, sound => sound.Name == name);
		if (S == null)
		{
			Debug.LogWarning("Sound:" + name + "not found;");
			return;
		}
		S.Source.Stop ();
	}
}
