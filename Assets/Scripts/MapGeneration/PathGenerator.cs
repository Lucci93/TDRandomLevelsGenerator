using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour {

	public GameObject node;
	public GameObject ground;
	public int threshold;
	// map generator can menage a lot of different map
	public Map[] maps;

    // contains all the tile
    List<Node> grid;
	// get a reference to the map
    Map currentMap;
    // current map index
    int mapIndex;
	// parent of the grid
	Transform parentT;
	// store all the shuffled tiles coords
	Queue<Node> shuffledTilesCoords;

	void Awake() {
        // first level
        NewMap();
	}

    void Update() {
        // move to the next map on N press if exist
        if (mapIndex < maps.Length && Input.GetKeyDown(KeyCode.N)) {
            NewMap();
        }
    }

    void NewMap() {
        // choose the map
        mapIndex++;
		// generate the map with a random path
		GenerateMap();
		// create a path for the enemies
		// use the pathfinding A* algorithm to reach a target
		PathfindingAStar pathfinder = new PathfindingAStar(grid, currentMap);
		// draw the map
		CreateMap(pathfinder.FindPath());
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

    // add path to the map
    void FillWithPath(List<Node> path) {
        int count = 0;
        foreach (Node item in grid) {
            // node found
            if (path.Exists(e => e.x == item.x && e.y == item.y)) {
				// set as path
				item.notWalkable = true;
                count++;
            }
            // stop the foreach if all the node are setted
            if (count == path.Count) {
                break;
            }
        }
        grid.Find(e => e.x == currentMap.start.x && e.y == currentMap.start.y).notWalkable = true;
        grid.Find(e => e.x == 0 && e.y == 0).notWalkable = false;
    }

    // generate the map
    void CreateMap(List<Node> path) {
        // reset map
        InitializeMatrix();
        // generate the path
        FillWithPath(path);

        // draw grid map
        foreach (Node item in grid) {
            int posX = item.x * threshold;
            int posY = item.y * threshold;
            // instantiate free cell
            if (item.notWalkable) {
                Instantiate(ground, new Vector3(posX, 0f, posY), Quaternion.identity, parentT);
            }
            // instatiate path cell
            else {
                Instantiate(node, new Vector3(posX, 0f, posY), Quaternion.identity, parentT);
            }
        }
	}

    void InitializeMatrix() {
		grid = new List<Node>();
		for (int x = 0; x < currentMap.mapSize.x; x++) {
			for (int y = 0; y < currentMap.mapSize.y; y++) {
				// node add to grid
                grid.Add(new Node(x, y, false));
			}
		}
    }

	void GenerateMap() {
		// create the matrix
        currentMap = maps[mapIndex-1];

		// add color to the map elements
		ground.GetComponent<MeshRenderer>().sharedMaterial.color = currentMap.groundsColor;
		node.GetComponent<MeshRenderer>().sharedMaterial.color = currentMap.nodesColor;

        // reset map
        ResetMapGrid();

		InitializeMatrix();
		// get the shuffled tiles coords queue using the shuffled algorithm with the list of coords
        shuffledTilesCoords = new Queue<Node>(Utility.ShuffleList(new List<Node>(grid), currentMap.seed));
        // Add obstacles to the map
        AddObstacles();
	}

    // reinitialize map
    void ResetMapGrid() {
		// name of the object that store all the tile
		string holderName = "Level Grid";
		// if the object exists
		if (transform.Find(holderName)) {
            parentT = null;
			// destroy before recreate it
			DestroyImmediate(transform.Find(holderName).gameObject);
		}
		// create a new object
        parentT = new GameObject(holderName).transform;
		// set mapgenerator like parent
		parentT.parent = transform;
    }

	void AddObstacles() {
		// number of obstacles based on the size of the map
        int groundCount = (int)(currentMap.groundPercent * currentMap.mapSize.x * currentMap.mapSize.y);
		// total of instantiated obstacles 
		int currentGroundCount = 0;

		// spawn the obstacol based on the obstacle count
		for (int i = 0; i < groundCount; i++) {
			Node randomCoord = GetRandomCoord();
            if (randomCoord != currentMap.start && randomCoord != currentMap.end) {
                // assign map coordinate obstacles
                grid.Find(e => e.x == randomCoord.x && e.y == randomCoord.y).notWalkable = true;
                currentGroundCount++;
                // check if randomCoord is not on the start or end alis spawning point and target and there is no obstacles
                if (!MapIsFullyAccessible(currentGroundCount)) {
                    // if i can't create a new obstacole in that position
                    // remove the obstacles
                    grid.Find(e => e.x == randomCoord.x && e.y == randomCoord.y).notWalkable = false;
                    currentGroundCount--;
                }
            }
		}
	}

	// check if there are area locked for the movement of the player becaouse of obstacles
	bool MapIsFullyAccessible(int currentObstacleCount) {
        // copy of the map obstacle
        List<Node> mapFlags = new List<Node>(grid);
		Queue<Node> queue = new Queue<Node>();
		// we start by start and end because we are sure that are not fill
		// add start and end to the queue and set it on true on the map
        queue.Enqueue(currentMap.start);
        mapFlags.Find(e => e.x == currentMap.start.x && e.y == currentMap.end.y).notWalkable = true;
		queue.Enqueue(currentMap.end);
        mapFlags.Find(e => e.x == currentMap.end.x && e.y == currentMap.end.y).notWalkable = true;

		// keep track of all the tile visited, it start from one because the center tiles is accessible
		int accessibleTileCount = 2;

		// until there are coordinates in our queue
		while (queue.Count > 0) {
			// get the first item on the queue
			Node tile = queue.Dequeue();
			// we loop through all the adjacent element of the tile
			for (int x = -1; x <= 1; x++) {
				for (int y = -1; y <= 1; y++) {
					// set the adjacent tiles coord
					int neighbourX = tile.x + x;
					int neighbourY = tile.y + y;
					// in that way we don't check the diagonals
					if (x == 0 || y == 0) {
						// if we are inside the obstacle map
                        if (neighbourX >= 0 && neighbourX < currentMap.mapSize.x && neighbourY >= 0 && neighbourY < currentMap.mapSize.y){
							// we have not checked this tile and is not an obstacle
                            if (!mapFlags.Find(e => e.x == neighbourX && e.y == neighbourY).notWalkable && !grid.Find(e => e.x == neighbourX && e.y == neighbourY).notWalkable) {
								// so is free from obstacles
                                mapFlags.Find(e => e.x == neighbourX && e.y == neighbourY).notWalkable = true;
								queue.Enqueue(new Node(neighbourX, neighbourY, true));
								accessibleTileCount++;
							}
						}
					}
				}
			}
		}
		// how many tiles should there be
        int targetAccessibleTileCount = (currentMap.mapSize.x * currentMap.mapSize.y) - currentObstacleCount;
		return targetAccessibleTileCount == accessibleTileCount;
	}

	Node GetRandomCoord() {
		// we take the first element in the queue
		Node randomCoord = shuffledTilesCoords.Dequeue();
		// we enqueue it at the end of the queue
		shuffledTilesCoords.Enqueue(randomCoord);
		// we return the random coord
		return randomCoord;
	}
}
