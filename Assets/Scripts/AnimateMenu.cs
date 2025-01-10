using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateMenu : MonoBehaviour
{
    private float time = 2f;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if (time < 0)
        {
            animator.SetTrigger("swap");
            time = 2f;
        }
    }
}
