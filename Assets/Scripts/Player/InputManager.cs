using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public enum ControlType{
	KeyboardMouse,
	Controller
}

public class InputManager : MonoBehaviour
{
    // Setup variables
	public ControlType CurrentControl {get; private set;}
	public bool IsMovementPressed {get; private set;}
	public Vector2 CurrentMovementInput {get; private set;}
	public bool IsRunPressed {get; private set;}
	public bool IsAttackPressed {get; private set;}
	public bool IsLookPressed {get; private set;}
	public Vector2 CurrentLookInput {get; private set;}


	private PlayerControls _playerControls;

	// Debug
	private float timer = 0f;

	void Awake()
	{
		_playerControls = new PlayerControls();

		// Setup movement
		_playerControls.Controls.Movement.performed += ctx => onMovementPerformed(ctx);
		_playerControls.Controls.Movement.canceled += ctx => onMovementCanceled();

		// Setup run
		_playerControls.Controls.Run.started += ctx => onRunStarted(ctx);
		_playerControls.Controls.Run.canceled += ctx => onRunCanceled();

		// Setup look
		_playerControls.Controls.Look.performed += ctx => onLookPerformed(ctx);
		_playerControls.Controls.Look.canceled += ctx => onLookCanceled();

		// Setup attack
		_playerControls.Controls.Attack.started += ctx => onAttackStarted(ctx);
		_playerControls.Controls.Attack.canceled += ctx => onAttackCanceled();

	}

	/* Action handlers */
	// Movement handlers
	void onMovementPerformed(InputAction.CallbackContext context){
		checkDevice(context.action.activeControl.device.name);
		
		// Set value
		CurrentMovementInput = context.ReadValue<Vector2>();

        if(CurrentMovementInput.magnitude != 0){
			IsMovementPressed = true;
		} else {
			IsMovementPressed = false;
			CurrentMovementInput = Vector2.zero;
		}
	}

	void onMovementCanceled(){
		IsMovementPressed = false;
		CurrentMovementInput = Vector2.zero;
	}

	// Look handlers
	void onLookPerformed(InputAction.CallbackContext context){
		checkDevice(context.action.activeControl.device.name);

		// Set value
		CurrentLookInput = context.ReadValue<Vector2>();

		if(CurrentLookInput.magnitude != 0){
			IsLookPressed = true;
		} else {
			IsLookPressed = false;
			CurrentLookInput = Vector2.zero;
		}
	}
	void onLookCanceled(){
		IsLookPressed = false;
		CurrentLookInput = Vector2.zero;
	}

	// Run handlers
	void onRunStarted(InputAction.CallbackContext context){
		checkDevice(context.action.activeControl.device.name);
		
		IsRunPressed = true;
	}
	void onRunCanceled(){
		IsRunPressed = false;
	}

	// Attack handlers
	void onAttackStarted(InputAction.CallbackContext context){
		checkDevice(context.action.activeControl.device.name);
		
		IsAttackPressed = true;
	}
	void onAttackCanceled(){
		IsAttackPressed = false;
	}

	// Checks device
	void checkDevice(string deviceName){
		if(deviceName.Equals("Keyboard") || deviceName.Equals("Mouse")){
			if(!(CurrentControl == ControlType.KeyboardMouse)){
				CurrentControl = ControlType.KeyboardMouse;
			}
		} else {
			if(!(CurrentControl == ControlType.Controller)){
				CurrentControl = ControlType.Controller;
			}
		}
	}

	// Enable / Disable the control mapping along with this script
	void OnEnable()
	{
		_playerControls.Enable();
	}
	void OnDisable()
	{
		_playerControls.Disable();
	}
}
