using UnityEngine;

[System.Serializable]
public class Node : IHeapItem<Node> {

	public int x;
	public int z;

    [HideInInspector]
	public bool notFreeCell;
    [HideInInspector]
	public Node parent;
    [HideInInspector]
	public int gCost;
    [HideInInspector]
	public int hCost;

	public int FCost {
		get {
			return gCost + hCost;
		}
	}

	int heapIndex;

	public Node(int x, int z, bool notFreeCell) {
		this.notFreeCell = notFreeCell;
		this.x = x;
		this.z = z;
	}

	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare) {
		int compare = FCost.CompareTo(nodeToCompare.FCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		// we return 1 if is lower
		return -compare;
	}
}
