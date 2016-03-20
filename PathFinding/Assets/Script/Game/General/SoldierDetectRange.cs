using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoldierDetectRange : MonoBehaviour {

    public Dictionary<int, Building>/*List<Building>*/ RangeTargetList
    {
		get {
			return mRangeTargetList;
		}
		set {
			mRangeTargetList = value;
		}
	}
    private Dictionary<int, Building>  /*List<Building>*/ mRangeTargetList;

    public int AttackableAliveAliveBuildingNumber
    {
        get
        {
            return mAttackableAliveBuildingNumber;
        }
    }
    private int mAttackableAliveBuildingNumber;

    public int NotAttackableAliveBuildingNumber
    {
        get
        {
            return mNotAttackableAliveBuildingNumber;
        }
    }
    private int mNotAttackableAliveBuildingNumber;
	
	//public BuildingType mInterestedBuildingType;
	
	void Awake()
	{
        mRangeTargetList = new Dictionary<int, Building>(); /*new List<Building> ();*/

        mAttackableAliveBuildingNumber = 0;

        mNotAttackableAliveBuildingNumber = 0;
	}
	
	void OnTriggerEnter(Collider other)
	{
        if (other.CompareTag("Soldier") || other.CompareTag("Building"))
        {
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
					if (bd.mBI.IsDestroyed != true && mRangeTargetList.ContainsKey (bd.mBI.mIndex) != true) {
                        EventManager.mEMInstance.StartListening(bd.GetInstanceID() + "Break", BuildingDestroyedDelegate);
                        if (bd.mAttackable)
                        {
                            mAttackableAliveBuildingNumber++;
                        }
                        else
                        {
                            mNotAttackableAliveBuildingNumber++;
                        }
                    	mRangeTargetList.Add (bd.mBI.mIndex,bd);
						Utility.Log ("Soldier mRanTargetList.Add(bd) bd.name = " + bd.name);
					}
                    Utility.Log("After OnTriggerEnter()");
                    Utility.Log("Building.Name = " + bd.Name);
                    Utility.Log("mAttackableAliveBuildingNumber = " + mAttackableAliveBuildingNumber);
                    Utility.Log("mNotAttackableAliveBuildingNumber = " + mNotAttackableAliveBuildingNumber);
				}
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
        if (other.CompareTag("Soldier") || other.CompareTag("Building"))
        {
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
					if (mRangeTargetList.ContainsValue (bd) == true) {
                        EventManager.mEMInstance.StopListening(bd.GetInstanceID() + "Break", BuildingDestroyedDelegate);
                        if (bd.mAttackable)
                        {
                            mAttackableAliveBuildingNumber--;
                        }
                        else
                        {
                            mNotAttackableAliveBuildingNumber--;
                        }
						mRangeTargetList.Remove (bd.mBI.mIndex);
						Utility.Log ("Soldier mRanTargetList.Remove(bd) bd.name = " + bd.name);
					}
                    Utility.Log("After OnTriggerExit()");
                    Utility.Log("Building.Name = " + bd.Name);
                    Utility.Log("mAttackableAliveBuildingNumber = " + mAttackableAliveBuildingNumber);
                    Utility.Log("mNotAttackableAliveBuildingNumber = " + mNotAttackableAliveBuildingNumber);
				}
			}
		}
	}

    void BuildingDestroyedDelegate(int buildingindex)
    {
        if (mRangeTargetList.ContainsKey(buildingindex) == true)
        {
            Building bd = null;
            if(mRangeTargetList.TryGetValue(buildingindex, out bd))
            {
                if (bd.mAttackable)
                {
                    mAttackableAliveBuildingNumber--;
                }
                else
                {
                    mNotAttackableAliveBuildingNumber--;
                }
                mRangeTargetList.Remove(buildingindex);
            }
            Utility.Log("After BuildingDestroyedDelegate()");
            Utility.Log("Building.Name = " + bd.Name);
            Utility.Log("mAttackableAliveBuildingNumber = " + mAttackableAliveBuildingNumber);
            Utility.Log("mNotAttackableAliveBuildingNumber = " + mNotAttackableAliveBuildingNumber);
        }
    }

    public void RemoveAllListenersForBuildingInRange()
    {
        Dictionary<int, Building>.Enumerator enu = mRangeTargetList.GetEnumerator();
        KeyValuePair<int,Building> entry;
        while(enu.MoveNext())
        {
            entry = enu.Current;
            EventManager.mEMInstance.StopListening(entry.Value.GetInstanceID() + "Break", BuildingDestroyedDelegate);
            Utility.Log("After RemoveAllListenersForBuildingInRange()");
            Utility.Log("entry.Value.GetInstanceID()  = " + entry.Value.GetInstanceID());
        }
    }
}
