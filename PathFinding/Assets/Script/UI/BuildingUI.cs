using UnityEngine;
using System.Collections;

public class BuildingUI : MonoBehaviour {

	public void onCrossButtonClick()
	{
		Utility.Log ("onCrossButtonClick::onClick");
		MapManager.MMInstance.DeselectChoosingStaff ();
	}

	public void onTickButtonClick()
	{
		Utility.Log ("onTickButtonClick::onClick");
		if (MapManager.MMInstance.IsTerrainAvaibleToBuild ()) {
			MapManager.MMInstance.BuildBuilding ();
		}
	}

}
