using UnityEngine;
using System.Collections;

public class ZomBunnyButton : MonoBehaviour {
	public void onClick()
	{
		Utility.Log ("ZomBunnyButton::onClick");
		MapManager.MMInstance.setCurrentSelectedSoldier (SoldierType.E_ZOMBUNNY);
	}
}
