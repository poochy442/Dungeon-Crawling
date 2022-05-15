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

// Handles listening to player input
public class InputManager : MonoBehaviour
{
	#region Singleton

	public static InputManager instance;

	#endregion

    // Setup variables
	public ControlType CurrentControl {get; private set;}

	// Movement
	public bool IsMovementPressed {get; private set;}
	public Vector2 CurrentMovementInput {get; private set;}
	public bool IsRunPressed {get; private set;}
	public bool IsAttackPressed {get; private set;}
	public bool IsLookPressed {get; private set;}
	public bool IsInteractPressed {get; private set;}
	public Vector2 CurrentLookInput {get; private set;}

	// UI
	public bool isInventoryActive {get; private set;}

	private PlayerControls _playerControls;

	// Debug
	private float timer = 0f;

	void Awake()
	{
		// Singleton
		if(instance != null)
		{
			Debug.LogWarning("More that one instance of Input Manager!");
			return;
		}
		instance = this;

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

		// Setup interaction
		_playerControls.Controls.Interact.started += ctx => onInteractStarted(ctx);
		_playerControls.Controls.Interact.canceled += ctx => onInteractCanceled();

		// Setup UI
		_playerControls.Controls.Inventory.started += ctx => onInventoryStarted();
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

	// Interact handlers
	void onInteractStarted(InputAction.CallbackContext context){
		checkDevice(context.action.activeControl.device.name);

		IsInteractPressed = true;
	}

	void onInteractCanceled(){
		IsInteractPressed = false;
	}

	// UI handlers
	void onInventoryStarted(){
		isInventoryActive = !isInventoryActive;
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
		if(_playerControls != null)
			_playerControls.Disable();
	}
}
