using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReadyState : PlayerBaseState
{
    public PlayerReadyState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) {
		IsRootState = true;
		InitializeSubState();
	}
    public override void EnterState()
	{
		
	}

	public override void UpdateState()
	{
		CheckSwitchStates();
	}

	public override void ExitState()
	{

	}

	public override void InitializeSubState()
	{
		if(!Ctx.IsMovementPressed && !Ctx.IsRunPressed){
			SetSubState(Factory.Idle());
		} else if(Ctx.IsMovementPressed && !Ctx.IsRunPressed){
			SetSubState(Factory.Walk());
		} else {
			SetSubState(Factory.Run());
		}
	}

    public override void CheckSwitchStates()
	{
		if(Ctx.IsAttackPressed){
			SwitchState(Factory.Attack());
		}
	}
}
