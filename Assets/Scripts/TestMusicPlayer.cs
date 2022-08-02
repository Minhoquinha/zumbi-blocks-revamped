using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMusicPlayer : MonoBehaviour
{
    public AudioManager Audio;
    public bool PlaySongs;
    public string [] SongArray;
    private int SongNum;

    void Start()
    {
        Audio = FindObjectOfType<AudioManager>();
        SongNum = 0;

        if (PlaySongs)
        {
            Audio.Play(SongArray [SongNum].ToString());
            Debug.Log("Playing music:" + SongArray [SongNum].ToString());
        }
    }

    void Update()
    {
        if (PlaySongs && Input.GetKeyDown(KeyCode.K))
        {
            if (SongNum >= SongArray.Length - 1)
            {
                Audio.Stop(SongArray [SongNum].ToString());
                Debug.Log("Stopping music:" + SongArray [SongNum].ToString());

                SongNum = 0;

                Audio.Play(SongArray [SongNum].ToString());
                Debug.Log("Playing music:" + SongArray [SongNum].ToString());
            }
            else
            {
                Audio.Stop(SongArray [SongNum].ToString());
                Debug.Log("Stopping music:" + SongArray [SongNum].ToString());

                SongNum++;

                Audio.Play(SongArray [SongNum].ToString());
                Debug.Log("Playing music:" + SongArray [SongNum].ToString());
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (PlaySongs)
            {
                Audio.Stop(SongArray [SongNum].ToString());
                Debug.Log("Stopping music:" + SongArray [SongNum].ToString());
                PlaySongs = false;
            }
            else
            {
                Audio.Play(SongArray [SongNum].ToString());
                Debug.Log("Playing music:" + SongArray [SongNum].ToString());
                PlaySongs = true;
            }
        }
    }
}
