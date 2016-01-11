using UnityEngine;
using System.Collections;

public class Drawer : Building {
	
	public override void Awake()
	{
		base.Awake ();
		mBAttackState = new BuildingAttackState (this);
		mBIdleState = new BuildingIdleState (this);
		mBBuildingState = new BuildingBuildState (this);
	}
	
	public override void Start()
	{
		base.Start ();
		BCurrentState = mBBuildingState;
	}

	public override void Update()
	{
		base.Update ();
	}


	public override void FixedUpdate()
	{
		base.FixedUpdate ();
	}
	
	public override void TakeDamage(float damage)
	{
		base.TakeDamage (damage);
	}

	public override void ActiveBuildingUI(bool isactive)
	{
		base.ActiveBuildingUI (isactive);
	}

	public override bool CanAttack()
	{
		return false;
	}

	public override void Attack()
	{

	}

	public override bool IsTargetAvalibleToAttack()
	{
		return false;
	}
}
