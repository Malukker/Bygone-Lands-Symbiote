using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    private Transform playerTransform;
    private CircleCollider2D circleZone;
    public GameObject projectile;
    public Transform shootPoint;

    private bool following;
    private float cooldown;

    private void Start()
    {
        circleZone = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (following)
        {
            PointToPlayer(transform);
            ShootNLoad();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            following = true;
            playerTransform = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            following = false;
        }
    }

    void PointToPlayer(Transform selfTransform)
    {
        var playerPos = playerTransform.position;
        var startingScreenPos = selfTransform.position;
        playerPos.x -= startingScreenPos.x;
        playerPos.y -= startingScreenPos.y;
        var angle = Mathf.Atan2(playerPos.y, playerPos.x) * Mathf.Rad2Deg;
        selfTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void OnDrawGizmos()
    {
        if (circleZone == null)
        {
            circleZone = GetComponent<CircleCollider2D>();
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, circleZone.radius * transform.lossyScale.x);
    }

    void ShootNLoad()
    {
        if(GameObject.FindGameObjectWithTag("EnemyProjo") == null)
        {
            Instantiate(projectile, shootPoint.position, shootPoint.rotation);
        }
    }
}
