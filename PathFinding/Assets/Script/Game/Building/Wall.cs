using UnityEngine;
using System.Collections;

public class Wall : Building {

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
	
	public override void FixedUpdate()
	{
		base.FixedUpdate ();
	}

	public override bool IsTargetAvalibleToAttack()
	{
		return false;
	}

	public override void Update()
	{
		base.Update ();
	}

	public override void TakeDamage(float damage)
	{
		base.TakeDamage (damage);
		//Once the wall has been destroyed, we should update Node status
		if (mBI.IsDestroyed) {
			MapManager.MMInstance.PathFinder.NavGraph.Nodes[mBI.mIndex].IsWall = false;

			MapManager.MMInstance.NodeTerrainList[mBI.mIndex].IsWall = false;

			MapManager.MMInstance.UpdateSpecificNodeWallStatus(mBI.mIndex,false);

			MapManager.MMInstance.UpdateSpecificNodeWeight(mBI.mIndex, -mWeight);

			//Once wall is breaked, we dispatch WALL_BREAK Event to soldier
			EventManager.mEMInstance.TriggerEvent("WALL_BREAK");
		}
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
}
