using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour {

	public static InputManager mInputInstance = null;

	public float mValidInputDeltaTime = 0.5f;

	private float mInputTimer = 0.0f;

	void Awake()
	{
		if (mInputInstance == null) {
			mInputInstance = this;
		} else if (mInputInstance != this) {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		mInputTimer += Time.deltaTime;

		if (Input.GetMouseButtonDown (0)) {
			Debug.Log ("Left Mouse Clicked");

			if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ()) {
				mInputTimer = 0.0f;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, Mathf.Infinity, LayerMask.GetMask ("Terrain"))) {
					if (hit.collider) {
						if (MapManager.MMInstance.isSoldierSelected) {
							Debug.Log ("hit.point = " + hit.point);
							MapManager.MMInstance.DeploySoldier (hit.point);
						}
						else{
							Debug.Log("hit.collider.name = " + hit.collider.name);
							MapManager.MMInstance.CurrentSelectedNode = hit.collider.gameObject.GetComponent<TerrainNode>();
							UIManager.UIMInstance.ShowNWAdjustPanel();
						}
					}
				}
				else
				{
					MapManager.MMInstance.CurrentSelectedNode = null;
					UIManager.UIMInstance.HideNWAdustPanel();
				}
			}
		}

		
		if (mInputTimer > mValidInputDeltaTime) {
			if (Input.GetKey (KeyCode.O)) {
				Debug.Log ("KeyCode.O Pressed");
				mInputTimer = 0.0f;
				if (MapManager.MMInstance.IsTerrainAvaibleToBuild ()) {
					MapManager.MMInstance.BuildBuilding ();
				}
			}
			
			if (Input.GetKey (KeyCode.F1)) {
				Debug.Log ("KeyCode.F1 Pressed");
				mInputTimer = 0.0f;
				MapManager.MMInstance.DeselectChoosingStaff ();
			}
		}

		/*
		if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Ended) {
			Debug.Log("Touched");
			Vector2 touchposition = Input.GetTouch (0).position;
			Debug.Log ("touchposition.x = " + touchposition.x);
			Debug.Log ("touchposition.y = " + touchposition.y);
			//Camera.main.ScreenToWorldPoint(
		}
		*/
	}
}
