using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Mathematics;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Sounds References")]
    [SerializeField] private AudioSource soundObj;
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioClip[] bgmClip;
    [SerializeField] private float bgmVol;
    [SerializeField] private float sfxVol;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        PlayBgm(0);
    }

    public void PlayBgm(int clip)
    {
        if (bgmClip == null)
            return;
        bgmSource.clip = bgmClip[clip];
        bgmSource.volume = bgmVol;
        bgmSource.Play();
    }

    public void PlaySfx(AudioClip audioClip, Transform spawnTransform)
    {
        AudioSource audioSource = Instantiate(soundObj, spawnTransform.position, quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = sfxVol;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public float GetBGMVol()
    {
        return bgmVol;
    }
    public float GetSFXVol()
    {
        return sfxVol;
    }
    public void SetBGMVol(float vol)
    {
        bgmSource.volume = vol;
        bgmVol = vol;
    }
    public void SetSFXVol(float vol)
    {
        sfxVol = vol;
    }
}
