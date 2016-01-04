using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System;

//Utility tool
public class Utility {
	public static void ParserStringToInt(string s, out int result)
	{

		if (!Int32.TryParse (s,out result)) {
			Utility.Log("----------------------------ParserStringToInt Failed");
		}

		Assert.IsTrue (result >= 0);
	}

	public static void ParserStringToFloat(string s, out float result)
	{

		if (!float.TryParse(s,out result)) {
			Utility.Log("----------------------------ParserStringToFloat Failed");
		}
		
		Assert.IsTrue (result >= 0.0f);
	}

	public static int ConvertRCToIndex(int row, int column)
	{
		int totalrow = MapManager.MMInstance.PathFinder.mRow;
		int totalcolumn = MapManager.MMInstance.PathFinder.mColumn;

		Assert.IsTrue( (row > 0 && row <= totalrow) );
		Assert.IsTrue( (column > 0 && column <= totalcolumn) );

		return ((row -1 ) * totalcolumn + (column - 1));
	}

	public static Vector2 ConvertIndexToRC(int index)
	{
		//int row = index / MapManager.MMInstance.PathFinder.mColumn + 1;
		//int column = index % MapManager.MMInstance.PathFinder.mColumn + 1;
		int row = index / MapManager.MMInstance.PathFinder.mColumn + 1;
		int column = index % MapManager.MMInstance.PathFinder.mRow + 1;
		return new Vector2 (row, column);
	}

	public static int ConvertFloatPositionToIndex(Vector3 pos)
	{
		int index = 0;

		int row = (int)(pos.x) + 1;
		int column = (int)(pos.z) + 1;
		index = ConvertRCToIndex(row, column);

		return index;
	}

	public static Vector2 ConvertFloatPositionToRC(Vector3 pos)
	{
		Vector2 rc;
		
		int row = (int)(pos.x) + 1;
		int column = (int)(pos.z) + 1;
		rc.x = row;
		rc.y = column;
		
		return rc;
	}

	public static void Log(string s)
	{
		if (GameManager.mGameInstance.mIsDebugEnable) {
			Debug.Log (s);
		}
	}
}
