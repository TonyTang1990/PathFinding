﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<T1, T2>
{
	public PriorityQueue()
	{
		mHeap = new Heap<T1, T2>();
	}
	
	public PriorityQueue(Heap<T1, T2> heap)
	{
		mHeap = heap;
	}

	public PriorityQueue(List<KeyValuePair<T1,T2>> key)
	{
		mHeap = new Heap<T1, T2> (key);
	}
	
	public bool Empty()
	{
		return (mHeap.Size() == 0);
	}
	
	public void Push(KeyValuePair<T1, T2> kvp)
	{
		mHeap.Insert(kvp);
	}
	
	public KeyValuePair<T1, T2> Pop()
	{
		KeyValuePair<T1,T2> result = mHeap.Top();
		mHeap.RemoveTop();
		return result;
	}
	
	public int Size()
	{
		return mHeap.Size();
	}
	
	public KeyValuePair<T1, T2> Top()
	{
		return mHeap.Top(); ;
	}
	
	public void PrintOutAllMember()
	{
		mHeap.PrintOutAllMember();
	}
	
	private Heap<T1, T2> mHeap;
}

public class Heap<T1, T2>
{
	private List<KeyValuePair<T1, T2>> mList;
	private IComparer<T2> mComparer;
	private int mCount;
	
	public Heap()
	{
		mList = new List<KeyValuePair<T1, T2>>();
		mComparer = Comparer<T2>.Default;
		mCount = 0;
	}
	
	public Heap(List<KeyValuePair<T1, T2>> list)
	{
		mList = list;
		mCount = list.Count;
		mComparer = Comparer<T2>.Default;
		BuildingHeap();
	}
	
	public int Size()
	{
		if (mList != null)
		{
			return mCount;
		}
		else
		{
			return 0;
		}
	}
	
	//O(Log(N))
	public void RemoveTop()
	{
		if (mList != null)
		{
			mList[0] = mList[mCount - 1];
			mList.RemoveAt(mCount-1);
			mCount--;
			HeapifyFromBeginningToEnd(0,mCount - 1);
		}
	}
	
	public KeyValuePair<T1, T2> Top()
	{
		if (mList != null)
		{
			return mList[0];
		}
		else
		{
			//No more member
			throw new InvalidOperationException("Empty heap.");
		}
	}
	
	public void PrintOutAllMember()
	{
		foreach (KeyValuePair<T1, T2> valuepair in mList)
		{
			Debug.Log(valuepair.ToString());
		}
	}
	
	//O(Log(N))
	public void Insert(KeyValuePair<T1, T2> valuepair)
	{
		mList.Add(valuepair);
		mCount++;
		HeapifyFromEndToBeginning(mCount - 1);
	}
	
	//调整堆确保堆是最大堆，这里花O(log(n))，跟堆的深度有关
	private void HeapifyFromBeginningToEnd(int parentindex, int length)
	{
		int max_index = parentindex;
		int left_child_index = parentindex * 2 + 1;
		int right_child_index = parentindex * 2 + 2;
		
		//Chose biggest one between parent and left&right child
		if (left_child_index < length && mComparer.Compare(mList[left_child_index].Value, mList[max_index].Value) < 0)
		{
			max_index = left_child_index;
		}
		
		if (right_child_index < length && mComparer.Compare(mList[right_child_index].Value, mList[max_index].Value) < 0)
		{
			max_index = right_child_index;
		}
		
		//If any child is bigger than parent, 
		//then we swap it and do adjust for child again to make sure meet max heap definition
		if (max_index != parentindex)
		{
			Swap(max_index, parentindex);
			HeapifyFromBeginningToEnd(max_index, length);
		}
	}
	
	//O(log(N))
	private void HeapifyFromEndToBeginning(int index)
	{
		if(index >= mCount)
		{
			return;
		}
		while (index > 0)
		{
			int parentindex = (index - 1) / 2;
			if(mComparer.Compare(mList[parentindex].Value,mList[index].Value) > 0)
			{
				Swap(parentindex, index);
				index = parentindex;
			}
			else
			{
				break;
			}
		}
	}
	
	//通过初试数据构建最大堆
	////O(N*Log(N))
	private void BuildingHeap()
	{
		if (mList != null)
		{
			for (int i = mList.Count / 2 - 1; i >= 0; i--)
			{
				//1.2 Adjust heap
				//Make sure meet max heap definition
				//Max Heap definition:
				// (k(i) >= k(2i) && k(i) >= k(2i+1))   (1 <= i <= n/2)
				HeapifyFromBeginningToEnd(i, mList.Count);
			}
		}
	}
	
	////O(N*log(N))
	private void HeapSort()
	{
		if (mList != null)
		{
			//Steps:
			// 1. Build heap
			// 1.1 Init heap
			// 1.2 Adjust heap
			// 2. Sort heap
			
			//1. Build max heap
			// 1.1 Init heap
			//Assume we construct max heap
			BuildingHeap();
			//2. Sort heap
			//这里花O(n)，跟数据数量有关
			for (int i = mList.Count - 1; i > 0; i--)
			{
				//swap first element and last element
				//do adjust heap process again to make sure the new array are still max heap
				Swap(i, 0);
				//Due to we already building max heap before,
				//so  we just need to adjust for index 0 after we swap first and last element
				HeapifyFromBeginningToEnd(0, i);
			}            
		}
		else
		{
			Debug.Log("mList == null");
		}
	}
	
	private void Swap(int id1, int id2)
	{
		KeyValuePair<T1, T2> temp;
		temp = mList[id1];
		mList[id1] = mList[id2];
		mList[id2] = temp;
	}
}