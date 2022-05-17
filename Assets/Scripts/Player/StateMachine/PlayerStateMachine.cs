using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Main state machine.
// Handles base logic as well as storage of variables
// Manages and handles state logic
public class PlayerStateMachine : MonoBehaviour
{
    // Declare Reference variables
    CharacterController _characterController;
    Animator _animator;
    Camera _camera;

    // Hashes for animation getting/setting
    int _isWalkingHash, _isRunningHash, _isAttackingHash, _attackCountHash, _isCastingHash, _forwardMovementHash, _sideMovementHash, _attackRateHash;

    // Constants
    public float _rotationSpeed = 15.0f;
    public float _moveSpeed = 8f, _runMultiplier = 2.0f;

    // Layer masks used
    public LayerMask _backgroundMask, _wallMask, _enemyMask, _hudMask;

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

	// Spell variables
	public float _spellCooldown = 10f, _castSpeed = 0.5f;
	float _nextSpellTime = 0f;
	bool _isCasting = false;
	Image _spellCooldownImage;
	Text _spellCooldownText;
	public GameObject _fireball;

    // State variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

	// HUD Variables
	public Sprite[] attackSprites;
	private Image _attackImage;
	private Image _spellImage;
	private bool _isInteractingWithHud;

	// Getters and Setters
	public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
	public Animator Animator { get { return _animator; } set { _animator = value; }}
	public Coroutine CurrentAttackResetRoutine { get { return currentAttackResetRoutine; } set { currentAttackResetRoutine = value; }}
	public Vector2 CurrentMovementInput { get { return InputManager.instance ? InputManager.instance.CurrentMovementInput : Vector2.zero; }}
	public Vector2 CurrentLookInput { get { return InputManager.instance ? InputManager.instance.CurrentLookInput : Vector2.zero; }}
	public bool IsMovementPressed { get { return InputManager.instance ? InputManager.instance.IsMovementPressed : false; }}
	public bool IsLookPressed { get { return InputManager.instance ? InputManager.instance.IsLookPressed : false; }}
	public bool IsRunPressed { get { return InputManager.instance ? InputManager.instance.IsRunPressed : false; }}
	public bool IsAttackPressed { get { return InputManager.instance ? InputManager.instance.IsAttackPressed : false; }}
	public bool IsSpellPressed { get { return InputManager.instance ? InputManager.instance.IsSpellPressed : false; }}
	public bool IsAttacking {get { return _isAttacking; } set { _isAttacking = value; }}
	public int AttackCount { get { return _attackCount; } set { _attackCount = value; }}
	public int AttackRateHash {get { return _attackRateHash; }}
	public int AttackCountHash {get { return _attackCountHash; }}
	public int ForwardMovementHash { get { return _forwardMovementHash; }}
	public int SideMovementHash { get { return _sideMovementHash; }}
	public int IsWalkingHash {get { return _isWalkingHash; }}
	public int IsRunningHash {get { return _isRunningHash; }}
	public int IsAttackingHash {get { return _isAttackingHash; }}
	public int IsCastingHash {get { return _isCastingHash; }}
	public float AppliedMovementX { get { return _appliedMovement.x; } set { _appliedMovement.x = value; }}
	public float AppliedMovementZ { get { return _appliedMovement.z; } set { _appliedMovement.z = value; }}
	public float RunMultiplier { get { return _runMultiplier; }}
	public float NextAttackTime { get { return _nextAttackTime; } set { _nextAttackTime = value; }}
	public float NextSpellTime { get { return _nextSpellTime; } set { _nextSpellTime = value; }}
	public int AttackAmount { get { return _attackDurations.Keys.Count; }}
	public Image AttackImage {get { return _attackImage; } set { _attackImage = value; }}
	public Image SpellImage {get { return _spellImage; } set { _spellImage = value; }}
	public Text SpellText {get { return _spellCooldownText; } set { _spellCooldownText = value; }}
	public GameObject Fireball { get { return _fireball; }}
	public bool IsInteractingWithHud { get { return _isInteractingWithHud;}}

    void Awake()
    {
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
		_spellImage = GameObject.Find("SpellImage").GetComponent<Image>();

        // Set the hash references
        _isWalkingHash = Animator.StringToHash("IsWalking");
        _isRunningHash = Animator.StringToHash("IsRunning");
        _isAttackingHash = Animator.StringToHash("IsAttacking");
		_attackCountHash = Animator.StringToHash("AttackCount");
		_isCastingHash = Animator.StringToHash("IsCasting");
		_forwardMovementHash = Animator.StringToHash("MovementForward");
		_sideMovementHash = Animator.StringToHash("MovementSide");
		_attackRateHash = Animator.StringToHash("AttackRate");

        SetupAttack();
		SetupSpells();
    }

    // Initialize attack variables
    void SetupAttack()
    {
        // Calculate durations
        float firstDuration = 1.1f / _attackRate;
        float secondDuration = 2.667f / _attackRate;

        _attackDurations.Add(1, firstDuration);
        _attackDurations.Add(2, secondDuration);

		_attackTimings.Add(1, firstDuration / 5);
		_attackTimings.Add(2, secondDuration / 4);

		_attackImage.sprite = attackSprites[0];
    }

	// Initialize spell variables
	void SetupSpells()
	{
		_nextSpellTime = Time.time;
	}

    // Start is called before the first frame update
    void Start()
    {
		PlayerManager.instance.SetPlayer(gameObject);

		if(BSPDungeonGeneration.instance != null)
		{
			_characterController.enabled = false;
			PlayerManager.instance.MovePlayer(BSPDungeonGeneration.instance.PlayerSpawn);
			_characterController.enabled = true;
		}

		_spellCooldownImage = _spellImage.transform.GetChild(0).GetComponent<Image>();
		_spellCooldownImage.type = Image.Type.Filled;
		_spellCooldownText = _spellImage.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
		// Check if fell through terrain
		if(transform.position.y < -5)
		{
			GameManager.instance.Lose();
		}

		// Check if the user is interacting with the HUD
		if(InputManager.instance.CurrentControl == ControlType.KeyboardMouse){
			if(EventSystem.current.IsPointerOverGameObject())
				_isInteractingWithHud = true;
			else
				_isInteractingWithHud = false;
		}

		// Run state update logic
		_currentState.UpdateStates();

		// Handle update logic
        HandleRotation();
		handleWalls();

		// Set animation parameters
		if(_characterController.velocity.magnitude > 0){
			_animator.SetFloat(_forwardMovementHash,
				Vector3.Dot(transform.forward, _characterController.velocity.normalized),
				0.075f,
				Time.deltaTime);
			_animator.SetFloat(_sideMovementHash, 
				Vector3.Dot(transform.right, _characterController.velocity.normalized),
				0.075f,
				Time.deltaTime);
		}

		// Move
        _characterController.SimpleMove(_moveSpeed * _appliedMovement);

		// Handle interaction
		if(InputManager.instance.IsInteractPressed && PlayerManager.instance.currentTarget != null)
		{
			PlayerManager.instance.currentTarget.Interact();
		}

		// Update spell cooldown
		float remainingTime = Mathf.Max(_nextSpellTime - Time.time, 0);
		_spellCooldownImage.fillAmount = remainingTime / _spellCooldown;
		if(remainingTime <= 0)
			SpellText.text = "";
		else
			SpellText.text = remainingTime.ToString("F1");
    }

	// Handles the rotation of the character
    void HandleRotation()
    {
		if(InputManager.instance.CurrentControl == ControlType.Controller && IsLookPressed){
			if(CurrentLookInput.magnitude < 0.05f)
				return;
			
			// Debug.Log($"Controller look: x {CurrentLookInput.x}, y {CurrentLookInput.y}");
			Vector3 positionToLookAt = transform.position + new Vector3(CurrentLookInput.x * 100, 0, CurrentLookInput.y * 100);
			positionToLookAt.y = 0;

			Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
		} else if(InputManager.instance.CurrentControl == ControlType.KeyboardMouse) {
			// Look towards the mouse
			RaycastHit hit;
			Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
			if(Physics.Raycast(ray, out hit, 100f, _backgroundMask)){
				Vector3 direction = (hit.point - transform.position).normalized;
				direction.y = 0;

				// Create a new rotation based on where the cursor is located
				Quaternion targetRotation = Quaternion.LookRotation(direction);
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
