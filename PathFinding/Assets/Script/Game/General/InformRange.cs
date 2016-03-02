using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InformRange : MonoBehaviour {

	public Hashtable RangeTargetList {
		get {
			return mRangeTargetList;
		}
		set {
			mRangeTargetList = value;
		}
	}
	private Hashtable mRangeTargetList;
	
	public ObjectType mInterestedObjectType;
	
	void Awake()
	{
		mRangeTargetList = new Hashtable (50, 0.6f);
	}
	
	void OnTriggerEnter(Collider other)
	{
        if (mInterestedObjectType == ObjectType.EOT_SOLDIER)
        {
            if (other.CompareTag("Soldier"))
            {
                //ObjectType objtype = other.gameObject.GetComponent<GameObjectType>().GameType;//other.transform.parent.gameObject.GetComponent<GameObjectType> ().GameType;
                //if (objtype == ObjectType.EOT_SOLDIER)
                //{
                    Soldier so = other.gameObject.GetComponent<Soldier>();
                    int soinstanceid = so.gameObject.GetInstanceID();
                    if (so.IsDead != true && mRangeTargetList.Contains(soinstanceid) != true)
                    {
                        mRangeTargetList.Add(soinstanceid, so);
                        Utility.Log("BuildingInformRange::OnTriggerEnter mRanTargetList.Add(so) so.name = " + so.name);
                    }
                //}
            }
        }
        else if (mInterestedObjectType == ObjectType.EOT_BUILDING)
        {
            if (other.CompareTag("Building"))
            {
                Building bd = other.gameObject.GetComponent<Building>();

                if (bd.mBI.IsDestroyed != true && mRangeTargetList.Contains(bd.mBI.mIndex) != true)
                {
                    mRangeTargetList.Add(bd.mBI.mIndex, bd);
                    Utility.Log("Soldier mRanTargetList.Add(bd) bd.name = " + bd.name);
                }
            }
        }
	}
	
	void OnTriggerExit(Collider other)
	{
        if (mInterestedObjectType == ObjectType.EOT_SOLDIER)
        {
            if (other.CompareTag("Soldier"))
            {
                //ObjectType objtype = other.gameObject.GetComponent<GameObjectType>().GameType;
                //if (objtype == ObjectType.EOT_SOLDIER)
                //{
                    Soldier so = other.gameObject.GetComponent<Soldier>();
                    int soinstanceid = so.gameObject.GetInstanceID();
                    if (mRangeTargetList.Contains(soinstanceid) == true)
                    {
                        mRangeTargetList.Remove(soinstanceid);
                        Utility.Log("BuildingInformRange::OnTriggerExit mRanTargetList.Remove(so) so.name = " + so.name);
                    }
                //}
            }
        }
        if (mInterestedObjectType == ObjectType.EOT_BUILDING)
        {
            if (other.CompareTag("Building"))
            {
                Building bd = other.gameObject.GetComponent<Building>();
            
                if (mRangeTargetList.Contains(bd.mBI.mIndex) == true)
                {
                    mRangeTargetList.Remove(bd.mBI.mIndex);
                    Utility.Log("Soldier mRanTargetList.Remove(bd) bd.name = " + bd.name);
                }
            }
        }
	}
}
