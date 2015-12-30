using UnityEngine;
using System.Collections;

public class HellephantButton : MonoBehaviour {
	public void onClick()
	{
		Debug.Log ("ZomBearButton::onClick");
		MapManager.MMInstance.setCurrentSelectedSoldier (SoldierType.E_HELLEPHANT);
	}
}
