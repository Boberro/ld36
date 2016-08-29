using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockSpawnerController : MonoBehaviour {

	float terrainSize = 250f;
	float terrainBorder = 10f;
	float safeZoneRadius = 50f;

	float numberOfBlocks = 256;

	GameObject blockPrefab;
	List<GameObject> createdBlocks = new List<GameObject>();

	void Start () {
		blockPrefab = Resources.Load<GameObject> ("Prefabs/Cube Block");

		for (int i = 0; i < numberOfBlocks; i++) {
			Vector3 pos;
			do {
				float x = Mathf.Clamp (Random.value * (terrainSize * 2) - terrainSize, -terrainSize + terrainBorder, terrainSize - terrainBorder);
				float z = Mathf.Clamp (Random.value * (terrainSize * 2) - terrainSize, -terrainSize + terrainBorder, terrainSize - terrainBorder);
				float y = Terrain.activeTerrain.SampleHeight (new Vector3 (x, 0, z));
				pos = new Vector3 (x, y, z);
			} while (Mathf.Abs((transform.position - pos).magnitude) < safeZoneRadius);

			createdBlocks.Add (
				GameObject.Instantiate (blockPrefab, pos, Random.rotation) as GameObject
			);
		}
	}
}
