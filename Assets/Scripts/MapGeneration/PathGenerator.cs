using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour {

	public GameObject freeCellObject;
	public GameObject pathObject;
    // star and end of the enemies
    public GameObject startObject;
    public GameObject endObject;
    public bool addStartAndEndObject;
    public int cellThreshold;
	// weight of a diagonal movement in the pathfinding
	public int diagonalWeight;
	// weight of horizontal or vertical movement in the pathfinding
    public int verticalWeight;
    public int horizontalWeight;
	// map generator can menage a lot of different map
	public Map[] maps;

    // contains all the tile
    List<Node> grid;
	// get a reference to the map
    Map currentMap;
    // current map index
    int mapIndex;
	// parent of the grid for free and path cells
	Transform freeT;
    Transform pathT;
	// store all the shuffled tiles coords
	Queue<Node> shuffledTilesCoords;

    // ad the start of the game
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
        PathfindingAStar pathfinder = new PathfindingAStar(grid, currentMap, diagonalWeight, verticalWeight, horizontalWeight);
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
                int checkY = currentNode.z + y;
                if (checkX > 0 && checkX < currentMap.mapSize.x-1 && checkY > 0 && checkY < currentMap.mapSize.z-1) {
                    // Add to the neighbours
                    neighbours.Add(grid.Find(e => e.x == checkX && e.z == checkY));
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
            if (path.Exists(e => e.x == item.x && e.z == item.z)) {
                // set as path
				item.notFreeCell = true;
                item.parent = path.Find(e => e.x == item.x && e.z == item.z).parent;
                count++;
            }
            // stop the foreach if all the node are setted
            if (count == path.Count) {
                break;
            }
        }
        grid.Find(e => e.x == currentMap.start.x && e.z == currentMap.start.z).notFreeCell = true;
        grid.Find(e => e.x == 0 && e.z == 0).notFreeCell = false;
    }

    // add neighbours of each path item so we can know how to connect path cells
    bool[] FindParent(Node item) {
        bool[] parent = new bool[4];
        int x = item.parent.x - item.x;
        int z = item.parent.z - item.z;
		// north
		if (x == 0 && z == 1) {
			parent[0] = true;
		}
		// right
		else if (x == 1 && z == 0) {
			parent[3] = true;
		}
		// left
		else if (x == -1 && z == 0) {
			parent[2] = true;
		}
		// south
		else {
			parent[1] = true;
		}
        return parent;
    }

    // resize the path to connet with the other cells
    // the double return is composed by [scaleX, scaleZ, posX, posZ] to add to the gameobject instantiate
    float[] ResizeCell(Node item) {
        if (item.x == currentMap.start.x && item.z == currentMap.start.z) {
			return new float[] {
    			0f, 0f, 0f, 0f
            };
        }
        bool[] neighboursArray = FindParent(item);
        // north
        if (neighboursArray[0]) {
            return new float[] {
                0f, cellThreshold - pathObject.transform.localScale.z, 0f, (cellThreshold - pathObject.transform.localScale.z) / 2f
            };
        }
        // south
        if (neighboursArray[1]) {
            return new float[] {
                0f, cellThreshold - pathObject.transform.localScale.z, 0f, (pathObject.transform.localScale.z - cellThreshold) / 2f
            };
        }
        // left
        if (neighboursArray[2]) {
            return new float[] {
                cellThreshold - pathObject.transform.localScale.x, 0f, (pathObject.transform.localScale.x - cellThreshold) / 2f, 0f
            };
        }
        // right
        return new float[] {
            cellThreshold - pathObject.transform.localScale.x, 0f, (cellThreshold - pathObject.transform.localScale.x) / 2f, 0f
        };
    }

    // generate the map
    void CreateMap(List<Node> path) {
        // reset map
        InitializeMatrix();
        // generate the path
        FillWithPath(path);
        // draw grid map
        foreach (Node item in grid) {
            int posX = item.x * cellThreshold;
            int posZ = item.z * cellThreshold;
            // is the path
            if (item.notFreeCell) {
                // if is the start object
                if (posX == (currentMap.start.x * cellThreshold) && posZ == (currentMap.start.z * cellThreshold) && addStartAndEndObject) {
                    Instantiate(startObject, new Vector3(posX, ((startObject.transform.localScale.y / 2f) + (pathObject.transform.localScale.y /2f)), posZ), Quaternion.identity, pathT);
                }
				// if is the end object
                else if (posX == (currentMap.end.x * cellThreshold) && posZ == (currentMap.end.z * cellThreshold) && addStartAndEndObject) {
                    Instantiate(endObject, new Vector3(posX, ((endObject.transform.localScale.y / 2f) + (pathObject.transform.localScale.y / 2f)), posZ), Quaternion.identity, pathT);
                }
				float[] paramethers = ResizeCell(item);
				// instantiate path cell resized
                GameObject cell = Instantiate(pathObject, new Vector3(posX + paramethers[2], 0f, posZ + paramethers[3]), Quaternion.identity, pathT);
                cell.transform.localScale = new Vector3(cell.transform.localScale.x + paramethers[0], cell.transform.localScale.y, cell.transform.localScale.z + paramethers[1]);
            }
            // instatiate free cell
            else {
                Instantiate(freeCellObject, new Vector3(posX, 0f, posZ), Quaternion.identity, freeT);
            }
        }
	}

    void InitializeMatrix() {
		grid = new List<Node>();
		for (int x = 0; x < currentMap.mapSize.x; x++) {
			for (int y = 0; y < currentMap.mapSize.z; y++) {
				// node add to grid
                grid.Add(new Node(x, y, false));
			}
		}
    }

	void GenerateMap() {
		// create the matrix
        currentMap = maps[mapIndex-1];

		// add color to the map elements
        pathObject.GetComponent<MeshRenderer>().sharedMaterial.color = currentMap.pathCellColor;
        freeCellObject.GetComponent<MeshRenderer>().sharedMaterial.color = currentMap.freeCellColor;
        if (addStartAndEndObject) {
            startObject.GetComponent<MeshRenderer>().sharedMaterial.color = currentMap.startObjectColor;
            endObject.GetComponent<MeshRenderer>().sharedMaterial.color = currentMap.endObjectColor;
        }

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
			// destroy before recreate it
			DestroyImmediate(transform.Find(holderName).gameObject);
		}
        // create a new object
        Transform parentT = new GameObject(holderName).transform;
        freeT = new GameObject("Free Cells").transform;
        pathT = new GameObject("Path Cells").transform;
        freeT.parent = parentT;
        pathT.parent = parentT;
		// set mapgenerator like parent
		parentT.parent = transform;
    }

	void AddObstacles() {
		// number of obstacles based on the size of the map
        int groundCount = (int)(currentMap.groundPercent * currentMap.mapSize.x * currentMap.mapSize.z);
		// total of instantiated obstacles 
		int currentGroundCount = 0;

		// spawn the obstacol based on the obstacle count
		for (int i = 0; i < groundCount; i++) {
			Node randomCoord = GetRandomCoord();
            if (randomCoord != currentMap.start && randomCoord != currentMap.end) {
                // assign map coordinate obstacles
                grid.Find(e => e.x == randomCoord.x && e.z == randomCoord.z).notFreeCell = true;
                currentGroundCount++;
                // check if randomCoord is not on the start or end alis spawning point and target and there is no obstacles
                if (!MapIsFullyAccessible(currentGroundCount)) {
                    // if i can't create a new obstacole in that position
                    // remove the obstacles
                    grid.Find(e => e.x == randomCoord.x && e.z == randomCoord.z).notFreeCell = false;
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
        mapFlags.Find(e => e.x == currentMap.start.x && e.z == currentMap.end.z).notFreeCell = true;
		queue.Enqueue(currentMap.end);
        mapFlags.Find(e => e.x == currentMap.end.x && e.z == currentMap.end.z).notFreeCell = true;

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
					int neighbourY = tile.z + y;
					// in that way we don't check the diagonals
					if (x == 0 || y == 0) {
						// if we are inside the obstacle map
                        if (neighbourX >= 0 && neighbourX < currentMap.mapSize.x && neighbourY >= 0 && neighbourY < currentMap.mapSize.z){
							// we have not checked this tile and is not an obstacle
                            if (!mapFlags.Find(e => e.x == neighbourX && e.z == neighbourY).notFreeCell && !grid.Find(e => e.x == neighbourX && e.z == neighbourY).notFreeCell) {
								// so is free from obstacles
                                mapFlags.Find(e => e.x == neighbourX && e.z == neighbourY).notFreeCell = true;
								queue.Enqueue(new Node(neighbourX, neighbourY, true));
								accessibleTileCount++;
							}
						}
					}
				}
			}
		}
		// how many tiles should there be
        int targetAccessibleTileCount = (currentMap.mapSize.x * currentMap.mapSize.z) - currentObstacleCount;
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
