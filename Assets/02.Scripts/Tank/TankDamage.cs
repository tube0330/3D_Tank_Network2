using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// Tank hp가 0 이하일 때 잠시 meshRenderer를 비활성화하고 5초 후 다시 활성화

public class TankDamage : MonoBehaviourPun
{
    public MeshRenderer[] meshRenderer;
    public GameObject explosionPrefab;
    private int InitialHp = 100;            // 초기 hp
    public int hp = 0;               // 현재 hp
    private readonly string playerTag = "Tank";
    public Canvas hudCanvas;
    public Image hpBar;

    void Start()
    {
        meshRenderer = GetComponentsInChildren<MeshRenderer>();
        explosionPrefab = Resources.Load<GameObject>("Explosion");
        hp = InitialHp;
        hpBar.color = Color.green;
    }

    public void OnDamage()
    {
        if (photonView.IsMine)
            photonView.RPC(nameof(OnDamagePun), RpcTarget.All);
    }

    [PunRPC]
    public void OnDamagePun()
    {
        if (hp > 0)
        {
            hp -= 10;
            HpBarUpdate();
            if (hp <= 0)
                StartCoroutine(ExplosionTank());
        }
    }

    private void HpBarUpdate()          // hp바 ui갱신
    {
        hpBar.fillAmount = (float)hp / InitialHp;
        if (hpBar.fillAmount < 0.3f)
            hpBar.color = Color.red;
        else if (hpBar.fillAmount < 0.6f)
            hpBar.color = Color.yellow;
        else
            hpBar.color = Color.green;
    }

    private IEnumerator ExplosionTank()
    {
        Object expEff = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(expEff, 2.0f);
        SetTankVisible(false);
        hudCanvas.enabled = false;
        yield return new WaitForSeconds(5.0f);
        hp = InitialHp;
        SetTankVisible(true);
        hudCanvas.enabled = true;
        HpBarUpdate();
    }

    private void SetTankVisible(bool isVisible)
    {
        foreach (var mesh in meshRenderer)
            mesh.enabled = isVisible;
    }
}
