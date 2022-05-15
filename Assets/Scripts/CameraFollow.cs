using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3 Offset = new Vector3(0, 10, -10);
	public float HideStartDistance = 10f, HideFinishDistance = 4f;
	public LayerMask WallMask;
    private GameObject _target;

    void Start(){
		_target = GameObject.FindWithTag("Player");
        if(_target != null)
		{
			transform.position = _target.transform.position + Offset;
			transform.rotation = Quaternion.AngleAxis(45, new Vector3(1, 0, 0));
		}
    }

    // Using LateUpdate so the movement of the camera happens after all other movement
    void LateUpdate()
    {
		if(_target == null) _target = GameObject.FindWithTag("Player");
        else
		{
			transform.position = _target.transform.position + Offset;
			transform.rotation = Quaternion.AngleAxis(45, new Vector3(1, 0, 0));
		}
    }
}
