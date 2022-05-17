using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player action state - ready
// Base action state where player is ready to perform a new action
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
		if(Ctx.IsAttackPressed && !Ctx.IsInteractingWithHud){
			SwitchState(Factory.Attack());
		} else if(Ctx.IsSpellPressed && Time.time > Ctx.NextSpellTime && !Ctx.IsInteractingWithHud)
		{
			SwitchState(Factory.Spell());
		}
	}
}
