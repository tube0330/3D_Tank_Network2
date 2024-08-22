using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ApacheMoveAI : MonoBehaviourPun
{
    private Rigidbody rb;
    private Transform tr;
    private float moveSpeed = 0;
    private const float ApplyMoveSpeed = 0.3f;
    private float rotSpeed = 0;
    private const float ApplyRotSpeed = 0.9f;
    private float verticalSpeed = 0;
    private const float ApplyVerticalSpeed = 0.1f;

    public Transform[] trPoints;
    public List<Transform> pointList;
    public int nextPoint;

    public Vector3 applyMove;
    public int rotApply = 0;        // 1: right, -1: left, 0: none
    public bool moveApply = true;   // true: move, false: stop
    public int verticalApply = 0;   // 1: up, -1: down, 0: none

    public bool isSearch = false;
    public float TankFindDist;
    public Vector3 targetDist;
    public Quaternion targetRotation;

    private GameObject plasmaEffect;
    private GameObject bullet;

    public Transform[] firePos = new Transform[2];

    readonly string tankTag = "Tank";

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        trPoints = GameObject.FindWithTag("Point").GetComponentsInChildren<Transform>();
        pointList = new List<Transform>();
        for (int i = 1; i < trPoints.Length; i++)
        {
            pointList.Add(trPoints[i].transform);
        }
        plasmaEffect = Resources.Load<GameObject>("PlasmaExplosionEffect");
        bullet = Resources.Load<GameObject>("Bullet_ApacheAI");

        nextPoint = Random.Range(0, pointList.Count - 1);
    }

    void FixedUpdate()
    {
        ApacheMoveHorizontal();
        if (!isSearch)
        {
            ApacheRotate();
            ApacheMoveVertical();
        }
    }

    void Update()
    {
        ApplyMoveCheckPoint();
        CheckPoint();
        Search();
        Attack();
        if (isFire && !onFire)
            StartCoroutine(Fire());
    }



    private void CheckPoint()
    {
        if (Vector3.Distance(tr.position, pointList[nextPoint].position) < 50f)
        {
            /* nextPoint++;
            if (nextPoint >= PointList.Count)
                nextPoint = 0; */
            nextPoint = Random.Range(0, pointList.Count);
        }
    }

    private void ApplyMoveCheckPoint()
    {
        applyMove = transform.InverseTransformPoint(pointList[nextPoint].position).normalized;

        if (applyMove.x >= -1f && applyMove.x <= 0f)                       // right
        {
            rotApply = -1;
            moveApply = true;
        }
        else if (applyMove.x < 1f && applyMove.x > 0f)                     // left
        {
            rotApply = 1;
            moveApply = true;
        }

        if (isSearch) moveApply = false;
        else moveApply = true;

        if (applyMove.y >= -1f && applyMove.y <= 0f)                       // down
        {
            verticalApply = -1;
        }
        else if (applyMove.y < 1f && applyMove.y > 0f)                     // up
        {
            verticalApply = 1;
        }
    }
    private void ApacheRotate()
    {
        if (rotApply == -1)
        {
            if (rotSpeed > 0f) rotSpeed += -2.5f * ApplyRotSpeed;
            else rotSpeed += -1 * ApplyRotSpeed;
        }
        else if (rotApply == 1)
        {
            if (rotSpeed < 0f) rotSpeed += 2.5f * ApplyRotSpeed;
            else rotSpeed += 1 * ApplyRotSpeed;
        }
        else
        {
            if (rotSpeed > 2f) rotSpeed += -2 * ApplyRotSpeed;
            else if (rotSpeed > 0f) rotSpeed += -1 * ApplyRotSpeed;
            else if (rotSpeed < -2f) rotSpeed += 2 * ApplyRotSpeed;
            else if (rotSpeed < 0f) rotSpeed += 1 * ApplyRotSpeed;
        }
        rotSpeed = Mathf.Clamp(rotSpeed, -45.0f, 45.0f);
        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime);

        Quaternion targetRotation = Quaternion.Euler(0, tr.rotation.eulerAngles.y, 0);      // x, z 축을 0으로 설정하여 부드럽게 회전하기 위함
        tr.rotation = Quaternion.Slerp(tr.rotation, targetRotation, Time.deltaTime * 2.0f);

        Quaternion smoothRotation = Quaternion.Euler(Mathf.LerpAngle(tr.rotation.eulerAngles.x, 0, Time.deltaTime * 2.0f), tr.rotation.eulerAngles.y, Mathf.LerpAngle(tr.rotation.eulerAngles.z, 0, Time.deltaTime * 2.0f));
        tr.rotation = Quaternion.Slerp(tr.rotation, smoothRotation, Time.deltaTime * 2.0f);

    }
    private void ApacheMoveHorizontal()
    {
        if (moveApply)
        {
            if (moveSpeed < 0f) moveSpeed += 2 * ApplyMoveSpeed;
            else moveSpeed += 1 * ApplyMoveSpeed;
        }
        else if (moveApply)
        {
            if (moveSpeed > 0f) moveSpeed += -2 * ApplyMoveSpeed;
            else moveSpeed += -1 * ApplyMoveSpeed;
        }
        else
        {
            if (moveSpeed > 2f) moveSpeed += -2 * ApplyMoveSpeed;
            else if (moveSpeed > 0f) moveSpeed += -1 * ApplyMoveSpeed;
            else if (moveSpeed < -2f) moveSpeed += 2 * ApplyMoveSpeed;
            else if (moveSpeed < 0f) moveSpeed += 1 * ApplyMoveSpeed;
        }
        moveSpeed = Mathf.Clamp(moveSpeed, -15.0f, 15.0f);
        tr.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);
    }
    private void ApacheMoveVertical()
    {
        if (verticalApply == 1 && !isSearch)
        {
            if (verticalSpeed < 0f) verticalSpeed += 2 * ApplyVerticalSpeed;
            else verticalSpeed += ApplyVerticalSpeed;
        }
        else if (verticalApply == -1 && !isSearch)
        {
            if (verticalSpeed > 0f) verticalSpeed += -2 * ApplyVerticalSpeed;
            else verticalSpeed += -ApplyVerticalSpeed;
        }
        else
        {
            if (verticalSpeed > 0f) verticalSpeed += -ApplyVerticalSpeed;
            else if (verticalSpeed < 0f) verticalSpeed += ApplyVerticalSpeed;
        }
        tr.Translate(Vector3.up * verticalSpeed * Time.deltaTime, Space.Self);
    }

    private void Search()
    {
        TankFindDist = (GameObject.FindWithTag("Tank").transform.position - tr.position).magnitude;
        if (TankFindDist <= 75f)
            isSearch = true;
        else
            isSearch = false;
    }

    private void Attack()
    {
        if (isSearch)
        {
            targetDist = GameObject.FindWithTag("Tank").transform.position - tr.position;
            tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(targetDist.normalized), Time.deltaTime * 2.0f);
            isFire = true;
        }
        else
            isFire = false;
    }
    private bool isFire = false;
    private bool onFire = false;

    [PunRPC]
    IEnumerator Fire()
    {
        onFire = true;
        Ray ray = new Ray(tr.position, tr.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider.CompareTag(tankTag))
            {
                Instantiate(plasmaEffect, hit.point, Quaternion.identity);
                hit.collider.transform.parent.parent.SendMessage("OnDamage", SendMessageOptions.DontRequireReceiver);
            }
        }
        /* Instantiate(bullet, firePos[0].position, firePos[0].rotation);
        Instantiate(bullet, firePos[1].position, firePos[1].rotation); */

        yield return new WaitForSeconds(1f);
        onFire = false;
    }


}

