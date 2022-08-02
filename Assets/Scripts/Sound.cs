using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class Sound
{
    [Header ("Main")]
    public string Name;
	public AudioClip Clip;
    [HideInInspector]
    public AudioSource Source;
    public AudioMixerGroup Mixer;

    [Header("Properties")]
    [Range (0f, 1f)]
    public float Volume;
    [Range (0.1f, 3f)]
    public float Pitch;
	public bool Loop;
	public bool Mute;
}
