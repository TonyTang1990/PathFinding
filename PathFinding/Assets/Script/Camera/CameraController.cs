using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class CameraController : MonoBehaviour {

    public float mMoveSpeed = 24.0f;
	public float mZoomSpeed = 3.0f;

	public float mOrthographicMinSize = 10.0f;
	public float mOrthographicMaxSize = 30.0f;

	public float mCameraMinX = -60.0f;
	public float mCameraMaxX = -20.0f;
	public float mCameraMinZ = -60.0f;
	public float mCameraMaxZ = -20.0f;

	private Vector3 mLookDirection;

	//Record fingers position
	private Vector2 mOFinger1Position;

	private Vector2 mOFinger2Postion;

	private int mIsForward;

	private Vector2 mScreenPos;

	public float mValidInputDeltaTime = 0.5f;
	
	private float mInputTimer = 0.0f;

    void Start()
    {
		Camera.main.transparencySortMode = TransparencySortMode.Orthographic;

		mOFinger1Position = new Vector2 ();
		mOFinger2Postion = new Vector2 ();
		mIsForward = 1;
		mScreenPos = new Vector2 ();
    }

	//用于判断是否放大
	private bool isEnlarge(Vector2 oP1, Vector2 oP2, Vector2 nP1, Vector2 nP2)
	{
		//函数传入上一次触摸两点的位置与本次触摸两点的位置计算出用户的手势
		float leng1 = Mathf.Sqrt((oP1.x - oP2.x) * (oP1.x - oP2.x) + (oP1.y - oP2.y) * (oP1.y - oP2.y));
		float leng2 = Mathf.Sqrt((nP1.x - nP2.x) * (nP1.x - nP2.x) + (nP1.y - nP2.y) * (nP1.y - nP2.y));
		if (leng1 < leng2)
		{
			//放大手势
			return true;
		}
		else
		{
			//缩小手势
			return false;
		}
	}

	private void CameraMove()
	{
		if(Input.touches[0].phase == TouchPhase.Began)
		{
			mScreenPos = Input.touches[0].position;
		}
		
		if(Input.touches[0].phase == TouchPhase.Moved)
		{
			Vector3 movement = transform.rotation * new Vector3 (Input.touches[0].deltaPosition.x * mMoveSpeed * Time.deltaTime, 0.0f, Input.touches[0].deltaPosition.y * mMoveSpeed * Time.deltaTime);
			
			Debug.Log("Input.touches[0].deltaPosition.x = " + Input.touches[0].deltaPosition.x);
			Debug.Log("Input.touches[0].deltaPosition.y = " + Input.touches[0].deltaPosition.y);
			
			transform.position += movement;
			float clampx;
			float clampz;
			clampx = Mathf.Clamp (transform.position.x, mCameraMinX, mCameraMaxX);
			clampz = Mathf.Clamp (transform.position.z, mCameraMinZ, mCameraMaxZ);
			transform.position = new Vector3 (clampx, transform.position.y, clampz);
		}
	}

	void Update()
	{
		mInputTimer += Time.deltaTime;

		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
			if (Input.GetKey (KeyCode.U)) {
				Camera.main.orthographicSize += mZoomSpeed * Time.deltaTime;
			}

			if (Input.GetKey (KeyCode.P)) {
				Camera.main.orthographicSize -= mZoomSpeed * Time.deltaTime;
			}
			Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize, mOrthographicMinSize, mOrthographicMaxSize);
		
			float moveHorizontal = Input.GetAxis ("Horizontal") * mMoveSpeed * Time.deltaTime;
			float moveVertical = Input.GetAxis ("Vertical") * mMoveSpeed * Time.deltaTime;
			
			Vector3 movement = transform.rotation * new Vector3 (moveHorizontal, 0.0f, moveVertical);
			movement.y = 0.0f;
			
			transform.position += movement;
			float clampx;
			float clampz;
			clampx = Mathf.Clamp (transform.position.x, mCameraMinX, mCameraMaxX);
			clampz = Mathf.Clamp (transform.position.z, mCameraMinZ, mCameraMaxZ);
			transform.position = new Vector3 (clampx, transform.position.y, clampz);
		}


		if (Application.platform == RuntimePlatform.Android)
		{
			if(Input.touchCount <= 0)
			{
				return;
			}

			//Only one finger control
			if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject (Input.touches[0].fingerId))
			{
				if(Input.touchCount == 1)
				{
					if(!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject (Input.touches[0].fingerId))
					{
						Ray ray = new Ray();
						RaycastHit hit;

						if(MapManager.MMInstance.isBuildingSelected)
						{
							ray = Camera.main.ScreenPointToRay (Input.touches[0].position);

							if (Physics.Raycast (ray, out hit, Mathf.Infinity, LayerMask.GetMask ("Terrain"))) 
							{
								TerrainNode currentterrainnode = hit.transform.gameObject.GetComponent<TerrainNode>();
								if(!Utility.IsValidTerrainToMoveBuilding(currentterrainnode.Index))
								{
									CameraMove();
								}
							}
						}
						else
						{
							CameraMove();
						}
					}
				}
				else if(Input.touchCount == 2)
				{
					/*
					Vector2 fposition1 = new Vector2();
					Vector2 fposition2 = new Vector2();

					Vector2 deltadis1 = new Vector2();
					Vector2 deltadis2 = new Vector2();

					for(int i = 0; i < 2; i++)
					{
						Touch touch = Input.touches[i];
						if(touch.phase == TouchPhase.Ended)
						{
							break;
						}
						if(touch.phase == TouchPhase.Moved)
						{
							if(i == 0)
							{
								fposition1 = touch.position;
								deltadis1 = touch.deltaPosition;
							}
							else
							{
								fposition2 = touch.position;
								deltadis2 = touch.deltaPosition;

								if(isEnlarge(mOFinger1Position, mOFinger2Postion, fposition1, fposition2))
								{
									mIsForward = 1;
								}
								else
								{
									mIsForward = -1;
								}
							}

							//Record old finger position
							mOFinger1Position = fposition1;
							mOFinger2Postion = fposition2;
						}

						//Move camera and zoom in or out
						Vector2 centerpoint = (fposition1 + fposition2) / 2;
						Vector3 movement = transform.rotation * new Vector3 (centerpoint.x, 0.0f, centerpoint.y);
						
						transform.position += movement;
						float clampx;
						float clampz;
						clampx = Mathf.Clamp (transform.position.x, mCameraMinX, mCameraMaxX);
						clampz = Mathf.Clamp (transform.position.z, mCameraMinZ, mCameraMaxZ);
						transform.position = new Vector3 (clampx, transform.position.y, clampz);

						Camera.main.orthographicSize += mIsForward * mZoomSpeed * Time.deltaTime;

						Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize, mOrthographicMinSize, mOrthographicMaxSize);
					}
					*/
				}
			}
		    else
		    {
				return;
			}
		}
	}

    void LateUpdate()
    {
       
    }
}
