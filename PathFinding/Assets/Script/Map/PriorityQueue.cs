using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<T1,T2>
{
	public PriorityQueue()
	{
		mHeap = new Heap<T1,T2>();
	}
	
	public PriorityQueue(Heap<T1,T2> heap)
	{
		mHeap = heap;
	}
	
	public bool Empty()
	{
		return (mHeap.Size() == 0);
	}
	
	public void Push(KeyValuePair<T1, T2> kvp)
	{
		mHeap.Insert(kvp);
	}
	
	public void Pop()
	{
		mHeap.RemoveTop();
	}
	
	public int Size()
	{
		return mHeap.Size();
	}
	
	public KeyValuePair<T1,T2> Top()
	{
		return mHeap.Top(); ;
	}
	
	public void PrintOutAllMember()
	{
		mHeap.PrintOutAllMember();
	}
	
	private Heap<T1,T2> mHeap;
}

public class Heap<T1,T2>
{
	private List<KeyValuePair<T1, T2>> mList;
	private IComparer<T2> mComparer;
	
	public Heap()
	{
		mList = new List<KeyValuePair<T1, T2>>();
		mComparer = Comparer<T2>.Default;
	}
	
	public Heap(List<KeyValuePair<T1, T2>> list)
	{
		mList = list;
		mComparer = Comparer<T2>.Default;
		HeapSort();
	}
	
	public int Size()
	{
		if (mList != null)
		{
			return mList.Count;
		}
		else
		{
			return 0;
		}
	}
	
	public void RemoveTop()
	{
		if (mList != null)
		{
			mList.RemoveAt(0);
		}
	}
	
	public KeyValuePair<T1, T2> Top()
	{
		if(mList != null)
		{
			return mList[0];
		}
		else
		{
			//Note no member exist, so return default KeyValuePair
			return new KeyValuePair<T1, T2>();
		}
	}
	
	public void PrintOutAllMember()
	{
		foreach (KeyValuePair<T1, T2> valuepair in mList)
		{
			Debug.Log(valuepair.ToString());
		}
	}
	
	public void Insert(KeyValuePair<T1, T2> valuepair)
	{
		mList.Add(valuepair);
		HeapSort();
	}
	
	//调整堆确保堆是最大堆，这里花O(log(n))，跟堆的深度有关
	private void HeapAdjust(int parentindex, int length)
	{
		int max_index = parentindex;
		int left_child_index = parentindex * 2 + 1;
		int right_child_index = parentindex * 2 + 2;
		
		//Chose biggest one between parent and left&right child
		if (left_child_index < length && mComparer.Compare(mList[left_child_index].Value, mList[max_index].Value) > 0)
		{
			max_index = left_child_index;
		}
		
		if (right_child_index < length && mComparer.Compare(mList[right_child_index].Value, mList[max_index].Value) > 0)
		{
			max_index = right_child_index;
		}
		
		//If any child is bigger than parent, 
		//then we swap it and do adjust for child again to make sure meet max heap definition
		if (max_index != parentindex)
		{
			//swap_time++;
			Swap(max_index,parentindex/*mList[max_index], mList[parentindex]*/);
			HeapAdjust(max_index, length);
		}
	}
	
	//通过初试数据构建最大堆
	private void BuildingHeap()
	{
		for (int i = mList.Count / 2 - 1; i >= 0; i--)
		{
			//1.2 Adjust heap
			//Make sure meet max heap definition
			//Max Heap definition:
			// (k(i) >= k(2i) && k(i) >= k(2i+1))   (1 <= i <= n/2)
			HeapAdjust(i, mList.Count);
		}
	}
	
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
				Swap(i,0/*mList[i], mList[0]*/);
				//Due to we already building max heap before,
				//so  we just need to adjust for index 0 after we swap first and last element
				HeapAdjust(0, i);
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