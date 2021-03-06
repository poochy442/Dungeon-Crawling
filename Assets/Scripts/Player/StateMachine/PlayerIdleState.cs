using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player movement state - idle
public class PlayerIdleState : PlayerBaseState
{
	public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) {
		
	}
	public override void EnterState()
	{
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
		Ctx.Animator.SetBool(Ctx.IsRunningHash, false);

		Ctx.AppliedMovementX = 0;
		Ctx.AppliedMovementZ = 0;
	}

    public override void UpdateState()
	{
		Ctx.AppliedMovementX = 0;
		Ctx.AppliedMovementZ = 0;
		CheckSwitchStates();
	}

	public override void ExitState()
	{

	}

	public override void InitializeSubState()
	{

	}

    public override void CheckSwitchStates()
	{
		if(Ctx.IsMovementPressed && Ctx.IsRunPressed && !Ctx.IsAttacking){
			SwitchState(Factory.Run());
		} else if(Ctx.IsMovementPressed && !Ctx.IsAttacking){
			SwitchState(Factory.Walk());
		}
	}
}
