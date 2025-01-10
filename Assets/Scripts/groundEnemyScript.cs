using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.XR.GoogleVr;
using UnityEngine;
using UnityEngine.UIElements;

public class GroundEnemyScript : MonoBehaviour
{
    //METTRE CE SCRIPT SUR LES ENNEMIS

    readonly private float _speed = 4f;                                            // Vitesse de déplacement ennemi
    [SerializeField, Range(0.1f, 50f)] private float limiteDroite = 1f; // distance entre l'ennemi et la limite de patrouille à droite (limité entre 0.1 et 50)
    [SerializeField, Range(0.1f, 50f)] private float limiteGauche = 1f; // distance entre l'ennemi et la limite de patrouille à gauche (limité entre 0.1 et 50)
    private Vector3 limiteDroitePosition;                               // Sert a transformer la distance avec la limite droite en coordonnées X/Y/Z
    private Vector3 limiteGauchePosition;                               // Sert a transformer la distance avec la limite gauche en coordonnées X/Y/Z
    private Rigidbody2D rb;                                             // Le rigidbody de l'ennemi
    private float direction = 1f;                                       // Direction vers laquelle l'ennemi se dirige (1 = droite, -1 = gauche)
    private SpriteRenderer skin;                                        // Le sprite de l'ennemi, pour qu'on puisse le retourner quand il change de direction
    private Animator animator;
    public GameObject blood;
    private Vector3 playerPos;
    public GameObject bonds;

    private float stunTime = 3f;
    private float lifeTimeAsProjo = 2f;

    public bool stunned;
    private bool ejecting;
    private bool launched;
    private bool attacking;
    private bool dashing = true;
    private bool enableDashAnim;


    // Au lancement du jeu, on enregistre le rigidbody et le sprite de l'ennemi
    // On transforme aussi les valeurs de limite Droite et Gauche en coordonnées réelles
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        skin = GetComponent<SpriteRenderer>();

        limiteDroitePosition = transform.position + new Vector3(limiteDroite, 0, 0);
        limiteGauchePosition = transform.position - new Vector3(limiteGauche, 0, 0);
    }


    void Update()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        PathManager();
        AttackManager();
        StunManager();
        Animate();
    }

    void PathManager() //Manages the different behaviours for the kobold's movement
    {
        //Manages the usual path movement of the player
        if (!stunned && !attacking && !launched)
        {
            if (Mathf.Abs(rb.velocity.x) < 0.1f && _speed != 0)
            {
                direction = -direction;
            }

            if (transform.position.x > limiteDroitePosition.x)
            {
                direction = -1f;
            }

            if (transform.position.x < limiteGauchePosition.x)
            {
                direction = 1f;
            }

            if (direction == 1f)
            {
                skin.flipX = false;
            }

            if (direction == -1f)
            {
                skin.flipX = true;
            }

            rb.velocity = new Vector2(direction * _speed, rb.velocity.y);
        }

        else
        { 
            //Manages the ejection of the kobold by the player
            if (ejecting)
            {
                gameObject.tag = "Projo";
                rb.freezeRotation = false;
                PointToMouse(transform);
                rb.AddForce(transform.right * 1000f);
                rb.AddForce(Vector2.up*250f);
                rb.AddTorque(direction * 50f, ForceMode2D.Force);
                launched = true;
                ejecting = false;
            }
            
            //Manages the lifetime of the kobold when launched
            if (launched) 
            {
                lifeTimeAsProjo -= Time.deltaTime;
                if (lifeTimeAsProjo < 0) { Sterben(); }
            }
        }
    }

    void StunManager() //Manages the stunning of the kobold by the player's hook's head
    {
        if (stunned)
        {
        Instantiate(bonds, transform.position, transform.rotation);
            if (Input.GetMouseButtonDown(1))
            {
                ejecting = true;
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

    void AttackManager() //Manages the kobold's aggression, and calls its dash attack
    {
        if (Vector3.Distance(transform.position, playerPos) < 9f && !stunned && !ejecting && !launched)
        {
            attacking = true;

            if (dashing)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                StartCoroutine(Dash());
            }
        }
        else
        {
            attacking = false;
        }
    }

    IEnumerator Dash() //Manages the dash attack
    {
        dashing = false;
        yield return new WaitForSeconds(.75f);
        if (!stunned)
        {
            rb.velocity = (playerPos - transform.position) * 3f;
            enableDashAnim = true;
        }
        yield return new WaitForSeconds(.75f);
        dashing = true;
        enableDashAnim = false;
    }

    void PointToMouse(Transform selfTransform) //Points the kobold to the mouse cursor upon being ejected by the player
    {
        var mouseScreenPos = Input.mousePosition;
        var startingScreenPos = Camera.main.WorldToScreenPoint(selfTransform.position);
        mouseScreenPos.x -= startingScreenPos.x;
        mouseScreenPos.y -= startingScreenPos.y;
        var angle = Mathf.Atan2(mouseScreenPos.y, mouseScreenPos.x) * Mathf.Rad2Deg;
        selfTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void Animate() //Changes the animation, between its aggression state and its attack state
    {
        animator.SetBool("aggroing", attacking && !enableDashAnim);
        animator.SetBool("dashing", enableDashAnim);
    }

    void OnDrawGizmos() //Draws gizmos for the patrol's path and the agression range
    {
        if (!Application.IsPlaying(gameObject))
        {
            limiteDroitePosition = transform.position + new Vector3(limiteDroite, 0, 0);
            limiteGauchePosition = transform.position - new Vector3(limiteGauche, 0, 0);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawCube(limiteDroitePosition, new Vector3(0.2f, 1, 0.2f));
        Gizmos.DrawCube(limiteGauchePosition, new Vector3(0.2f, 1, 0.2f));
        Gizmos.DrawLine(limiteDroitePosition, limiteGauchePosition);

        Gizmos.DrawWireSphere(transform.position, 9f);
    }

    private void OnTriggerEnter2D(Collider2D collision) //Calls the kill function for the kobold or stuns it, if it meets damaging objects or the hook's head
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

        if(CompareTag("Projo"))
        {
            if (collision.gameObject.layer == 3)
            {
                Sterben();
            }
        }
    }

    void Sterben() //Kills the kobold, instantiates a blood effect, and adds money to the player
    {
        Instantiate(blood, transform.position, Quaternion.LookRotation(Vector3.right));
        if (GetComponent<KeyDrop>() != null)
        {
            this.gameObject.SendMessage("Drop");
        }
        GameObject.Find("CanvasHud").SendMessage("MoneyUp", 1);
        Destroy(gameObject);
    }
}