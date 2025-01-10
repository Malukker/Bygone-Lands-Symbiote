using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Transform camTransform;
    private Vector3 earlyCamPos;
    private Vector3 lateCamPos;

    [SerializeField] private float parallaxSpeedScale;

    // Start is called before the first frame update
    void Start()
    {
        camTransform = GameObject.FindGameObjectWithTag("Player").transform;
        earlyCamPos = camTransform.position;
        lateCamPos = camTransform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        earlyCamPos = camTransform.position;
        Vector3 deltaMovement = new Vector3((earlyCamPos.x - lateCamPos.x)*parallaxSpeedScale, earlyCamPos.y - lateCamPos.y, 0);
        transform.position += deltaMovement ;
        lateCamPos = camTransform.position;
    }
}
