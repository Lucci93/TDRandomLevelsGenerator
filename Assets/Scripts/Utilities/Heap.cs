using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T> {

	readonly T[] items;
	int currentItemCount;

	public Heap(int maxHeapSize) {
		items = new T[maxHeapSize];
	}

	public void Add(T item) {
		item.HeapIndex = currentItemCount;
		items[currentItemCount] = item;
		// set the position of the new element up
		SortUp(item);
        currentItemCount++;
	}

	public T RemoveFirst() {
		T firstItem = items[0];
		currentItemCount--;
		items[0] = items[currentItemCount];
		items[0].HeapIndex = 0;
		// set the position of the new element down
		SortDown(items[0]);
		return firstItem;
	}

	public bool Contains(T item) {
		return Equals(items[item.HeapIndex], item);
	}

	public int Count {
		get {
			return currentItemCount;
		}
	}

	// want to change the priority of an item
	public void UpdateItem(T item) {
		SortUp(item);
	}

	void SortDown(T item) {
		while (true) {
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;
			int swapIndex = 0;
			// check if at least item has a child
			if (childIndexLeft < currentItemCount) {
				swapIndex = childIndexLeft;
				// check if item has a right child
				if (childIndexRight < currentItemCount) {
					// check which of the two have the highest priority
					if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) {
						// if right has highest priority
						swapIndex = childIndexRight;
					}
				}
				// check if the parent has lowest priority
				if (item.CompareTo(items[swapIndex]) < 0) {
					// if it has, swap the two
					Swap(item, items[swapIndex]);
				}
				else {
					return;
				}
			}
			else {
				return;
			}
		}
	}

	void SortUp(T item) {
		int parentIndex = (item.HeapIndex - 1) / 2;
		while (true) {
			T parentItem = items[parentIndex];
			// swap if the child is less than parent
			if (item.CompareTo(parentItem) > 0) {
				Swap(item, parentItem);
			}
			else {
				return;
			}
			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	// swap two element between them
	void Swap(T itemA, T itemB) {
		items[itemA.HeapIndex] = itemB;
		items[itemB.HeapIndex] = itemA;
		int itemAIndex = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = itemAIndex;
	}
}

public interface IHeapItem<T> : IComparable<T> {
	int HeapIndex {
		get;
		set;
	}
}