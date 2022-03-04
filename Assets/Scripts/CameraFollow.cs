using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3 Offset = new Vector3(0, 5, -5);
	public float HideStartDistance = 10f, HideFinishDistance = 4f;
	public LayerMask WallMask;
    private GameObject _target;

    void Start(){
		_target = GameObject.FindWithTag("Player");
        transform.position = _target.transform.position + Offset;
    }

    // Using LateUpdate so the movement of the camera happens after all other movement
    void LateUpdate()
    {
        transform.position = _target.transform.position + Offset;
    }
}
