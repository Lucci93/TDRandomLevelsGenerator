using UnityEngine;

public class Node : IHeapItem<Node> {

	public bool walkable;
	public int gridX;
	public int gridY;
	public Node parent;

	public int gCost;
	public int hCost;

	public int FCost {
		get {
			return gCost + hCost;
		}
	}

	int heapIndex;

	public Node(bool _walkable, int _gridX, int _gridY) {
		walkable = _walkable;
		gridX = _gridX;
		gridY = _gridY;
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
