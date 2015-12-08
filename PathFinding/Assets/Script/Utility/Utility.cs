﻿using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System;

//Utility tool
public class Utility {
	public static void ParserStringToInt(string s, out int result)
	{

		if (!Int32.TryParse (s,out result)) {
			Debug.Log("----------------------------Parse Failed");
		}

		Assert.IsTrue (result > 0);
	}

	public static int ConvertRCToIndex(int row, int column)
	{
		int totalrow = MapManager.MMInstance.mRow;
		int totalcolumn = MapManager.MMInstance.mColumn;

		Assert.IsTrue( (row > 0 && row <= totalrow) );
		Assert.IsTrue( (column > 0 && column <= totalcolumn) );

		return ((row -1 ) * totalcolumn + (column - 1));
	}
}