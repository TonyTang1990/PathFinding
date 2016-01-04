using UnityEngine;
using System.Collections;

public class HouseButton : MonoBehaviour{
	public void onClick()
	{
		Utility.Log ("HouseButton::onClick");
		MapManager.MMInstance.setCurrenctSelectedBuilding ((int)BuildingType.E_HOUSE);
	}
}
