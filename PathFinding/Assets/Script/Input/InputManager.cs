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
							Debug.Log("hit.collider.name = " + hit.collider.name);
						MapManager.MMInstance.CurrentSelectedNode = hit.collider.gameObject.GetComponent<TerrainNode>();
							UIManager.UIMInstance.ShowNWAdjustPanel();
						}
					}
					else
					{
						MapManager.MMInstance.CurrentSelectedNode = null;
						UIManager.UIMInstance.HideNWAdustPanel();
					}
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
