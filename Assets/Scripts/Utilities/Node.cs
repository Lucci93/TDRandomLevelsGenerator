using UnityEngine;

[System.Serializable]
public class Node : IHeapItem<Node> {

	public int x;
	public int y;

    [HideInInspector]
	public bool notWalkable;
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

	public Node(int x, int y, bool walkable) {
		this.notWalkable = walkable;
		this.x = x;
		this.y = y;
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
