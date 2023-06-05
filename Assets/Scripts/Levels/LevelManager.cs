using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] AudioClip levelMusic;

    IEnumerator Start()
    {
        while (AudioManager.Instance == null)
        {
            yield return null;
        }
        AudioManager.Instance.PlayMusic(levelMusic);
    }
}
