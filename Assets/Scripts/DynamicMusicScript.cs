using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class DynamicMusicScript : MonoBehaviour
{
    private AudioSource sound1, sound2;
    private int situation = 0;

    private void Start()
    {
        sound1 = gameObject.transform.GetChild(0).GetComponent<AudioSource>();
        sound2 = gameObject.transform.GetChild(1).GetComponent<AudioSource>();

        sound2.volume = 0.0001f;
    }

    void SituationCheck(int enemies)
    {
        if (enemies == 0)
        {
            if(situation != 0)
            {
                Swap();
                situation = 0;
            }
        }
        else
        {
            if (situation != 1)
            {
                Swap();
                situation = 1;
            }
        }
    }

    void Swap()
    {
        if (sound2.volume < 1)
        {
            sound1.volume = 0.0001f;
            sound2.volume = 1;
        }
        else
        {
            sound1.volume = 1;
            sound2.volume = 0.0001f;
        }
    }
}
