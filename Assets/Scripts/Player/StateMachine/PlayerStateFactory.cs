using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateFactory
{
    PlayerStateMachine _context;
	private PlayerBaseState idleState = null;
	private PlayerBaseState walkState = null;
	private PlayerBaseState runState = null;
	private PlayerBaseState readyState = null;
	private PlayerBaseState attackState = null;

	public PlayerStateFactory(PlayerStateMachine currentContext){
		_context = currentContext;
	}

	public PlayerBaseState Idle(){
		/* if(idleState == null){
			idleState = new PlayerIdleState(_context, this);
		}
		return idleState; */
		return new PlayerIdleState(_context, this);
	}
	public PlayerBaseState Walk(){
		/* if(walkState == null){
			walkState = new PlayerWalkState(_context, this);
		}
		return walkState; */
		return new PlayerWalkState(_context, this);
	}
	public PlayerBaseState Run(){
		/* if(runState == null){
			runState = new PlayerRunState(_context, this);
		}
		return runState; */
		return new PlayerRunState(_context, this);
	}
	public PlayerBaseState Ready(){
		/* if(readyState == null){
			readyState = new PlayerReadyState(_context, this);
		}
		return readyState; */
		return new PlayerReadyState(_context, this);
	}
	public PlayerBaseState Attack(){
		/* if(attackState == null){
			attackState = new PlayerAttackState(_context, this);
		}
		return attackState; */
		return new PlayerAttackState(_context, this);
	}
}
