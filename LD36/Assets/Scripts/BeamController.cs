using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BeamStatus {
	idle,
	grabbing,
	grabbed
}

public class BeamController : MonoBehaviour {

	public float beamScale = 4f;
	public float beamScaleMin = 1f;
	public float beamScaleMax = 12f;

	Vector3 normalScale;
	Vector3 normalScaleStep;

	public BeamStatus BeamStatus { get; set; }
	public bool GrabHighlightedNow { get; set; }
	public bool DropAllNow { get; set; }

	public List<BuildingBlockController> highlighted = new List<BuildingBlockController> ();
	public List<BuildingBlockController> grabbed = new List<BuildingBlockController> ();

	UfoController ufoController;

	void Start () {
		ufoController = GetComponentInParent<UfoController> ();

		GetComponent<MeshRenderer> ().enabled = false;

		this.BeamStatus = BeamStatus.idle;
		GrabHighlightedNow = false;

		normalScale = transform.localScale;
		normalScaleStep = new Vector3 (normalScale.x / 4, normalScale.y / 4, normalScale.z / 4);
	}

	void Update () {
		if (GrabHighlightedNow) {
			foreach (BuildingBlockController buildingBlockController in highlighted) {
				grabbed.Add (buildingBlockController);
				buildingBlockController.Grabbed = true;
			}
			GrabHighlightedNow = false;
		}

		if ((BeamStatus != BeamStatus.grabbed) && (grabbed.Count > 0)) {
			foreach (BuildingBlockController buildingBlockController in grabbed) {
				buildingBlockController.Grabbed = false;
			}
			grabbed.Clear ();
		}
	}

	void FixedUpdate () {
		if (beamScale == 4f) {
			transform.localScale = normalScale;
		} else {
			transform.localScale = new Vector3 (normalScaleStep.x * beamScale, normalScaleStep.y * beamScale, normalScale.z);
		}
	}

	public void ChangeBeamSize(float value) {
		beamScale = Mathf.Clamp (beamScale + value, beamScaleMin, beamScaleMax);
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag.Equals ("Building Block")) {
			BuildingBlockController buildingBlockController = other.GetComponent<BuildingBlockController> ();
			buildingBlockController.Highlighted = true;
			highlighted.Add (buildingBlockController);
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag.Equals ("Building Block")) {
			BuildingBlockController buildingBlockController = other.GetComponent<BuildingBlockController> ();

			buildingBlockController.Highlighted = false;
			highlighted.Remove (buildingBlockController);
			buildingBlockController.Grabbed = false;
			grabbed.Remove (buildingBlockController);
		}
	}

	void OnTriggerStay (Collider other) {
//		if (other.gameObject.tag.Equals ("Building Block")) {
//			BuildingBlockController buildingBlockController = other.GetComponent<BuildingBlockController> ();
//			if (highlighted.IndexOf (buildingBlockController) > -1) {
//				highlighted.Add (buildingBlockController);
//				buildingBlockController.Highlighted = true;
//			}
//		}
	}

}
