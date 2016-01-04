using UnityEngine;
using System.Collections;

public class HellephantButton : MonoBehaviour {
	public void onClick()
	{
		Utility.Log ("ZomBearButton::onClick");
		MapManager.MMInstance.setCurrentSelectedSoldier (SoldierType.E_HELLEPHANT);
	}
}
