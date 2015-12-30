using UnityEngine;
using System.Collections;

public class WallButton : MonoBehaviour{
	public void onClick()
	{
		Debug.Log ("WallButton::onClick");
		MapManager.MMInstance.setCurrenctSelectedBuilding ((int)BuildingType.E_WALL);
	}
}
