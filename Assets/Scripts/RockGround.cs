using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockGround : MonoBehaviour
{
    private Collider2D[] colls;
    private BoxCollider2D monColl;
    private Rigidbody2D rb;
    private GameObject killZone;

    // Start is called before the first frame update
    void Start()
    {
        killZone = gameObject.transform.GetChild(0).gameObject;
        monColl = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
    }

    void GroundCheck()
    {
        killZone.tag = "Kill";
        rb.bodyType = RigidbodyType2D.Dynamic;

        colls = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, monColl.bounds.min.y + 0.4f), monColl.bounds.size.x * 0.4f);

        foreach (Collider2D coll in colls)
        {
            if (coll != monColl && !coll.isTrigger)
            {
                rb.bodyType = RigidbodyType2D.Static;
                killZone.tag = "Untagged";
                break;
            }
        }
    }
}
