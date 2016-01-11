using UnityEngine;
using System.Collections;

public class SoldierAttackState : SoldierState {

	private Soldier mSoldier; 

	public SoldierAttackState(Soldier soldier)
	{
		mSoldier = soldier;
	}

	public void EnterState()
	{
		mSoldier.UpdateChildPosition ();
	}

	public void UpdateState()
	{
		if (!mSoldier.IsDead) {
			mSoldier.MakeDecision ();
			if(mSoldier.AttackTarget != null)
			{
				if(!mSoldier.AttackTarget.mBI.IsDestroyed && mSoldier.IsTargetInAttackRange())
				{	
					mSoldier.Attack();
				}
				else
				{
					ToMoveState();
				}
			}
			else
			{
				ToDeadState();
			}
			/*
			mSoldier.AttackTimer += Time.deltaTime;
			if (mSoldier.AttackTimer >= mSoldier.mAttackInterval) {
				mSoldier.AttackTimer = 0.0f;
			*/
			//}
		} else {
			ToDeadState();
		}
	}

	public void ExitState()
	{

	}

	public void ToAttackState()
	{

	}

	public void ToMoveState()
	{
		mSoldier.Anim.SetBool("SoldierMoving",true);
		mSoldier.SCurrentState = mSoldier.mSMoveState;
	}

	public void ToDeadState()
	{
		mSoldier.Anim.SetBool("SoldierDead",true);
		mSoldier.SCurrentState = mSoldier.mSDeadState;
	}
}
