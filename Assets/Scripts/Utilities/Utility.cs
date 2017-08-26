using UnityEngine;

// contains all the utility functions
public static class Utility {

    // our shuffle array is of undefined type so we use a generic T type
    // shuffle array is a random array maded wit a particular iteration way
    public static T[] ShuffleArray<T>(T[] array, int seed) {

        // we use the seed to control the random generation
        System.Random prng = new System.Random(seed);

        // we can discard the last iteration because [n-1, n] don't have element to swap
        for (int i = 0; i < array.Length - 1; i++) {
            // we get a random index between range [i,n]
            int randomIndex = prng.Next(i, array.Length);
            // spaw the i element of the array with the randomindex element of the array
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }
        // return the custom array
        return array;
    }

    // return the node searched in the grid
    public static Node FindNode(Node[] array, int a, int b) {
        for (int i = 0; i < array.Length; i++) {
            if (array[i].x == a && array[i].y == b) {
                return array[i];
            }
        }
        return null;
    }

	public static int FindNodeIndex(Node[] array, int a, int b) {
		int count = 0;
		for (int i = 0; i < array.Length; i++) {
			if (array[i].x == a && array[i].y == b) {
				return count;
			}
			count++;
		}
		return count;
	}

	// just for debug print a node grid on terminal
	public static void PrintNodeGrid(Node[] grid, Map map) {
		Debug.Log("---- NODES ----");
		string text = "";
		for (int x = 0; x < map.mapSize.x; x++) {
			for (int y = 0; y < map.mapSize.y; y++) {
                if (grid[FindNodeIndex(grid, x, y)].parent == null) {
                    if (grid[FindNodeIndex(grid, x, y)].notWalkable) {
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
