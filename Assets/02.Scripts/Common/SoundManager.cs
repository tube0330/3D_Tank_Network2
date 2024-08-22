using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public bool isMute = false;
    public static SoundManager s_instance;

    void Awake()
    {
        if (s_instance == null)
            s_instance = this;
        else if (s_instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void BackgroundSound(Vector3 pos, AudioClip bgm, bool isLoop)
    {
        if (isMute) return;

        GameObject soundobj = new GameObject("BackgroundSound");
        soundobj.transform.SetParent(transform);
        soundobj.transform.position = pos;
        AudioSource audioSource = soundobj.AddComponent<AudioSource>();

        audioSource.clip = bgm;
        audioSource.loop = isLoop;
        audioSource.minDistance = 10f;
        audioSource.maxDistance = 30f;
        audioSource.volume = 1.0f;
        audioSource.Play();

    }

    public void OtherPlayerSound(Vector3 pos, AudioClip bgm, bool isLoop)
    {
        if (isMute) return;

        GameObject soundobj = new GameObject("SFX");
        soundobj.transform.SetParent(transform);
        soundobj.transform.position = pos;
        AudioSource audioSource = soundobj.AddComponent<AudioSource>();
        audioSource.clip = bgm;

        audioSource.minDistance = 10f;
        audioSource.maxDistance = 30f;
        audioSource.volume = 1.0f;
        audioSource.Play();
    }
}