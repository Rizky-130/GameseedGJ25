using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    public AudioSource audioSource;
    public AudioClip musicClip;

    void Awake()
    {
        // Enforce singleton: if one already exists, kill this duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic()
    {
        if (audioSource == null || musicClip == null)
            return;

        if (audioSource.isPlaying && audioSource.clip == musicClip)
            return;

        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopMusic()
    {
        if (audioSource == null)
            return;

        audioSource.Stop();
    }

    // Call this from any scene where you want the music (and this manager)
    // to be removed entirely rather than just paused.
    public void StopAndDestroy()
    {
        StopMusic();
        Instance = null;
        Destroy(gameObject);
    }
}
