﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoldierDetectRange : MonoBehaviour {

	public Hashtable/*List<Building>*/ RangeTargetList {
		get {
			return mRangeTargetList;
		}
		set {
			mRangeTargetList = value;
		}
	}
	private Hashtable /*List<Building>*/ mRangeTargetList;
	
	//public BuildingType mInterestedBuildingType;
	
	void Awake()
	{
		mRangeTargetList = new Hashtable (20, 0.6f); /*new List<Building> ();*/
	}
	
	void OnTriggerEnter(Collider other)
	{
        if (other.CompareTag("TerrainTile") || other.CompareTag("AttackRange") || other.CompareTag("Bullet") || other.CompareTag("Spell") || other.CompareTag("Soldier"))
        {
			return;
		} else {
			//mRangeTargetList.RemoveAll(item => item == null);
			ObjectType objtype = other.gameObject.GetComponent<GameObjectType>().GameType;//other.transform.parent.gameObject.GetComponent<GameObjectType> ().GameType;
			if(objtype == ObjectType.EOT_BUILDING)
			{
				Building bd = other.gameObject.GetComponent<Building>();
				if(bd.mBI.getBuildingType() == BuildingType.E_WALL)
				{
					return;
				}
				else
				{
					if (bd.mBI.IsDestroyed != true && mRangeTargetList.Contains (bd.mBI.mIndex) != true) {
						mRangeTargetList.Add (bd.mBI.mIndex,bd);
						Utility.Log ("Soldier mRanTargetList.Add(bd) bd.name = " + bd.name);
					}
				}
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
        if (other.CompareTag("TerrainTile") || other.CompareTag("AttackRange") || other.CompareTag("Bullet") || other.CompareTag("Spell"))
        {
			return;
		} else {
			ObjectType objtype = other.gameObject.GetComponent<GameObjectType> ().GameType;
			if(objtype == ObjectType.EOT_BUILDING)
			{
				Building bd = other.gameObject.GetComponent<Building>();
				if(bd.mBI.getBuildingType()  == BuildingType.E_WALL)
				{
					return;
				}
				else
				{
					if (mRangeTargetList.Contains (bd) == true) {
						mRangeTargetList.Remove (bd.mBI.mIndex);
						Utility.Log ("Soldier mRanTargetList.Remove(bd) bd.name = " + bd.name);
					}
				}
			}
		}
	}
}
