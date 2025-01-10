using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    float LifeTime = 4f;

    // Update is called once per frame
    void Update()
    {
        LifeTime -= Time.deltaTime;

        if (LifeTime < 0 )
        {
            Destroy(gameObject);
        }
    }
}
