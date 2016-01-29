using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPoolManager : MonoBehaviour{

    public static ObjectPoolManager mObjectPoolManagerInstance = null;

    public GameObject mBuildingBullet;

    public int mBBulletPoolAmount = 20;

    private List<GameObject> mBBulletsList;

    public GameObject mSoldierBullet;

    public int mSBulletPoolAmount = 50;

    private List<GameObject> mSBulletsList;

    public bool mWillGrow = true;

    void Awake()
    {
        if (mObjectPoolManagerInstance == null)
        {
            mObjectPoolManagerInstance = this;
        }
        else if (mObjectPoolManagerInstance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        mBBulletsList = new List<GameObject>();
        mSBulletsList = new List<GameObject>();

        for (int i = 0; i < mBBulletPoolAmount; i++)
        {
            GameObject bbulletobj = Instantiate(mBuildingBullet) as GameObject;
            bbulletobj.SetActive(false);
            mBBulletsList.Add(bbulletobj);
        }

        for (int j = 0; j < mSBulletPoolAmount; j++)
        {
            GameObject sbulletobj = Instantiate(mSoldierBullet) as GameObject;
            sbulletobj.SetActive(false);
            mSBulletsList.Add(sbulletobj);
        }
    }

    public GameObject GetBuildingBulletObject()
    {
        for (int i = 0; i < mBBulletsList.Count; i++)
        {
            if (!mBBulletsList[i].activeInHierarchy)
            {
                mBBulletsList[i].SetActive(true);
                return mBBulletsList[i];
            }
        }

        if (mWillGrow)
        {
            GameObject bbulletobj = Instantiate(mBuildingBullet) as GameObject;
            mBBulletsList.Add(bbulletobj);
            return bbulletobj;
        }

        return null;
    }

    public GameObject GetSoldierBulletObject()
    {
        for (int i = 0; i < mSBulletsList.Count; i++)
        {
            if (!mSBulletsList[i].activeInHierarchy)
            {
                mSBulletsList[i].SetActive(true);
                return mSBulletsList[i];
            }
        }

        if (mWillGrow)
        {
            GameObject sbulletobj = Instantiate(mSoldierBullet) as GameObject;
            mSBulletsList.Add(sbulletobj);
            return sbulletobj;
        }

        return null;
    }
}
