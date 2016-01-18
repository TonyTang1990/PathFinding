using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingAttackRange: MonoBehaviour{

	public Hashtable RangeTargetList {
		get {
			return mRangeTargetList;
		}
		set {
			mRangeTargetList = value;
		}
	}
	private Hashtable mRangeTargetList;

	//public ObjectType mInterestedObjectType;

	void Awake()
	{
		mRangeTargetList = new Hashtable (50, 0.6f);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "TerrainTile" || other.tag == "AttackRange" || other.tag == "Bullet" || other.tag == "Spell") {
			return;
		} else {
			ObjectType objtype = other.gameObject.GetComponent<GameObjectType>().GameType;//other.transform.parent.gameObject.GetComponent<GameObjectType> ().GameType;
				if(objtype == ObjectType.EOT_SOLDIER)
				{
					Soldier so = other.gameObject.GetComponent<Soldier>();
					int soinstanceid = so.gameObject.GetInstanceID();
				if (so.IsDead != true && mRangeTargetList.Contains (soinstanceid) != true) {
						mRangeTargetList.Add (soinstanceid, so);
						Utility.Log ("BuildingAttackRange::OnTriggerEnter mRanTargetList.Add(so) so.name = " + so.name);
					}
				}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "TerrainTile" || other.tag == "AttackRange" || other.tag == "Bullet" || other.tag == "Spell") {
			return;
		} else {
			ObjectType objtype = other.gameObject.GetComponent<GameObjectType> ().GameType;
				if(objtype == ObjectType.EOT_SOLDIER)
				{
					Soldier so = other.gameObject.GetComponent<Soldier>();
					int soinstanceid = so.gameObject.GetInstanceID();
					if (mRangeTargetList.Contains (soinstanceid) == true) {
						mRangeTargetList.Remove (soinstanceid);
						Utility.Log ("BuildingAttackRange::OnTriggerExit mRanTargetList.Remove(so) so.name = " + so.name);
					}
				}
		}
	}

}
