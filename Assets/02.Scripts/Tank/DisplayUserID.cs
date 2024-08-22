using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class DisplayUserID : MonoBehaviourPun
{
    public Text userID;

    void Start()
    {
        userID.text = photonView.Owner.NickName;
    }
}
