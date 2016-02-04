using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellDetectRange : MonoBehaviour {

	public Hashtable RangeTargetList {
		get {
			return mRangeTargetList;
		}
		set {
			mRangeTargetList = value;
		}
	}
	private Hashtable mRangeTargetList;

	public ObjectType mInterstedType;

	public BuildingType mInterestedBuildingType;

	void Awake()
	{
		if (mInterstedType == ObjectType.EOT_BUILDING) {
			mRangeTargetList = new Hashtable (20, 0.6f);
		} else if (mInterstedType == ObjectType.EOT_SOLDIER) {
			mRangeTargetList = new Hashtable (50, 0.6f);
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
        if (other.CompareTag("Building") || other.CompareTag("Soldier"))
        {
            ObjectType objtype = other.gameObject.GetComponent<GameObjectType>().GameType;//other.transform.parent.gameObject.GetComponent<GameObjectType> ().GameType;
			if(objtype == mInterstedType && mInterstedType == ObjectType.EOT_SOLDIER)
			{
				Soldier so = other.gameObject.GetComponent<Soldier>();
				int soinstanceid = so.gameObject.GetInstanceID();
				if (so.IsDead != true && mRangeTargetList.Contains (soinstanceid) != true) {
					mRangeTargetList.Add (soinstanceid, so);
                    Utility.Log("SpellDetectRange::OnTriggerEnter mRanTargetList.Add(so) so.name = " + so.name);
				}
			}
			else if(objtype == mInterstedType && mInterstedType == ObjectType.EOT_BUILDING)
			{
				Building bd = other.gameObject.GetComponent<Building>();
				if(bd.mBI.getBuildingType() == mInterestedBuildingType)
				{
					if (bd.mBI.IsDestroyed != true && mRangeTargetList.Contains (bd) != true) {
						mRangeTargetList.Add (bd.mBI.mIndex,bd);
                        Utility.Log("Spell mRanTargetList.Add(bd) bd.name = " + bd.name);
					}
				}
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
        if (other.CompareTag("Building") || other.CompareTag("Soldier"))
        {
			ObjectType objtype = other.gameObject.GetComponent<GameObjectType> ().GameType;
			if(objtype == mInterstedType && mInterstedType == ObjectType.EOT_SOLDIER)
			{
				Soldier so = other.gameObject.GetComponent<Soldier>();
				int soinstanceid = so.gameObject.GetInstanceID();
				if (mRangeTargetList.Contains (soinstanceid) == true) {
					mRangeTargetList.Remove (soinstanceid);
                    Utility.Log("SpellDetectRange::OnTriggerExit mRanTargetList.Remove(so) so.name = " + so.name);
				}
			}
			else if(objtype == mInterstedType && mInterstedType == ObjectType.EOT_BUILDING)
			{
				Building bd = other.gameObject.GetComponent<Building>();
				if(bd.mBI.getBuildingType() == mInterestedBuildingType)
				{
					if (mRangeTargetList.Contains (bd) == true) {
						mRangeTargetList.Remove (bd.mBI.mIndex);
                        Utility.Log("Spell mRanTargetList.Remove(bd) bd.name = " + bd.name);
					}
				}
			}
		}
	}
}
