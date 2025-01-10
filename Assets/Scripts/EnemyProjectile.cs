using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    public float lifeTime;
    public GameObject impact;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }

    // Update is called once per frame
    void Update()
    {
        LifeManager();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3 || collision.gameObject.layer == 10)
        {
            lifeTime = 0;
        }
        
    }

    void LifeManager()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0)
        {
            Instantiate(impact, transform.position, transform.rotation*Quaternion.Euler(0, 0, -90));
            Destroy(gameObject);
        }
    }
}
