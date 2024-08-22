using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSelfDestroy : MonoBehaviour
{
    private Transform tr;

    void Start()
    {
        tr = transform;
        Invoke("Destroy", 3f);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

}
