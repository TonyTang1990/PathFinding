using UnityEngine;
using System.Collections;

public class DrawerButton : MonoBehaviour {
	public void onClick()
	{
		Debug.Log ("DrawerButton::onClick");
		MapManager.MMInstance.setCurrenctSelectedBuilding ((int)BuildingType.E_DRAWER);
	}
}
