using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepletedHearts : MonoBehaviour
{
    [HideInInspector] public GameObject[] coeurVide;

    void Awake()
    {
        coeurVide = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            coeurVide[i] = transform.GetChild(i).gameObject;
        }
    }
    
    public void FairePlace(int vieMax)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i < vieMax)
            {
                coeurVide[i].SetActive(true);
            }
            else
            {
                coeurVide[i].SetActive(false);
            }
        }
    }
}
