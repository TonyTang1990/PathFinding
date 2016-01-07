using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
	public Text mFPSText;

	private float mDeltaTime = 0.0f;

	private float mFPS = 0.0f;

	void Update()
	{
		mDeltaTime += (Time.deltaTime - mDeltaTime) * 0.1f;
		float msec = mDeltaTime * 1000.0f;
		mFPS = 1.0f / mDeltaTime;
		mFPSText.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, mFPS);

	}
	
	void OnGUI()
	{
		/*
		int w = Screen.width, h = Screen.height;
		
		GUIStyle style = new GUIStyle();
		
		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 4 / 100;
		style.normal.textColor = new Color (255.0f, 255.0f, 255.0f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
		*/
	}
}