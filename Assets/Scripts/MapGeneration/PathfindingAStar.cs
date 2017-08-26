using System.Collections.Generic;
using UnityEngine;

public class PathfindingAStar {

    public Node[] grid;
    readonly Map currentMap;
    Node startNode;
    Node endNode;

    public PathfindingAStar(Coord[] grid, Map currentMap) {
        this.grid = Utility.CoordToNodeArray(grid, currentMap);
		this.currentMap = currentMap;
        startNode = FindNodeFromCoord(currentMap.start);
        endNode = FindNodeFromCoord(currentMap.end);
    }

	// find a path from a start to a target point
    public List<Node> FindPath() {
        Heap<Node> openSet = new Heap<Node>(currentMap.MaxSize);
		HashSet<Node> closedSet = new HashSet<Node>();
        // used to get the min distance from the target if not reached to carve a path until it
        Node minNode = startNode;
        minNode.hCost = GetDistance(startNode, endNode);
        // is initialize to the max distance
		openSet.Add(startNode);

		while (openSet.Count > 0) {
			Node currentNode = openSet.RemoveFirst();
			closedSet.Add(currentNode);

            // arrive
            if (currentNode == endNode) {
				return RetracePath();
			}

            foreach (Node neighbour in GetNeighboursNode(currentNode)) {
				// if the neighbour is not ok, check another
                if (!neighbour.walkable || closedSet.Contains(neighbour)) {
					continue;
				}
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
				// if is shorter or not in open set
				if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, endNode);
					neighbour.parent = currentNode;
					// check if is the nearest to the end node
                    if (neighbour.hCost < minNode.hCost) {
                        minNode = neighbour;
					}
				}
				// check if is not in the open set and ad to it
				if (!openSet.Contains(neighbour)) {
					openSet.Add(neighbour);
				}
				else {
					openSet.UpdateItem(neighbour);
				}
			}
		}
        // not find a path
        return new List<Node> { minNode };
	}

	// get distance between two node
	int GetDistance(Node nodeA, Node nodeB) {
		int distX = Mathf.Abs(nodeA.x - nodeB.x);
		int distY = Mathf.Abs(nodeA.y - nodeB.y);
		if (distX > distY) {
            return (Constant.DIAGONAL_WEIGHT * distY) + Constant.VERT_OR_HOR_WEIGHT * (distX - distY);
		}
        return (Constant.DIAGONAL_WEIGHT * distX) + Constant.VERT_OR_HOR_WEIGHT * (distY - distX);
	}

	// retrive the path searched from parent nodes
	List<Node> RetracePath() {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
        return path;
	}

	// return all the neighbours 
	List<Node> GetNeighboursNode(Node currentNode) {
		List<Node> neighbours = new List<Node>();
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
                // not check diagonals
				if (x == 0 && y == 0 || x == 1 && y == 1 || x == 1 && y == -1 || x == -1 && y == 1 || x == -1 && y == -1) {
					continue;
				}
				// check if is inside the grid
				int checkX = currentNode.x + x;
				int checkY = currentNode.y + y;
				if (checkX > 0 && checkX < currentMap.mapSize.x-1 && checkY > 0 && checkY < currentMap.mapSize.y-1) {
					// Add to the neighbours
                    neighbours.Add(grid[Utility.FindNodeIndex(grid, checkX, checkY)]);
				}
			}
		}
		return neighbours;
	}

	// search a node in the grid
	public Node FindNodeFromCoord(Coord coord) {
		foreach (Node node in grid) {
			if (node.x == coord.x && node.y == coord.y) {
				return node;
			}
		}
		return null;
	}
}
