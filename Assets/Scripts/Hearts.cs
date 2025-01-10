using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hearts : MonoBehaviour
{
    [HideInInspector] public GameObject[] coeur;

    void Awake()
    {
        coeur = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            coeur[i] = transform.GetChild(i).gameObject;
        }
    }

    public void VieUpdate(int vie)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i < vie)
            {
                coeur[i].SetActive(true);
            }
            else
            {
                coeur[i].SetActive(false);
            }
        }
    }
}
