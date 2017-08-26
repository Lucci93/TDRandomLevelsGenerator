using UnityEngine;

[System.Serializable]
public struct Coord {
	public int x;
	public int y;
	[HideInInspector]
	public bool walkable;

	public Coord(int _x, int _y, bool _walkable) {
		x = _x;
		y = _y;
		walkable = _walkable;
	}

	// override all the function to properly work with coordinates structure
	public override bool Equals(System.Object obj) {
		return obj is Coord && this == (Coord)obj;
	}

	public bool Equals(Coord c) {
		return this == c;
	}

	public static bool operator ==(Coord c1, Coord c2) {
		return c1.x == c2.x && c1.y == c2.y;
	}

	public static bool operator !=(Coord c1, Coord c2) {
		return !(c1 == c2);
	}

	public override int GetHashCode() {
		return x ^ y;
	}
}
