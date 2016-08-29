using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PyramidStarterController : MonoBehaviour {

	int bottomSize = 8;
	int levels = 3;

	GameObject blockPrefab;
	public List<GameObject> createdBlocks = new List<GameObject>();

	void Start () {
		blockPrefab = Resources.Load<GameObject> ("Prefabs/Cube Block");

		#region spawn pyramid starter
		Vector3 steps = blockPrefab.transform.lossyScale;

		for (int y=0; y<levels; y++) {
			int size = bottomSize - y;
			if (size <= 0) break;

			for (int x=0; x<size; x++) {
				float offsetX = (steps.x * size) / 2;
				for (int z=0; z<size; z++) {
					float offsetZ = (steps.z * size) / 2;

					Vector3 pos = new Vector3 (steps.x * x - offsetX, steps.y * y, steps.z * z - offsetZ);
					createdBlocks.Add(
						GameObject.Instantiate(blockPrefab, transform.position + pos, transform.rotation) as GameObject
					);
				}
			}
		}
		#endregion
	}
}
