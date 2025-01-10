using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeHazardPlatformTrigger : MonoBehaviour
{
    public BoxCollider2D coll;
    [SerializeField] private SpriteRenderer skin;

    void DrenchThemInRockNStones()
    {
        skin.color = Color.grey;
        coll.enabled = false;
    }
}
