using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeShop : MonoBehaviour
{
    public GameObject shopUI;
    private Collider2D[] colls;

    // Update is called once per frame
    void Update()
    {
        ShopManager();
    }

    bool InCheckPointRange()
    {
        colls = Physics2D.OverlapCircleAll(transform.position, 2f);
        foreach (Collider2D c in colls)
        {
            if (IsCheckPoint(c))
            {
                return true;
            }
        }

        return false;
    }

    bool IsCheckPoint(Collider2D coll)
    {
        return coll.CompareTag("Tchèque poing");
    }

    void ShopManager() 
    { 
        if (InCheckPointRange())
        {
            shopUI.SetActive(true);
        }
        else
        {
            shopUI.SetActive(false);
        }
    }
}
