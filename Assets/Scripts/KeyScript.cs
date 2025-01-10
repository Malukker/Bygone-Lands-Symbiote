using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    private PlayerMovement playerScript;

    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerScript = FindObjectOfType<PlayerMovement>();

            playerScript.key = true;
            Destroy(gameObject);
        }
    }
}
