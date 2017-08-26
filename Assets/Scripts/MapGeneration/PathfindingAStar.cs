using System.Collections.Generic;
using UnityEngine;

public class PathfindingAStar {

    public List<Node> grid;

    readonly Map currentMap;

    Node startNode;
    Node endNode;
	// weight of a diagonal movement in the pathfinding
	int diagonalWeight;
	// weight of horizontal or vertical movement in the pathfinding
    int verticalWeight;
    int horizontalWeight;

    public PathfindingAStar(List<Node> grid, Map currentMap, int diagonalWeight, int verticalWeight, int horizontalWeight) {
        this.grid = grid;
		this.currentMap = currentMap;
        this.diagonalWeight = diagonalWeight;
        this.verticalWeight = verticalWeight;
        this.horizontalWeight = horizontalWeight;
        startNode = grid.Find(e => e.x == currentMap.start.x && e.y == currentMap.start.y);
        endNode = grid.Find(e => e.x == currentMap.end.x && e.y == currentMap.end.y);
    }

	// find a path from a start to a target point
    public List<Node> FindPath() {
		// repeat until a valid path will be found
		while (true) {
            Heap<Node> openSet = new Heap<Node>(grid.Count);
            HashSet<Node> closedSet = new HashSet<Node>();
            // used to get the min distance from the target if not reached to carve a path until it
            Node minNode = startNode;
            minNode.hCost = GetDistance(startNode, endNode);
            // is initialize to the max distance
            openSet.Add(startNode);

            while (openSet.Count > 0) {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                // arrived
                if (currentNode == endNode) {
                    return RetracePath();
                }

                foreach (Node neighbour in GetNeighboursNode(currentNode)) {
                    // if the neighbour is not ok, check another
                    if (neighbour.notWalkable || closedSet.Contains(neighbour)) {
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

            // path not found try to carve
            CarvePathInGrid(minNode);
        }
	}

	// generate a hole between the nearest point to the target and the target in the grid
	void CarvePathInGrid(Node minNode) {
		while (true) {
			Node nearObstacle = GetNeighboursObstaclesCoord(minNode);
			// remove obstacle to path
			grid.Find(e => e.x == nearObstacle.x && e.y == nearObstacle.y).notWalkable = false;
			Node direction = GetDirectionNode(nearObstacle, minNode);

			// check if in this direction there is another obstacle
			if (grid.Find(e => e.x == direction.x && e.y == direction.y).notWalkable) {
				// repeat
				minNode = nearObstacle;
			}
			else {
				return;
			}
		}
	}

	// get the position of the next node in the direction of the new coord
	Node GetDirectionNode(Node newCoord, Node oldCoord) {
		int x = newCoord.x - oldCoord.x;
		int y = newCoord.y - oldCoord.y;
		return new Node(x + newCoord.x, y + newCoord.y, false);
	}

	// get distance between two node
	int GetDistance(Node nodeA, Node nodeB) {
		int distX = Mathf.Abs(nodeA.x - nodeB.x);
		int distY = Mathf.Abs(nodeA.y - nodeB.y);
		if (distX > distY) {
            return (diagonalWeight * distY) + (horizontalWeight * distX) - (verticalWeight * distY);
		}
        return (diagonalWeight * distX) + (verticalWeight * distY) - (horizontalWeight * distX);
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
                    neighbours.Add(grid.Find(e => e.x == checkX && e.y == checkY));
				}
			}
		}
		return neighbours;
	}

	// return all the neighbours 
	Node GetNeighboursObstaclesCoord(Node currentNode) {
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				// not check diagonals
				if (x == 0 && y == 0 || x == 1 && y == 1 || x == 1 && y == -1 || x == -1 && y == 1 || x == -1 && y == -1) {
					continue;
				}
				// check if is inside the grid
				int checkX = currentNode.x + x;
				int checkY = currentNode.y + y;
				// chek if is out of bounds and a ostacle
				if (checkX > 0 && checkX < currentMap.mapSize.x - 1 && checkY > 0 && checkY < currentMap.mapSize.y - 1 && grid.Find(e => e.x == checkX && e.y == checkY).notWalkable) {
					// return to the neighbours
					return grid.Find(e => e.x == checkX && e.y == checkY);
				}
			}
		}
        // impossible
		return null;
	}
}
