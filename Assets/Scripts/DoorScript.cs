using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private BoxCollider2D boxColl;
    private SpriteRenderer spriteRenderer;

    private PlayerMovement playerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = FindObjectOfType<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxColl = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (playerScript.key) 
            {
                boxColl.enabled = false;
                spriteRenderer.enabled = false;
                playerScript.key = false;
            }
        }
    }
}
