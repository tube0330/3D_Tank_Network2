using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlay : MonoBehaviour
{
    public AudioClip bgm;
    Transform tr;

    void Start()
    {
        tr = transform;
        SoundManager.s_instance.BackgroundSound(tr.position, bgm, true);
    }
}
