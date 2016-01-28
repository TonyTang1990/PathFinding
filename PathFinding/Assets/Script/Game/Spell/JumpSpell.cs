using UnityEngine;
using System.Collections;

public class JumpSpell : Spell {

	public override void Awake()
	{
		base.Awake ();
	}

	void Start()
	{
		Invoke ("UpdateWallJumpableStatus", 0.1f);
	}

	private void UpdateWallJumpableStatus()
	{
        Utility.Log("UpdateWallJumpableStatus() called");
        Utility.Log("mSpellDetectRange.RangeTargetList.Count = " + mSpellDetectRange.RangeTargetList.Count);
		foreach (int wallindex in mSpellDetectRange.RangeTargetList.Keys) {
            Utility.Log("WallIndex = " + wallindex);
			MapManager.MMInstance.UpdateSpecificNodeWallJumpableStatus (wallindex,true);
		}
		Invoke ("RecoverWallJumpableStatus", mLifeTime);
	}

	private void RecoverWallJumpableStatus()
	{
		Debug.Log ("RecoverWallJumpableStatus() called");
		foreach (int wallindex in mSpellDetectRange.RangeTargetList.Keys) {
			Utility.Log("WallIndex = " + wallindex);
			MapManager.MMInstance.UpdateSpecificNodeWallJumpableStatus (wallindex,false);
		}
		Destroy (gameObject);
	}
}
