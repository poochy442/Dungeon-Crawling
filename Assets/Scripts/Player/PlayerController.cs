using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MovementSpeed = 8f, RotationSpeed = 4f;
    public LayerMask _layerMask;
    private Animator _animator;
    private CharacterController _characterController;
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        handleMovement();
    }

    void handleMovement()
    {
        // Register inputs
        Vector3 movementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if(movementInput.magnitude > 1)
            movementInput.Normalize();
        
        Vector3 velocity = movementInput * MovementSpeed;

        // Look towards cursor
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask)){
            Vector3 direction = (hit.point - transform.position);
            direction.y = 0;
            direction.Normalize();

            transform.forward = Vector3.RotateTowards(transform.forward, direction, RotationSpeed * Time.deltaTime, 0);
        }

        // Set animation parameter
        _animator.SetFloat("VelocityFront", Vector3.Dot(transform.forward, velocity.normalized) * velocity.magnitude / MovementSpeed, 0.075f, Time.deltaTime);
        _animator.SetFloat("VelocitySide", Vector3.Dot(transform.right, velocity.normalized) * velocity.magnitude / MovementSpeed, 0.075f, Time.deltaTime);

        // Move
        _characterController.SimpleMove(velocity);
    }
}
