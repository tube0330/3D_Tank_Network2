using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class VirtualCam : MonoBehaviourPun
{
    CinemachineVirtualCamera cineCam;
    void Start()
    {
        cineCam = FindObjectOfType<CinemachineVirtualCamera>();
    }

    void Update()
    {
        if (photonView.IsMine)
            cineCam.LookAt = transform;
    }
}
