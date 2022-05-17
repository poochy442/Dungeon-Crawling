using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Player action state - spell
// Handles player spell cooldown and casting logic
public class PlayerSpellState : PlayerBaseState
{
	float _castingTime = 0;
	bool hasCast = false;

	public PlayerSpellState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory){
		IsRootState = true;
		InitializeSubState();
	}
	public override void EnterState()
	{
		CastSpell();
	}

	public override void UpdateState()
	{
		if(!hasCast && Time.time > _castingTime){
			HandleSpell();
			hasCast = true;
		} else if(hasCast)
		{
			CheckSwitchStates();
		}
	}

	public override void ExitState()
	{
		
	}

	public override void InitializeSubState()
	{
		SetSubState(Factory.Idle());
	}

    public override void CheckSwitchStates()
	{
		bool nextSpell = Time.time > Ctx.NextSpellTime;
		if (nextSpell && Ctx.IsSpellPressed && !Ctx.IsInteractingWithHud){
			CastSpell();
		} else {
			Ctx.IsAttacking = false;
			SwitchState(Factory.Ready());
		}
	}

	void CastSpell() {
		Ctx.Animator.SetTrigger(Ctx.IsCastingHash);
		Ctx.NextSpellTime = Time.time + Ctx._spellCooldown;

		// Setup casting timer
		_castingTime = Time.time + (1 / Ctx._castSpeed);
		hasCast = false;
		Ctx.IsAttacking = true;
	}

	void HandleSpell() {
		Vector3 offset = Ctx.transform.forward + new Vector3(0, 1, 0);
		GameObject f = GameObject.Instantiate(Ctx.Fireball, Ctx.transform.position + offset, Quaternion.identity);
		f.transform.forward = Ctx.transform.forward;
	}
}
