using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonInit : MonoBehaviourPunCallbacks
{
    public string version = "V1.0";
    public InputField userID;

    void Start()
    {
        PhotonNetwork.GameVersion = version;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
        //PhotonNetwork.JoinRandomRoom();
        userID.text = GetUserID();
    }

    string GetUserID()
    {
        string UserID = PlayerPrefs.GetString("USER_ID");

        if (string.IsNullOrEmpty(UserID))
            UserID = "USER_" + Random.Range(0, 1000).ToString("000");

        return UserID;
    }

    //Button OnClick을 위한 함수    
    public void OnClickJoinRandomRoom()
    {
        PhotonNetwork.NickName = userID.text;   //포톤서버에 ID 전달
        PlayerPrefs.SetString("USER_ID", userID.text);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("No Room");
        PhotonNetwork.CreateRoom("Room1", new RoomOptions { MaxPlayers = 20 });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        //PhotonNetwork.LoadLevel("TankScene");
        //CreateTank();
        StartCoroutine(LoadTankScene());
    }

    IEnumerator LoadTankScene()
    {
        PhotonNetwork.IsMessageQueueRunning = false;    //Scene이 이동하는 동안 Photon Cloud Server로부터 Network Message 수신 중단
        AsyncOperation ao = SceneManager.LoadSceneAsync("TankScene");   //비동기 방식을 사용해 씬로딩 
        yield return ao;
    }

    /* void CreateTank()
    {
        float pos = Random.Range(-50f, 50f);
        PhotonNetwork.Instantiate("Tank", new Vector3(pos, 5f, pos), Quaternion.identity, 0);
    } */

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.InRoom.ToString());
    }
}
