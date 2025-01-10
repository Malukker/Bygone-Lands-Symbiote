using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BondsManager : MonoBehaviour
{
    private float lifeTime;

    private void Start()
    {
        lifeTime = Time.deltaTime;
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0) 
        {
            Destroy(gameObject);
        }
    }
}
