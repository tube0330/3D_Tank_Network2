using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class CameraSetup : MonoBehaviourPun
{
    void Start()
    {
        CinemachineVirtualCamera followcam = FindObjectOfType<CinemachineVirtualCamera>();

        //가상카메라의 추적대상을 자신의 transform으로 설정
        followcam.Follow = transform;
        followcam.LookAt = transform;
    }
}
