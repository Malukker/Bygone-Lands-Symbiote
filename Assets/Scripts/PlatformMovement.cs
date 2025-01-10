using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform[] points;

    private int i;
    private Hook hookScript;
    private bool boarded;
    // Start is called before the first frame update
    void Start()
    {
        hookScript = FindObjectOfType<Hook>();
        transform.position = points[0].position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveManager();
    }

    void MoveManager()
    {
        if (hookScript.platformCheck && boarded)
        {
            if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
            {
                i++;
                if (i == points.Length)
                {
                    i = 0;
                }
            }

            transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
            hookScript.grapplePoint = transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.isTrigger)
        {
            collider.transform.SetParent(transform);
            boarded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (!collider.isTrigger)
        {
            collider.transform.SetParent(null);
            boarded = false;
        }
    }
}
