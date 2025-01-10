using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDrop : MonoBehaviour
{
    [SerializeField] private GameObject key;

    private void Awake()
    {
        if (key != null)
        {
            key.SetActive(false);
        }
    }

    void Drop()
    {
        if (key != null)
        {
            key.SetActive(true);
        }
    }
}
