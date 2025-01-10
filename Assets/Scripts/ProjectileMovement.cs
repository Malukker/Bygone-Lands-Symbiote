using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    [SerializeField] private LayerMask groundLayer, swingLayer, dashLayer, slowLayer, platformLayer, hazardLayer, climbLayer;
    private Hook hookScript;
    [SerializeField] private float decayTime;
    public GameObject impact;

    // Start is called before the first frame update
    void Start()
    {
        hookScript = FindObjectOfType<Hook>();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }

    // Update is called once per frame
    void Update()
    {
        LayerCheck();
        LifetimeCheck();
    }

    void LayerCheck() //Casts raycasts on the hook's position, checking the different layers that the grapple acts upon
    {
        RaycastHit2D terrainHit = Physics2D.Raycast
                (
                origin: transform.position,
                direction: Vector2.zero,
                distance: Mathf.Infinity,
                layerMask: groundLayer
                );

        if (terrainHit.collider != null)
        {
            decayTime = 0;
        }

        RaycastHit2D swingHit = Physics2D.Raycast
                (
                origin: transform.position,
                direction: Vector2.zero,
                distance: Mathf.Infinity,
                layerMask: swingLayer
                );

        if (swingHit.collider != null)
        {
            hookScript.swingCheck = true;
            hookScript.grapplePoint = new Vector2(swingHit.collider.bounds.center.x, swingHit.collider.bounds.min.y);
            Vector2 emitPoint = GameObject.FindGameObjectWithTag("M et Teur").transform.position;
            hookScript.swingDistance = Vector2.Distance(emitPoint, swingHit.point);
            decayTime = 0;
        }

        RaycastHit2D dashHit = Physics2D.Raycast
                (
                origin: transform.position,
                direction: Vector2.zero,
                distance: Mathf.Infinity,
                layerMask: dashLayer
                );

        if (dashHit.collider != null)
        {
            hookScript.dashCheck = true;
            hookScript.grapplePoint = new Vector2(dashHit.collider.bounds.center.x, dashHit.collider.bounds.min.y);
            decayTime = 0;
        }

        RaycastHit2D slowHit = Physics2D.Raycast
                (
                origin: transform.position,
                direction: Vector2.zero,
                distance: Mathf.Infinity,
                layerMask: slowLayer
                );

        if (slowHit.collider != null)
        {
            Vector2 emitPoint = GameObject.FindGameObjectWithTag("M et Teur").transform.position;
            hookScript.slowCheck = true;
            hookScript.slowDistance = Vector2.Distance(emitPoint, slowHit.point);
            hookScript.grapplePoint = new Vector2(slowHit.collider.bounds.center.x, slowHit.collider.bounds.min.y);
            decayTime = 0;
        }

        RaycastHit2D platformHit = Physics2D.Raycast
                (
                origin: transform.position,
                direction: Vector2.zero,
                distance: Mathf.Infinity,
                layerMask: platformLayer
                ) ;

        if (platformHit.collider != null)
        {
            hookScript.platformCheck = true;
            hookScript.grapplePoint = platformHit.rigidbody.position;
            decayTime = 0;
        }

        RaycastHit2D hazardHit = Physics2D.Raycast
                (
                origin: transform.position,
                direction: Vector2.zero,
                distance: Mathf.Infinity,
                layerMask: hazardLayer
                );

        if (hazardHit.collider != null)
        {
            hookScript.platformCheck = true;
            hookScript.grapplePoint = hazardHit.transform.position;
            decayTime = 0;
            hazardHit.collider.SendMessage("DrenchThemInRockNStones"); 
        }

        RaycastHit2D climbHit = Physics2D.Raycast
                (
                origin: transform.position,
                direction: Vector2.zero,
                distance: Mathf.Infinity,
                layerMask: climbLayer
                );

        if (climbHit.collider != null)
        {
            hookScript.climbCheck = true;
            hookScript.grapplePoint = climbHit.collider.gameObject.transform.GetChild(0).transform.position;
            hookScript.secondaryRopePoint = new Vector2(climbHit.collider.bounds.center.x, climbHit.collider.bounds.min.y);
            decayTime = 0;
        }
    }

    private void LifetimeCheck() //Manages the lifetime of the hook, and destroys it when it ends
    {
        decayTime -= Time.deltaTime;

        if (decayTime <= 0)
        {
            Instantiate(impact, transform.position, transform.rotation * Quaternion.Euler(0, -90, -90));
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision) //Destroys the hook upon touching an enemy, and destroy turrets if the upgrade was bought
    {
        if (collision.gameObject.tag == "RangedEnemy")
        {
            if (hookScript.IsTurretKillEnabled)
            {
                Destroy(collision.gameObject);
                decayTime = 0;
            }
        }
        else if (collision.gameObject.layer == 8 || collision.gameObject.layer == 11) 
        {
            decayTime = 0;
        }
    }
}
