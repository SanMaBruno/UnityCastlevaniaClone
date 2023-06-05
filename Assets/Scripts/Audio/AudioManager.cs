using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float musicVolume = 1f;
    [Range(0, 1)]
    [SerializeField] private float sfxVolume = 1f;

    private AudioSource musicAudioSource;
    private AudioSource sfxAudioSource;

    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var existingObject = GameObject.Find("AudioManager");
                if (existingObject == null)
                {
                    existingObject = new GameObject("AudioManager");
                    _instance = existingObject.AddComponent<AudioManager>();
                    DontDestroyOnLoad(existingObject);
                }
                else
                {
                    _instance = existingObject.GetComponent<AudioManager>();
                }

                var gameMusic = new GameObject("Music");
                gameMusic.transform.parent = existingObject.transform;
                _instance.musicAudioSource = gameMusic.AddComponent<AudioSource>();

                var gameSfx = new GameObject("Sfx");
                gameSfx.transform.parent = existingObject.transform;
                _instance.sfxAudioSource = gameSfx.AddComponent<AudioSource>();
            }
            return _instance;
        }
    }

    private void Update()
    {
        musicAudioSource.volume = musicVolume;
        sfxAudioSource.volume = sfxVolume;
    }

    public void PlaySfx(AudioClip audioClip)
    {
        sfxAudioSource.PlayOneShot(audioClip);
    }

    public void PlayMusic(AudioClip audioClip)
    {
        if (musicAudioSource.clip != audioClip)
        {
            musicAudioSource.clip = audioClip;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }
    }
}
