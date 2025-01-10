using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform[] points;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer skin;
    public GameObject blood;
    private Animator anim;
    public GameObject bonds;

    private bool ejecting;
    public bool stunned;
    private bool launched;
    private bool attacking;

    private int i;
    private float stunTime = 3f;
    private float direction;
    private float lifeTimeAsProjo = 2f;
    private float aggroTime;

    private Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 
        skin = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        transform.position = points[0].position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveManager();
        StunManager();
        AttackManager();
        AnimManager();
    }

    void MoveManager() //Manages the different behaviours of the crane demon
    {
        //Manages the patrol movement
        if (!stunned && !attacking)
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

            if (points[i].position.x > transform.position.x)
            {
                direction = 1f;
                skin.flipX = true;
            }
            else if (points[i].position.x < transform.position.x)
            {
                direction = -1f;
                skin.flipX = false;
            }
        }

        else
        {
            //Manages the crane demon's movement upon being ejected by the player
            if (ejecting)
            {
                gameObject.tag = "Projo";
                rb.freezeRotation = false;
                PointToMouse(transform);
                rb.AddForce(transform.right * 1000f);
                rb.AddForce(Vector2.up * 250f);
                rb.AddTorque(direction * 50f, ForceMode2D.Force);
                launched = true;
                ejecting = false;
            }

            //Manages the crane demon's lifetime during its ejection
            if (launched)
            {
                lifeTimeAsProjo -= Time.deltaTime;
                if (lifeTimeAsProjo < 0) { Sterben(); }
            }
        }
    }

    void StunManager() //Manages the crane demon's behaviour when stunned by the hook's head
    {
        if (stunned)
        {
            Instantiate(bonds, transform.position, transform.rotation);
            if (Input.GetMouseButtonDown(1))
            {
                ejecting = true;
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.velocity = Vector2.zero;
            }

            stunTime -= Time.deltaTime;

            if (stunTime < 0)
            {
                animator.enabled = true;
                stunned = false;
                stunTime = 3f;
            }
        }
    }

    void AttackManager() //Manages the aggression behaviour, and attack behaviour, of the crane demon towards the player
    {
        if (Vector3.Distance(transform.position, playerTransform.position) < 7f && !stunned && !launched && !ejecting)
        {
            attacking = true;
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
            aggroTime = 2f;
        }
        else
        {
            aggroTime -= Time.deltaTime;
            if (aggroTime < 0)
            {
                attacking = false;
            }
        }
    }

    void AnimManager() //Makes an exclamation mark appear alongside the usual animation, if the crane demon is aggroed
    {
        anim.SetBool("Aggroed", attacking);
    }

    void PointToMouse(Transform selfTransform) //Points the crane demon to the mouse when it is ejected by the player
    {
        var mouseScreenPos = Input.mousePosition;
        var startingScreenPos = Camera.main.WorldToScreenPoint(selfTransform.position);
        mouseScreenPos.x -= startingScreenPos.x;
        mouseScreenPos.y -= startingScreenPos.y;
        var angle = Mathf.Atan2(mouseScreenPos.y, mouseScreenPos.x) * Mathf.Rad2Deg;
        selfTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void OnTriggerEnter2D(Collider2D collision) //Stuns the crane demon or kills it if it meets the hook's head or a damaging object
    {
        if (collision.gameObject.CompareTag("Ouk Ed"))
        {
            stunned = true;

            animator.enabled = false;

            rb.velocity = Vector2.zero;
        }

        if (collision.gameObject.CompareTag("Projo"))
        {
            collision.gameObject.SendMessage("Sterben");
            Sterben();
        }

        if (CompareTag("Projo"))
        {
            if (collision.gameObject.layer == 3)
            {
                Sterben();
            }
        }
    }

    void Sterben() //Kills the demon, instantiates a blood effect, and adds money to the player
    {
        Instantiate(blood, transform.position, Quaternion.LookRotation(Vector3.right));
        GameObject.Find("CanvasHud").SendMessage("MoneyUp", 1);
        if (GetComponent<KeyDrop>() != null)
        {
            this.gameObject.SendMessage("Drop");
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos() //Draws a gizmos that represents the demon's aggression range
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 7f);
    }
}
