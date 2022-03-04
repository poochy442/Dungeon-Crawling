using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerStateMachine : MonoBehaviour
{
    // Declare Reference variables
    CharacterController _characterController;
    Animator _animator;
    Camera _camera;

    // Hashes for animation getting/setting
    int _isWalkingHash, _isRunningHash, _isAttackingHash, _attackCountHash, _forwardMovementHash, _sideMovementHash, _attackRateHash;

    // Constants
    public float _rotationSpeed = 15.0f;
    public float _moveSpeed = 8f, _runMultiplier = 2.0f;

    // Layer masks used
    public LayerMask _terrainMask, _wallMask, _enemyMask;

    // Movement variables
    Vector3 _currentMovement;
    Vector3 _appliedMovement;

    // Attack variables
	public Transform attackPoint;
    public float _attackRate = 2.0f, _attackDamage = 40f, _attackRange = 2f;
	float _nextAttackTime = 0f;
    bool _isAttacking = false;
    int _attackCount = 0;
    public Dictionary<int, float> _attackDurations = new Dictionary<int, float>();
	public Dictionary<int, float> _attackTimings = new Dictionary<int, float>();
    public Coroutine currentAttackResetRoutine = null;

    // State variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;
	InputManager _inputManager;

	// HUD Variables
	public Sprite[] attackSprites;
	private Image _attackImage;

	// Getters and Setters
	public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
	public Animator Animator { get { return _animator; } set { _animator = value; }}
	public Coroutine CurrentAttackResetRoutine { get { return currentAttackResetRoutine; } set { currentAttackResetRoutine = value; }}
	public Vector2 CurrentMovementInput { get { return _inputManager.CurrentMovementInput; }}
	public Vector2 CurrentLookInput { get { return _inputManager.CurrentLookInput; }}
	public bool IsMovementPressed { get { return _inputManager.IsMovementPressed; }}
	public bool IsLookPressed { get { return _inputManager.IsLookPressed; }}
	public bool IsRunPressed { get { return _inputManager.IsRunPressed; }}
	public bool IsAttackPressed { get { return _inputManager.IsAttackPressed; }}
	public bool IsAttacking {get { return _isAttacking; } set { _isAttacking = value; }}
	public int AttackCount { get { return _attackCount; } set { _attackCount = value; }}
	public int AttackRateHash {get { return _attackRateHash; }}
	public int AttackCountHash {get { return _attackCountHash; }}
	public int ForwardMovementHash { get { return _forwardMovementHash; }}
	public int SideMovementHash { get { return _sideMovementHash; }}
	public int IsWalkingHash {get { return _isWalkingHash; }}
	public int IsRunningHash {get { return _isRunningHash; }}
	public int IsAttackingHash {get { return _isAttackingHash; }}
	public float AppliedMovementX { get { return _appliedMovement.x; } set { _appliedMovement.x = value; }}
	public float AppliedMovementZ { get { return _appliedMovement.z; } set { _appliedMovement.z = value; }}
	public float RunMultiplier { get { return _runMultiplier; }}
	public float NextAttackTime { get { return _nextAttackTime; } set { _nextAttackTime = value; }}
	public Image AttackImage {get { return _attackImage; } set { _attackImage = value; }}

    void Awake()
    {
		// Add input manager
		_inputManager = gameObject.AddComponent<InputManager>();

        // Get Reference variables
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _camera = Camera.main;

        // Setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Ready();
        _currentState.EnterState();

		// Setup HUD
		_attackImage = GameObject.Find("AttackImage").GetComponent<Image>();

        // Set the hash references
        _isWalkingHash = Animator.StringToHash("IsWalking");
        _isRunningHash = Animator.StringToHash("IsRunning");
        _isAttackingHash = Animator.StringToHash("IsAttacking");
		_attackCountHash = Animator.StringToHash("AttackCount");
		_forwardMovementHash = Animator.StringToHash("MovementForward");
		_sideMovementHash = Animator.StringToHash("MovementSide");
		_attackRateHash = Animator.StringToHash("AttackRate");

        SetupAttack();
    }

    // Initialize attack variables
    void SetupAttack()
    {
        // Calculate durations
        float firstDuration = 1.1f / _attackRate;
        float secondDuration = 2.667f / _attackRate;
        float thirdDuration = 5.4f / _attackRate;

        _attackDurations.Add(1, firstDuration);
        _attackDurations.Add(2, secondDuration);
        _attackDurations.Add(3, thirdDuration);

		_attackTimings.Add(1, firstDuration / 5);
		_attackTimings.Add(2, secondDuration / 4);
		_attackTimings.Add(3, thirdDuration / 4);

		_attackImage.sprite = attackSprites[0];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		// Run state update logic
		_currentState.UpdateStates();

		// Handle update logic
        HandleRotation();
		handleWalls();

		// Set animation parameters
		if(_characterController.velocity.magnitude > 0){
			_animator.SetFloat(_forwardMovementHash,
				Vector3.Dot(transform.forward, _characterController.velocity.normalized)
				/ _characterController.velocity.magnitude,
				0.075f,
				Time.deltaTime);
			_animator.SetFloat(_sideMovementHash, 
				Vector3.Dot(transform.right, _characterController.velocity.normalized)
				/ _characterController.velocity.magnitude,
				0.075f,
				Time.deltaTime);
		}

		// Move
        _characterController.SimpleMove(_moveSpeed * _appliedMovement);

		// Log current state
		// Debug.Log("Current state" + _currentState.GetCurrentStatesPrint());
    }

	// Handles the rotation of the character
    void HandleRotation()
    {
		if(_inputManager.CurrentControl == ControlType.Controller && IsLookPressed){
			Vector3 positionToLookAt = transform.position + new Vector3(CurrentLookInput.x, 0, CurrentLookInput.y) * 100;
			positionToLookAt.y = 0;

			Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
		} else if(_inputManager.CurrentControl == ControlType.KeyboardMouse) {
			// Look towards the mouse
			RaycastHit hit;
			Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, _terrainMask)){
				Vector3 positionToLookAt = hit.point;
				positionToLookAt.y = 0;

				// Create a new rotation based on where the cursor is located
				Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
				// Rotate the character to face the position
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
			}
		}
        
    }

	// Handle setting walls between character and camera transparent
	void handleWalls()
	{
		float hideStartDistance = 6f;
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 8f, _wallMask);
		foreach (var hit in hitColliders){
			if(hit.transform.position.z >= transform.position.z // Make sure it is in the cameras direction
				|| Vector3.Angle(hit.transform.forward, new Vector3(1, 0, 0)) < 5f // Not perpendicular (+ direction)
				|| Vector3.Angle(hit.transform.forward, new Vector3(-1, 0, 0)) < 5f // Not perpendicular (- direction)
			){
				continue;
			}
			
			// Set transparent based on distance to target
			Color newColor = hit.gameObject.GetComponent<Renderer>().material.color;
			float distance = (hit.transform.position - transform.position).magnitude;
			if(distance < hideStartDistance){
				newColor.a = 0.2f;
			} else {
				newColor.a = 1;
			}
			hit.gameObject.GetComponent<Renderer>().material.color = newColor;
		}
	}
}
