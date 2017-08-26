using UnityEngine;

// show up in the inspector
[System.Serializable]
public class Map {
	public Coord mapSize;
	// define where enemies spawn
	public Coord start;
	// define where enemies arrive
	public Coord end;
	// number of obstacle to spawn in percentage
	[Range(0.5f, 1)] public double groundPercent;
	// seed for the pseudorandom generation of the obstacles
	public int seed;
	public Color nodesColor;
	public Color groundsColor;

	public int MaxSize {
		get {
            return mapSize.x * mapSize.y;
		}
	}
}
