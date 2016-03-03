using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JumpSpell : Spell {

    private InformRange mInformRange;

    private GameObject mInformRangeCollider;

	public override void Awake()
	{
		base.Awake ();
        mInformRangeCollider = gameObject.transform.Find("InformRangeCollider").gameObject;
        mInformRange = mInformRangeCollider.GetComponent<InformRange>();
	}

	void Start()
	{
		Invoke ("UpdateWallJumpableStatus", 0.1f);
	}

	private void UpdateWallJumpableStatus()
	{
        Utility.Log("UpdateWallJumpableStatus() called");
        Utility.Log("mSpellDetectRange.RangeTargetList.Count = " + mSpellDetectRange.RangeTargetList.Count);
        IDictionaryEnumerator enu = mSpellDetectRange.RangeTargetList.GetEnumerator();
        DictionaryEntry entry;
        int wallindex = 0;
        Wall wa;
        while (enu.MoveNext()) {
            entry = (DictionaryEntry)enu.Current;
            wallindex = (int)entry.Key;
            Utility.Log("WallIndex = " + wallindex);
			MapManager.MMInstance.UpdateSpecificNodeWallJumpableStatus (wallindex,true);
            wa = MapManager.MMInstance.BuildingsInfoInGame[wallindex] as Wall;
            wa.Jumpable = true;
            wa.UpdateJumpSpellWorking(this);

            //Modify Terrain Node info too to make sure the NodeWeightPanel info correct
            MapManager.MMInstance.NodeTerrainList[wallindex].IsJumpable = true;
		}
		Invoke ("RecoverWallJumpableStatus", mLifeTime);

        InformationForJumpSpell();
	}

	private void RecoverWallJumpableStatus()
	{
        Utility.Log("RecoverWallJumpableStatus() called");
        IDictionaryEnumerator enu = mSpellDetectRange.RangeTargetList.GetEnumerator();
        DictionaryEntry entry;
        int wallindex = 0;
        Wall wa;
        Soldier so;
        while (enu.MoveNext())
        {
            entry = (DictionaryEntry)enu.Current;
            wallindex = (int)entry.Key;
			Utility.Log("WallIndex = " + wallindex);
			MapManager.MMInstance.UpdateSpecificNodeWallJumpableStatus (wallindex,false);
            wa = MapManager.MMInstance.BuildingsInfoInGame[wallindex] as Wall;
            wa.Jumpable = false;
            wa.UpdateJumpSpellWorking(null);

            //Modify Terrain Node info too to make sure the NodeWeightPanel info correct
            MapManager.MMInstance.NodeTerrainList[wallindex].IsJumpable = false;
            //We only need to inform the soldier that contains the wall(jumpable state changed) in their AstarPath
		}

        //InformationForJumpSpell();
        
        Destroy(gameObject);
	}

    private void InformationForJumpSpell()
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
                    so.JumpSpellDelegate();
                }
            }
        }
    }
}
