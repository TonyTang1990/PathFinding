using UnityEngine;
using System.Collections;

public class BuildingIdleState : BuildingState {
	private Building mBuilding; 

	public BuildingIdleState(Building building)
	{
		mBuilding = building;
	}

	public void EnterState()
	{

	}

	public void UpdateState()
	{
		if (mBuilding.gameObject != null && !mBuilding.mBI.IsDestroyed && mBuilding.mAttackable) {
			if (mBuilding.CanAttack())
				ToAttackState();
		}
	}

	public void ExitState()
	{

	}
	
	public void ToAttackState()
	{
		mBuilding.BCurrentState = mBuilding.mBAttackState;
	}
	
	public void ToIdleState()
	{

	}
}
