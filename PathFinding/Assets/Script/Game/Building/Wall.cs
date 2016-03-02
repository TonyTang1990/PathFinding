using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wall : Building {

	private InformRange mInformRange;

	private GameObject mInformRangeCollider;

    public bool Jumpable
    {
        get
        {
            return mJumpable;
        }
        set
        {
            mJumpable = value;
            if (mJumpable == false)
            {
                EventManager.mEMInstance.TriggerEvent(gameObject.GetInstanceID() + "JumpableSpellDisappear");
            }
        }
    }
    private bool mJumpable = false;

    public JumpSpell LatestJumpSpell
    {
        get
        {
            return mLatestJumpSpell;
        }
        set
        {
            mLatestJumpSpell = value;
        }
    }
    private JumpSpell mLatestJumpSpell;

	public override void Awake()
	{
		base.Awake ();
		mBAttackState = new BuildingAttackState (this);
		mBIdleState = new BuildingIdleState (this);
		mBBuildingState = new BuildingBuildState (this);

		mInformRangeCollider = gameObject.transform.Find ("InformRangeCollider").gameObject;
        mInformRange = mInformRangeCollider.GetComponent<InformRange>();

        mName = "Wall" + GetInstanceID();
        //Debug.Log("mName = " + mName);
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
			//EventManager.mEMInstance.TriggerEvent("WALL_BREAK");
			InformAllSoldiersInRange();

            EventManager.mEMInstance.TriggerEvent(gameObject.GetInstanceID() + "Break");
		}
	}

	private void InformAllSoldiersInRange()
	{
        Building bd;
        IDictionaryEnumerator enu = mInformRange.RangeTargetList.GetEnumerator();
        DictionaryEntry entry;
        Soldier so;
        while (enu.MoveNext())
        {
            entry = (DictionaryEntry)enu.Current;
            bd = entry.Value as Building;
            if (bd != null)
            {
                foreach (KeyValuePair<int, Soldier> kvp in bd.AttackerList)
                {
                    so = kvp.Value as Soldier;
                    so.WallBreakDelegate();
                }
            }
        }
        //Soldier so;
        //IDictionaryEnumerator enu = mInformRange.RangeTargetList.GetEnumerator();
        //DictionaryEntry entry;
		//while(enu.MoveNext()) {
            //entry = (DictionaryEntry)enu.Current;
            //so = entry.Value as Soldier;
			//if(!so.IsDead /*&& so.AttackTarget.mBI.getBuildingType() == BuildingType.E_WALL*/)
			//{
				//so.WallBreakDelegate();
			//}
		//}
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

    public bool CanJump()
    {
        return mJumpable;
    }

    public void UpdateJumpSpellWorking(JumpSpell js)
    {
        mLatestJumpSpell = js;
    }
}
