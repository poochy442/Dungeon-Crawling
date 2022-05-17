using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 3f;
	bool hasInteracted = false;

	public virtual void Interact(){
		if(hasInteracted) return;
		
		// Debug.Log("Interacting with " + transform.name);
		hasInteracted = true;
	}

	void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}
