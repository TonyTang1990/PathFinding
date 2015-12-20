using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System;

//Utility tool
public class Utility {
	public static void ParserStringToInt(string s, out int result)
	{

		if (!Int32.TryParse (s,out result)) {
			Debug.Log("----------------------------ParserStringToInt Failed");
		}

		Assert.IsTrue (result >= 0);
	}

	public static void ParserStringToFloat(string s, out float result)
	{

		if (!float.TryParse(s,out result)) {
			Debug.Log("----------------------------ParserStringToFloat Failed");
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
		int row = index / MapManager.MMInstance.PathFinder.mColumn + 1;
		int column = index % MapManager.MMInstance.PathFinder.mColumn + 1;
		return new Vector2 (row, column);
	}
}
