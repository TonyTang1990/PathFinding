using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue {

}

public class Heap
{
	private List<KeyValuePair<int, float>> mList;
	
	public Heap(List<KeyValuePair<int,float>> list)
	{
		mList = list;
		
		HeapSort();
	}
	
	public void PrintOutAllMember()
	{
		foreach (KeyValuePair<int,float> valuepair in mList)
		{
			Console.WriteLine(valuepair.ToString());
		}
	}
	
	public void Insert(KeyValuePair<int,float> valuepair)
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
		if (left_child_index < length && mList[left_child_index].Value > mList[max_index].Value)
		{
			max_index = left_child_index;
		}
		
		if (right_child_index < length && mList[right_child_index].Value > mList[max_index].Value)
		{
			max_index = right_child_index;
		}
		
		//If any child is bigger than parent, 
		//then we swap it and do adjust for child again to make sure meet max heap definition
		if (max_index != parentindex)
		{
			//swap_time++;
			Swap(mList[max_index], mList[parentindex]);
			HeapAdjust(max_index, length);
		}
	}
	
	//通过初试数据构建最大堆
	private void BuildingHeap()
	{
		for( int i = mList.Count/2 - 1; i >= 0; i--)
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
				Swap(mList[i], mList[0]);
				//Due to we already building max heap before,
				//so  we just need to adjust for index 0 after we swap first and last element
				HeapAdjust(0, i);
			}
		}
		else
		{
			Console.WriteLine("mList == null");
		}
	}
	
	private void Swap(KeyValuePair<int,float> kvp1,KeyValuePair<int,float> kvp2)
	{
		KeyValuePair<int,float> temp;
		temp = kvp1;
		kvp1 = kvp2;
		kvp2 = temp;
	}
}