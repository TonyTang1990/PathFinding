using UnityEngine;
using System.Collections;

public class BuildingAttackState : BuildingState {
	private Building mBuilding; 

	public BuildingAttackState(Building building)
	{
		mBuilding = building;
	}

	public void EnterState()
	{

	}

	public void UpdateState()
	{
		if (mBuilding.IsTargetAvalibleToAttack () && !mBuilding.mBI.IsDestroyed) {
			Attack ();
		} else {
			ToIdleState();
		}
	}

	public void ExitState()
	{

	}

	public void ToAttackState()
	{
		
	}
	
	public void ToIdleState()
	{
		mBuilding.BCurrentState = mBuilding.mBIdleState;
	}

	private void Attack()
	{
		mBuilding.Attack();
	}
}