using UnityEngine;
using System.Collections;

public class Wall : Building {

	public override void Awake()
	{
		base.Awake ();
		Debug.Log ("Wall::Awake()");
		mBAttackState = new BuildingAttackState (this);
		mBIdleState = new BuildingIdleState (this);
	}
	
	public override void Start()
	{
		base.Start ();
		mBCurrentState = mBIdleState;
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

			MapManager.MMInstance.UpdateSpecificNodeWeight(mBI.mIndex, 0);
			
			MapManager.MMInstance.UpdateSpecificNodeWallStatus(mBI.mIndex,false);

			MapManager.MMInstance.UpdateSpecificNodeWeight(mBI.mIndex, -mWeight);
		}
	}

	public override bool CanAttack()
	{
		return false;
	}

	public override void Attack()
	{
		
	}
}
