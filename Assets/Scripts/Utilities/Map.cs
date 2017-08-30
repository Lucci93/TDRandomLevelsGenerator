using UnityEngine;

// show up in the inspector
[CreateAssetMenu(fileName = "New Map")]
public class Map : ScriptableObject {
    // name of the level
    new public string name = "New Level";

	public Node mapSize;
	// define where enemies spawn
	public Node start;
	// define where enemies arrive
	public Node end;
	// number of obstacle to spawn in percentage
	[Range(0.5f, 1)] public double groundPercent;
	// seed for the pseudorandom generation of the obstacles
	public int seed;
    // color for the element of the map
	public Color freeCellColor;
	public Color pathCellColor;
    public Color startObjectColor;
    public Color endObjectColor;
}
