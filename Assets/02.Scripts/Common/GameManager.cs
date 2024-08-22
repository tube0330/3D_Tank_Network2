using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPun
{
    private static GameManager G_instance;
    public static GameManager g_instance
    {
        get { return G_instance; }
        set
        {
            if (G_instance == null)
                G_instance = value;
            else if (G_instance != value)
                Destroy(value);
        }
    }
    [SerializeField] private List<Transform> spawnList;
    [SerializeField] private GameObject apachePrefab;
    public bool isGameOver = false;

    private void Awake()
    {
        g_instance = this;
        DontDestroyOnLoad(gameObject);
        apachePrefab = Resources.Load<GameObject>("ApacheAI");
        CreateTank();
        PhotonNetwork.IsMessageQueueRunning = true; //Photon Cloud Server로부터 Network Message를 받아 네트워크에서 동기화
    }

    void Start()
    {
        var spawnPoint = GameObject.Find("PatrollPoint").gameObject;

        if (spawnPoint != null)
            spawnPoint.GetComponentsInChildren<Transform>(spawnList);

        spawnList.RemoveAt(0);

        /* if (spawnList.Count > 0)
            //StartCoroutine(CreateApache());
            InvokeRepeating("CreateApache", 0.2f, 3f); */
    }

    /*  IEnumerator CreateApache()
     {
         while (isGameOver == false)
         {
             int count = (int)GameObject.FindGameObjectsWithTag("Apache").Length;
             if (count < 10)
             {
                 yield return new WaitForSeconds(3.0f);
                 int idx = Random.Range(0, spawnList.Count);
                 Instantiate(apachePrefab, spawnList[idx].position, spawnList[idx].rotation);
             }
             else
             {
                 yield return null;
             }
         }
     } */

    void CreateApache()
    {
        int count = (int)GameObject.FindGameObjectsWithTag("Apache").Length;

        if (count < 10)
        {
            int idx = Random.Range(0, spawnList.Count);
            Instantiate(apachePrefab, spawnList[idx].position, spawnList[idx].rotation);
        }
    }

    void CreateTank()
    {
        float pos = Random.Range(0f, 50f);
        PhotonNetwork.Instantiate("Tank", new Vector3(pos, 0f, pos), Quaternion.identity, 0);
    }

}
