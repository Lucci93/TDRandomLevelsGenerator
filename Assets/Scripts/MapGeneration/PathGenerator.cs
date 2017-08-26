using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class PathGenerator : MonoBehaviour {

	public GameObject node;
	public GameObject ground;
	public int threshold;
	// map generator can menage a lot of different map
	public Map[] maps;

    // contains all the tile
    Coord[] coordTales;
	// get a reference to the map
	Map currentMap;
    // current map index
    int mapIndex;
	// parent of the grid
	Transform parentT;
	// store all the shuffled tiles coords
	Queue<Coord> shuffledTilesCoords;

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
		// just for debug
		Stopwatch sw = new Stopwatch();
		sw.Start();
        // choose the map
        mapIndex++;
		// generate the map with a random path
		GenerateMap();
		// create a path for the enemies
		// draw the map
		CreateMap(CreatePath());
		sw.Stop();
		// record 311 ms
		UnityEngine.Debug.Log("Path found in: " + sw.ElapsedMilliseconds + " ms");
	}

    // create a path with A*
    Coord[] CreatePath() {
		while (true) {
			// use the pathfinding A* algorithm to reach a target
			PathfindingAStar pathfinder = new PathfindingAStar(coordTales, currentMap, currentMap.start, currentMap.end);
			// search a path
			List<Node> path = new List<Node>(pathfinder.FindPath());
			if (path.Count > 1) {
				// found
                return Utility.NodeToCoordArray(path.ToArray(), currentMap);
			}
            // use the nearest point to the target to carve a path
            CarvePathInGrid(path[0]);
		}
    }

    // generate a hole between the nearest point to the target and the target in the grid
    void CarvePathInGrid(Node minNode) {
        Coord freeCoord = new Coord(minNode.gridX, minNode.gridY, minNode.walkable);
        while (true) {
			Coord nearObstacle = GetNeighboursObstaclesCoord(freeCoord)[0];
			// remove obstacle to path
			coordTales[Utility.FindCoordIndex(coordTales, nearObstacle.x, nearObstacle.y)].isGround = false;
            Coord direction = GetDirection(nearObstacle, freeCoord);

            // check if in this direction there is another obstacle
            if (coordTales[Utility.FindCoordIndex(coordTales, nearObstacle.x + direction.x, nearObstacle.y + direction.y)].isGround) {
                // repeat
                freeCoord = nearObstacle;
    		}
            else {
                return;
            }
        }
    }

    // get the direction between two coords
    Coord GetDirection(Coord newCoord, Coord oldCoord) {
        int x = newCoord.x - oldCoord.x;
        int y = newCoord.y - oldCoord.y;
        return new Coord(x, y, false);
    }

    // add path to the map
    void FillWithPath(Coord[] path) {
		int count = 0;
        for (int x = 0; x < coordTales.Length; x++) {
            for (int y = 0; y < path.Length; y++) {
				if (path[y].x == coordTales[x].x && path[y].y == coordTales[x].y) {
                    // set as ground
                    coordTales[x].isGround = true;
                    count++;
					break;
				}
			}
            // check all the path
            if (count >= path.Length) {
                return;
            }
        }
        coordTales[Utility.FindCoordIndex(coordTales, currentMap.start.x, currentMap.start.y)].isGround = true;
        coordTales[Utility.FindCoordIndex(coordTales, 0, 0)].isGround = false;
    }

    // generate the map
    void CreateMap(Coord[] path) {
        // reset map
        InitializeMatrix();
        // generate the path
        FillWithPath(path);

		int posX = 0;
        int count = 0;
        for (int x = 0; x < currentMap.mapSize.x; x++) {
			int posY = 0;
			for (int y = 0; y < currentMap.mapSize.y; y++) {
                // instantiate node
                if (!coordTales[count].isGround) {
                    Instantiate(node, new Vector3(posX, 0f, posY), Quaternion.identity, parentT);
				}
				// instatiate ground
				else {
					Instantiate(ground, new Vector3(posX, 0f, posY), Quaternion.identity, parentT);
				}
				posY += threshold;
                count++;
			}
			posX += threshold;
		}
	}

    void InitializeMatrix() {
		int count = 0;
		for (int x = 0; x < currentMap.mapSize.x; x++) {
			for (int y = 0; y < currentMap.mapSize.y; y++) {
				// node add to grid
				coordTales[count] = new Coord(x, y, false);
				count++;
			}
		}
    }

	void GenerateMap() {
		// create the matrix
		currentMap = maps[mapIndex-1];
		coordTales = new Coord[currentMap.mapSize.x * currentMap.mapSize.y];

		// add color to the map elements
		ground.GetComponent<MeshRenderer>().sharedMaterial.color = currentMap.groundsColor;
		node.GetComponent<MeshRenderer>().sharedMaterial.color = currentMap.nodesColor;

        // reset map
        ResetMapGrid();

		InitializeMatrix();
		// get the shuffled tiles coords queue using the shuffled algorithm with the list of coords
		shuffledTilesCoords = new Queue<Coord>(Utility.ShuffleArray((Coord[])coordTales.Clone(), currentMap.seed));
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
			Coord randomCoord = GetRandomCoord();
            if (randomCoord != currentMap.start && randomCoord != currentMap.end) {
                int coordIndex = Utility.FindCoordIndex(coordTales, randomCoord.x, randomCoord.y);
                // assign map coordinate obstacles
                coordTales[coordIndex].isGround = true;
                currentGroundCount++;
                // check if randomCoord is not on the start or end alis spawning point and target and there is no obstacles
                if (!MapIsFullyAccessible(currentGroundCount)) {
                    // if i can't create a new obstacole in that position
                    // remove the obstacles
                    coordTales[coordIndex].isGround = false;
                    currentGroundCount--;
                }
            }
		}
	}

	// check if there are area locked for the movement of the player becaouse of obstacles
	bool MapIsFullyAccessible(int currentObstacleCount) {
        // copy of the map obstacle
        Coord[] mapFlags = (Coord[])coordTales.Clone();
		Queue<Coord> queue = new Queue<Coord>();
		// we start by start and end because we are sure that are not fill
		// add start and end to the queue and set it on true on the map
        queue.Enqueue(currentMap.start);
        mapFlags[Utility.FindCoordIndex(mapFlags, currentMap.start.x, currentMap.start.y)].isGround = true;
		queue.Enqueue(currentMap.end);
		mapFlags[Utility.FindCoordIndex(mapFlags, currentMap.end.x, currentMap.end.y)].isGround = true;

		// keep track of all the tile visited, it start from one because the center tiles is accessible
		int accessibleTileCount = 2;

		// until there are coordinates in our queue
		while (queue.Count > 0) {
			// get the first item on the queue
			Coord tile = queue.Dequeue();
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
                            int neighbourIndex = Utility.FindCoordIndex(mapFlags, neighbourX, neighbourY);
                            int groundIndex = Utility.FindCoordIndex(coordTales, neighbourX, neighbourY);
							// we have not checked this tile and is not an obstacle
                            if (!mapFlags[neighbourIndex].isGround && !coordTales[groundIndex].isGround) {
								// so is free from obstacles
                                mapFlags[neighbourIndex].isGround = true;
								queue.Enqueue(new Coord(neighbourX, neighbourY, true));
								accessibleTileCount++;
							}
						}
					}
				}
			}
		}
		// how many tiles should there be
		int targetAccessibleTileCount = currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount;
		return targetAccessibleTileCount == accessibleTileCount;
	}

	// return all the neighbours 
    List<Coord> GetNeighboursObstaclesCoord(Coord currentNode) {
        List<Coord> neighbours = new List<Coord>();
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
                if (checkX > 0 && checkX < currentMap.mapSize.x-1 && checkY > 0 && checkY < currentMap.mapSize.y-1 && coordTales[Utility.FindCoordIndex(coordTales, checkX, checkY)].isGround) {
                    // Add to the neighbours
                    neighbours.Add(coordTales[Utility.FindCoordIndex(coordTales, checkX, checkY)]);
				}
			}
		}
		return neighbours;
	}

	Coord GetRandomCoord() {
		// we take the first element in the queue
		Coord randomCoord = shuffledTilesCoords.Dequeue();
		// we enqueue it at the end of the queue
		shuffledTilesCoords.Enqueue(randomCoord);
		// we return the random coord
		return randomCoord;
	}
}
