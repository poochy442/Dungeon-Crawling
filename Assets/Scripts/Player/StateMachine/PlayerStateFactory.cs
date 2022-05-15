using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// State factory.
// Handles creating the different states as well as injecting the context
public class PlayerStateFactory
{
    PlayerStateMachine _context;

	public PlayerStateFactory(PlayerStateMachine currentContext){
		_context = currentContext;
	}

	public PlayerBaseState Idle(){
		return new PlayerIdleState(_context, this);
	}
	public PlayerBaseState Walk(){
		return new PlayerWalkState(_context, this);
	}
	public PlayerBaseState Run(){
		return new PlayerRunState(_context, this);
	}
	public PlayerBaseState Ready(){
		return new PlayerReadyState(_context, this);
	}
	public PlayerBaseState Attack(){
		return new PlayerAttackState(_context, this);
	}

	public PlayerBaseState Spell(){
		return new PlayerSpellState(_context, this);
	}
}
