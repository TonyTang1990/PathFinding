﻿using UnityEngine;
using System.Collections;

public class SoldierMoveState : SoldierState {

	private Soldier mSoldier;

	public SoldierMoveState(Soldier soldier)
	{
		mSoldier = soldier;
	}

	public void EnterState()
	{
		
	}

	public void UpdateState()
	{
		if (!mSoldier.IsDead) {
			mSoldier.MakeDecision ();
			//mSoldier.CalculatePath();
			//GameManager.mGameInstance.Navigation
			if(mSoldier.AttackTarget != null)
			{
				if(mSoldier.IsTargetInAttackRange())
				{
					ToAttackState();
				}
				else
				{
                    mSoldier.Move ();
                }
			}
			else
			{
				ToDeadState();
			}
		} else {
			ToDeadState();
		}
	}

	public void ExitState()
	{
		
	}

	public void ToAttackState()
	{
        mSoldier.transform.LookAt(new Vector3(mSoldier.AttackTarget.transform.position.x, mSoldier.transform.position.y, mSoldier.AttackTarget.transform.position.z));

		mSoldier.AttackTimer = mSoldier.mAttackInterval;
		mSoldier.Anim.SetBool("SoldierMoving",false);
		mSoldier.SCurrentState = mSoldier.mSAttackState as SoldierState;
	}
	
	public void ToMoveState()
	{
		
	}

	public void ToDeadState()
	{
		mSoldier.Anim.SetBool("SoldierDead",true);
		mSoldier.SCurrentState = mSoldier.mSDeadState;
	}
	/*
	private void Move()
	{
		if (mSoldier.AttackTarget != null) {
			Vector3 movedirection = mSoldier.AttackTarget.mBI.Position - mSoldier.transform.position;
			movedirection.Normalize();
			mSoldier.transform.LookAt (mSoldier.AttackTarget.mBI.Position);
			if(!mSoldier.IsTargetInAttackRange())
			{
				Vector3 newposition = mSoldier.transform.position + movedirection * mSoldier.mSpeed * Time.deltaTime;
				mSoldier.transform.position = newposition;
			}
			else
			{
				ToAttackState();
			}
		}
	}
	*/
}
