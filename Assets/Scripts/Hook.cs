using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField] private float dashDistance;
    [HideInInspector] public float swingDistance, slowDistance;
    [SerializeField] private LayerMask swingLayer;
    [SerializeField] private LayerMask dashLayer;
    [SerializeField] private LayerMask slowLayer;
    [SerializeField] private LineRenderer rope1;
    [SerializeField] private LineRenderer rope2;

    private Transform shootingPoint;
    private Vector3[] shootingPositions = 
        { 
            new Vector3(1.07200003f, 0.226999998f, 0),
            new Vector3(-1.07200003f, 0.226999998f, 0),
            new Vector3(1.17200005f, 1.39699996f, 0),
            new Vector3(-1.17200005f, 1.39699996f, 0)
        };
    private SpriteRenderer skin;

    [SerializeField] private GameObject bulletPrefab;
    private Rigidbody2D rb;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 secondaryRopePoint;
    private DistanceJoint2D joint;

    [HideInInspector] public bool swingCheck;
    [HideInInspector] public bool dashCheck;
    [HideInInspector] public bool slowCheck;
    [HideInInspector] public bool platformCheck;
    [HideInInspector] public bool hazardCheck;
    [HideInInspector] public bool climbCheck;

    [HideInInspector] public bool IsTurretKillEnabled;

    private PlayerMovement moveScript;

    private float ropeTime = 0.4f;

    public GameObject hud;
    public GameObject upgradeHud;

    // Start is called before the first frame update
    void Start()
    {
        skin = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        moveScript = GetComponent<PlayerMovement>();
        joint = gameObject.GetComponent<DistanceJoint2D>();
        shootingPoint = gameObject.transform.GetChild(0).transform;
        joint.enabled = false;
        rope1.enabled = false;
        rope2.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        GrappleCheck();
        ShootCheck();
        ShootPointPositionManager();
    }

    void GrappleCheck() //Manages the entire behaviour of the lineRenderers and distanceJoint, based on the grapple's mode
    {
        if ((swingCheck && !Input.GetMouseButton(0)) || (dashCheck && !Input.GetMouseButton(0)) ||
            (slowCheck && !Input.GetMouseButton(0)))
        {
            swingCheck = false; dashCheck = false;
        }

        //Manages the behaviour of the swinging mode, which makes the player swing on the grapple point
        else if (swingCheck && Input.GetMouseButton(0))
        {
            joint.connectedAnchor = grapplePoint;
            joint.enabled = true;
            joint.distance = swingDistance - 2f;
            rope1.enabled = true;
            rope1.SetPosition(0, grapplePoint);
            rope1.SetPosition(1, shootingPoint.position);
        }


        //Manages the behaviour of the dashing mode, which attracts the player close to the grapple point
        else if (dashCheck && Input.GetMouseButton(0))
        {
            joint.connectedAnchor = grapplePoint;
            joint.enabled = true;
            joint.distance = dashDistance;
            rope1.enabled = true;
            rope1.SetPosition(0, grapplePoint);
            rope1.SetPosition(1, shootingPoint.position);
        }


        //Manages the behaviour of the tracting mode, which allows the player to shorten or lenghten the rope on a tractor point
        else if (slowCheck && Input.GetMouseButton(0))
        {
            joint.connectedAnchor = grapplePoint;
            joint.enabled = true;
            joint.distance = slowDistance - 1.5f;
            rope1.enabled = true;
            rope1.SetPosition(0, grapplePoint);
            rope1.SetPosition(1, shootingPoint.position);
            if (slowDistance > 2.5f && Input.GetAxis("Vertical") > 0)
            {
                slowDistance -= Time.deltaTime * 2f;
            }

            if (Input.GetAxis("Vertical") < 0 && !moveScript.grounded)
            {
                slowDistance += Time.deltaTime * 2f;
            }
        }


        //Manages  the interactions with the moving platforms
        else if (platformCheck && Input.GetMouseButton(0))
        {
            rope1.enabled = true;
            rope1.SetPosition(0, grapplePoint);
            rope1.SetPosition(1, shootingPoint.position);
        }


        //Manages the interactions with the rock droppers
        else if (hazardCheck && Input.GetMouseButton(0))
        {
            rope1.enabled = true;
            rope1.SetPosition(0, grapplePoint);
            rope1.SetPosition(1, shootingPoint.position);
        }


        //Manages the behaviour of the climbing mode, which allows the player to quicly climb up the designated points
        else if (climbCheck)
        {
            joint.connectedAnchor = grapplePoint;
            joint.enabled = true;
            joint.distance = dashDistance-0.5f;
            rope1.enabled = true;
            rope1.SetPosition(0, secondaryRopePoint);
            rope1.SetPosition(1, shootingPoint.position);

            ropeTime -= Time.deltaTime;
            if (ropeTime < 0.2f)
            {
                rope1.enabled = false;
            }
            if (ropeTime < 0)
            {
                joint.enabled = false;
                ropeTime = 0.4f;
                rb.AddForce(Vector2.up * 100f);
                climbCheck = false;
            }
        }


        //Disables the grapple if the left click isn't held down
        if (!Input.GetMouseButton(0))
        {
            joint.enabled = false;
            rope1.enabled = false;
            swingCheck = false;
            dashCheck = false;
            slowCheck = false;
            platformCheck = false;
            hazardCheck = false;
        }


        //Assigns the rope's position to wherever it is supposed to be, if it is enabled
        if (rope1.enabled)
        {
            if (climbCheck)
            {
                rope1.SetPosition(0, secondaryRopePoint);
                rope1.SetPosition(1, shootingPoint.position);
            }
            else
            {
                rope1.SetPosition(0, grapplePoint);
                rope1.SetPosition(1, shootingPoint.position);
            }
        }
    }

    void ShootCheck() //Manages the firing of the hook's head, and makes the rope follow it
    {
        PointToMouse(shootingPoint);

        if (GameObject.FindGameObjectWithTag("Ouk Ed") == null && !upgradeHud.activeInHierarchy)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
                rope2.enabled = true;
                rope2.SetPosition(0, shootingPoint.position);
                rope2.SetPosition(1, shootingPoint.position);
            }
        }

        if (rope2.enabled && GameObject.FindGameObjectWithTag("Ouk Ed"))
        {
            rope2.SetPosition(0, shootingPoint.position);
            rope2.SetPosition(1, GameObject.FindGameObjectWithTag("Ouk Ed").transform.position);
        }
        else
        {
            rope2.enabled = false;
        }
    }

    void PointToMouse(Transform shootEmittor) //Points the shooting point to the mouse
    {
        var mouseScreenPos = Input.mousePosition;
        var startingScreenPos = Camera.main.WorldToScreenPoint(shootEmittor.position);
        mouseScreenPos.x -= startingScreenPos.x;
        mouseScreenPos.y -= startingScreenPos.y;
        var angle = Mathf.Atan2(mouseScreenPos.y, mouseScreenPos.x) * Mathf.Rad2Deg;
        shootEmittor.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void ShootPointPositionManager() //Keeps coherency between the shooting point's position and the animations
    {
        if (dashCheck || swingCheck || slowCheck || climbCheck)
        {
            if (skin.flipX)
            {
                shootingPoint.localPosition = shootingPositions[3];
            }
            else
            {
                shootingPoint.localPosition = shootingPositions[2];
            }
        }
        else
        {
            if (skin.flipX)
            {
                shootingPoint.localPosition = shootingPositions[1];
            }
            else
            {
                shootingPoint.localPosition = shootingPositions[0];
            }
        }
    }

    public void EnableTurretKill() //Enables the turret kill upgreade if it has been bought
    {
        if (hud.GetComponent<MoneyCount>().money >= 100)
        {
            IsTurretKillEnabled = true;
            hud.SendMessage("MoneyDown", 100);
        }
    }
}
