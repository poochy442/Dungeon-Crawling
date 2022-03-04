using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttackState : PlayerBaseState
{
	float _damageTime = 0;
	bool hasDamaged = false;
	Image cooldownImage;
	Text cooldownText;

	IEnumerator IAttackResetRoutine()
	{
		yield return new WaitForSeconds(1f);
		Ctx.AttackCount = 0;
		Ctx.AttackImage.sprite = Ctx.attackSprites[0];
	}

	public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory){
		IsRootState = true;
		InitializeSubState();
	}
	public override void EnterState()
	{
		cooldownImage = Ctx.AttackImage.transform.GetChild(0).GetComponent<Image>();
		cooldownImage.type = Image.Type.Filled;
		cooldownText = Ctx.AttackImage.GetComponentInChildren<Text>();

		Ctx.Animator.SetFloat(Ctx.AttackRateHash, Ctx._attackRate);
		if(Ctx.currentAttackResetRoutine != null)
			Ctx.StopCoroutine(Ctx.currentAttackResetRoutine);
		BeginAttack();
	}

	public override void UpdateState()
	{
		if(!hasDamaged && Time.time > _damageTime){
			HandleAttack();
			hasDamaged = true;
		}
		if(Time.time > Ctx.NextAttackTime){
			cooldownImage.fillAmount = 0;
			cooldownText.text = "";
		} else {
			var remainingTime = Ctx.NextAttackTime - Time.time;
			cooldownImage.fillAmount = remainingTime / Ctx._attackDurations[Ctx.AttackCount];
			cooldownText.text = remainingTime.ToString("F1");
		}
		CheckSwitchStates();
	}

	public override void ExitState()
	{
		Ctx.CurrentAttackResetRoutine = Ctx.StartCoroutine(IAttackResetRoutine());
		if(Ctx.AttackCount == 3){
			Ctx.AttackCount = 0;
		}
		Ctx.Animator.SetBool(Ctx.IsAttackingHash, false);
	}

	public override void InitializeSubState()
	{
		if(Ctx.IsMovementPressed){
			SetSubState(Factory.Walk());
		} else {
			SetSubState(Factory.Idle());
		}
	}

    public override void CheckSwitchStates()
	{
		bool nextAttack = Time.time > Ctx.NextAttackTime;
		if (nextAttack && Ctx.IsAttackPressed){
			BeginAttack();
		} else if (nextAttack){
			SwitchState(Factory.Ready());
		}
	}

	void BeginAttack() {
		if(Ctx.AttackCount == 3){
			Ctx.AttackCount = 1;
		} else {
			Ctx.AttackCount += 1;
		}
		Ctx.Animator.SetBool(Ctx.IsAttackingHash, true);
        Ctx.Animator.SetInteger(Ctx.AttackCountHash, Ctx.AttackCount);
		Ctx.NextAttackTime = Time.time + Ctx._attackDurations[Ctx.AttackCount];

		// Setup damage timer
		_damageTime = Time.time + Ctx._attackTimings[Ctx.AttackCount];
		hasDamaged = false;

		// Set attack sprite
		int i = Ctx.AttackCount >= 3 ? 0 : Ctx.AttackCount;
		Ctx.AttackImage.sprite = Ctx.attackSprites[i];
	}

	void HandleAttack() {
		// Detect enemies in range of attack
        // TODO: Delay detection for animation trigger
        Collider[] hitEnemies = Physics.OverlapSphere(Ctx.attackPoint.position, Ctx._attackRange, Ctx._enemyMask);

        // Damage enemies
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(Ctx._attackDamage);
        }
	}
}
