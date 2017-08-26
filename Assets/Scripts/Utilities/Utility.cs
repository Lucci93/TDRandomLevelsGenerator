using System.Collections.Generic;
using UnityEngine;

// contains all the utility functions
public static class Utility {

    // our shuffle array is of undefined type so we use a generic T type
    // shuffle array is a random array maded wit a particular iteration way
    public static List<T> ShuffleList<T>(List<T> list, int seed) {

        // we use the seed to control the random generation
        System.Random prng = new System.Random(seed);

        // we can discard the last iteration because [n-1, n] don't have element to swap
        for (int i = 0; i < list.Count - 1; i++) {
            // we get a random index between range [i,n]
            int randomIndex = prng.Next(i, list.Count);
            // spaw the i element of the array with the randomindex element of the array
            T tempItem = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = tempItem;
        }
        // return the custom array
        return list;
    }

	// just for debug print a node grid on terminal
	public static void PrintNodeGrid(List<Node> grid, Map map) {
		Debug.Log("---- NODES ----");
		string text = "";
		for (int x = 0; x < map.mapSize.x; x++) {
			for (int y = 0; y < map.mapSize.y; y++) {
                if (grid.Find(e => e.x == x && e.y == y).parent == null) {
                    if (grid.Find(e => e.x == x && e.y == y).notWalkable) {
                        // node add to grid
                        text += "0" + " ";
                    }
                    else {
                        text += "1" + " ";
                    }
                }
                else {
                    text += "2" + " ";
                }
			}
			text += "\n";
		}
		Debug.Log(text);
	}
}
