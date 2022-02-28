using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void Start(){
        transform.position = target.position + offset;
    }

    // Using LateUpdate so the movement of the camera happens after all other movement
    void LateUpdate()
    {
        transform.position = target.position + offset;
    }
}
