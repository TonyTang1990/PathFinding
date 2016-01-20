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
		Debug.Log ("UpdateWallJumpableStatus() called");
		Debug.Log ("mSpellDetectRange.RangeTargetList.Count = " + mSpellDetectRange.RangeTargetList.Count);
		foreach (int wallindex in mSpellDetectRange.RangeTargetList.Keys) {
			Debug.Log("WallIndex = " + wallindex);
			MapManager.MMInstance.UpdateSpecificNodeWallJumpableStatus (wallindex,true);
		}
		Invoke ("RecoverWallJumpableStatus", mLifeTime);
	}

	private void RecoverWallJumpableStatus()
	{
		Debug.Log ("RecoverWallJumpableStatus() called");
		foreach (int wallindex in mSpellDetectRange.RangeTargetList) {
			Debug.Log("WallIndex = " + wallindex);
			MapManager.MMInstance.UpdateSpecificNodeWallJumpableStatus (wallindex,false);
		}
		Destroy (gameObject);
	}
}
