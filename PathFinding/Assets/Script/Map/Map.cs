using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class Map
{
    //public ArrayList mBuildings = new ArrayList();
    public List<BuildingInfo> mBuildings = new List<BuildingInfo>();

    public bool[,] mMapOccupied = new bool[MapManager.MMInstance.PathFinder.mRow, MapManager.MMInstance.PathFinder.mColumn];

    public void addBuilding(BuildingInfo building)
    {
        Utility.Log(string.Format("addBuilding() building.getPosition().mX = {0} mY = {1} mZ = {2}", building.Position.x, building.Position.y, building.Position.z));
        mBuildings.Add(building);
    }

    public void removeBuilding(BuildingInfo building)
    {
        Utility.Log(string.Format("removeBuilding() building.getPosition().mX = {0} mY = {1} mZ = {2}", building.Position.x, building.Position.y, building.Position.z));
        /*
        if(mBuildings.Contains(building))
        {
            int removeindex = mBuildings.IndexOf(building);
            mBuildings.RemoveAt(removeindex);
        }
        */
        mBuildings.RemoveAll(item => item.mIndex == building.mIndex);
    }

    public List<BuildingInfo> getBuildings()
    //public ArrayList getBuildings()
    {
        return mBuildings;
    }

    public void setMapOccupiedInfo(int row, int column, bool isoccupied)
    {
        mMapOccupied[row, column] = isoccupied;
    }

    public bool getMapOccupiedInfo(int row, int column)
    {
        if (mMapOccupied.Length == 0)
        {
            Utility.Log("mMapOccupied.Length == 0");
        }
        return mMapOccupied[row, column];
    }
}
