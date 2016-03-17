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

    private Vector2 mPreMoveDirection;

    private Vector2[] mPreTouchFingerPos;

    private Vector2[] mCurrentTouchFingerPos;

    private Vector2[] mTouchFingerDeltaPos;

    private float mCurrentTwoFingersDistance;

    private float mPreTwoFingersDistance;

	public float mValidInputDeltaTime = 0.5f;
	
	private float mInputTimer = 0.0f;

    void Start()
    {
		Camera.main.transparencySortMode = TransparencySortMode.Orthographic;

		mOFinger1Position = new Vector2 ();
		mOFinger2Postion = new Vector2 ();
		mIsForward = 1;
		mScreenPos = new Vector2 ();
        mPreMoveDirection = new Vector2();
        mCurrentTouchFingerPos = new Vector2[2];
        mPreTouchFingerPos = new Vector2[2];
        mTouchFingerDeltaPos = new Vector2[2];
        mCurrentTwoFingersDistance = 0.0f;
        mPreTwoFingersDistance = 0.0f;
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
        if (Input.touchCount == 1)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                mCurrentTouchFingerPos[0] = Input.touches[0].position;
                mPreTouchFingerPos[0] = Input.touches[0].position;
                mTouchFingerDeltaPos[0] = Vector2.zero;
            }
            else if (Input.touches[0].phase == TouchPhase.Moved)
            {
                //Vector3 movement = transform.rotation * new Vector3 (Input.touches[0].deltaPosition.x * mMoveSpeed * Time.deltaTime, 0.0f, Input.touches[0].deltaPosition.y * mMoveSpeed * Time.deltaTime);

                mPreTouchFingerPos[0] = mCurrentTouchFingerPos[0];
                mCurrentTouchFingerPos[0] = Input.touches[0].position;
                mTouchFingerDeltaPos[0] = Input.touches[0].deltaPosition;

                Vector2 offposition = Input.touches[0].deltaPosition;
                //Low down the screen touch move
                offposition /= 5;
                MoveCamera(offposition);
            }
            else if (Input.touches[0].phase == TouchPhase.Ended)
            {
                mCurrentTouchFingerPos[0] = Vector2.zero;
                mPreTouchFingerPos[0] = Vector2.zero;
                mTouchFingerDeltaPos[0] = Vector2.zero;
            }
        }
        else if (Input.touchCount == 2)
        {
            for(int i = 0; i < 2; i++)
            {
                if (Input.touches[i].phase == TouchPhase.Began)
                {
                    mCurrentTouchFingerPos[i] = Input.touches[0].position;
                    mPreTouchFingerPos[i] = Input.touches[0].position;
                    mTouchFingerDeltaPos[i] = Vector2.zero;
                    if (i == 1)
                    {
                        mPreTwoFingersDistance = mCurrentTwoFingersDistance;
                        mCurrentTwoFingersDistance = Vector2.Distance(mCurrentTouchFingerPos[0], mCurrentTouchFingerPos[1]);
                    }
                }
                else if (Input.touches[i].phase == TouchPhase.Moved)
                {
                    mPreTouchFingerPos[i] = mCurrentTouchFingerPos[i];
                    mCurrentTouchFingerPos[i] = Input.touches[i].position;
                    mTouchFingerDeltaPos[i] = Input.touches[i].deltaPosition;
                    if (i == 1)
                    {
                        mPreTwoFingersDistance = mCurrentTwoFingersDistance;
                        mCurrentTwoFingersDistance = Vector2.Distance(mCurrentTouchFingerPos[0], mCurrentTouchFingerPos[1]);
                    }
                }
                else if (Input.touches[i].phase == TouchPhase.Ended)
                {
                    mCurrentTouchFingerPos[i] = Vector2.zero;
                    mPreTouchFingerPos[i] = Vector2.zero;
                    mTouchFingerDeltaPos[i] = Vector2.zero;
                    mCurrentTwoFingersDistance = 0.0f;
                    mPreTwoFingersDistance = 0.0f;
                    return;
                }
            }

            //Caculate for camera move and zoom
            Vector2 offposition = mTouchFingerDeltaPos[0] + mTouchFingerDeltaPos[1];
            //Slow down camera move speed
            offposition /= 10;
            MoveCamera(offposition);

            float distanceoffset = 0.0f;
            distanceoffset = mCurrentTwoFingersDistance - mPreTwoFingersDistance;
            //Slow down the zoom speed
            distanceoffset = Mathf.Abs(distanceoffset) / 4;
            if (mCurrentTwoFingersDistance > mPreTwoFingersDistance && mPreTwoFingersDistance != 0.0f)
            {
                Camera.main.orthographicSize -= mZoomSpeed * distanceoffset * Time.deltaTime;
            }
            else if (mCurrentTwoFingersDistance <= mPreTwoFingersDistance && mPreTwoFingersDistance != 0.0f)
            {
                Camera.main.orthographicSize += mZoomSpeed * distanceoffset * Time.deltaTime;
            }

            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, mOrthographicMinSize, mOrthographicMaxSize);
        }
	}

    private void MoveCamera(Vector2 offposition)
    {
        if (offposition.magnitude != 0.0f)
        {
            //Slow down the movement of screen touch
            float moveHorizontal = offposition.x * mMoveSpeed * Time.deltaTime;
            float moveVertical = offposition.y * Camera.main.aspect * mMoveSpeed * Time.deltaTime;

            Vector3 movement = transform.rotation * new Vector3(moveHorizontal, 0.0f, moveVertical);
            //Slow down screen touch move if the orthographicSize is smaller
            movement = -movement * Camera.main.orthographicSize / mOrthographicMaxSize;
            movement.y = 0.0f;
            transform.position += movement;
            float clampx;
            float clampz;
            clampx = Mathf.Clamp(transform.position.x, mCameraMinX, mCameraMaxX);
            clampz = Mathf.Clamp(transform.position.z, mCameraMinZ, mCameraMaxZ);
            transform.position = new Vector3(clampx, transform.position.y, clampz);
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
                    CameraMove();
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
