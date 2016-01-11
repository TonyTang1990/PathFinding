using UnityEngine;
using System.Collections;

public class BuildingBuildState : BuildingState {
	private Building mBuilding; 
	
	public BuildingBuildState(Building building)
	{
		mBuilding = building;
	}

	public void EnterState()
	{
		mBuilding.ActiveBuildingUI (true);
	}

	public void UpdateState()
	{
		if (mBuilding.gameObject != null && !mBuilding.mBI.IsDestroyed) {
			if(mBuilding.mBI.IsBuildedCompleted)
			{
				ToIdleState();
			}
		}
	}

	public void ExitState()
	{
		mBuilding.ActiveBuildingUI (false);
	}

	public void ToAttackState()
	{

	}
	
	public void ToIdleState()
	{
		mBuilding.BCurrentState = mBuilding.mBIdleState;
	}
}
