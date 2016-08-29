using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class BuildingBlockController : MonoBehaviour {

	Color highlightColor = new Color (203, 0, 225, 255);
	Color grabColor = new Color (0, 255, 0, 255);

	Renderer outlineRenderer;

	Transform ufoTransform;
	Vector3 ufoPreviousPos;

	Rigidbody rigidBody;

	private bool highlighted;
	public bool Highlighted {
		get {
			return this.highlighted;
		}
		set {
			if (this.highlighted != value) {
				this.highlighted = value;
				outlineRenderer.enabled = value;
			}
		}
	}
	private bool grabbed;
	public bool Grabbed {
		get {
			return this.grabbed;
		}
		set {
			if (this.grabbed != value) {
				this.grabbed = value;
				if (value) {
					outlineRenderer.material.color = grabColor;
					rigidBody.useGravity = false;
					rigidBody.angularDrag = 1f;
				} else {
					outlineRenderer.material.color = highlightColor;
					rigidBody.useGravity = true;
					rigidBody.angularDrag = 0.05f;
				}
			}
		}
	}

	void Start () {
		outlineRenderer = transform.FindChild ("Outline").GetComponent<Renderer> ();
		rigidBody = GetComponent<Rigidbody> ();

		ufoTransform = GameObject.Find ("Ufo").transform;
		ufoPreviousPos = ufoTransform.position;

		Highlighted = false;
		Grabbed = false;
	}

	void FixedUpdate () {
		if (Grabbed) {
			Vector3 ufoMoved = ufoTransform.position - ufoPreviousPos;
			rigidBody.MovePosition (transform.position + ufoMoved);
		}
		ufoPreviousPos = ufoTransform.position;
	}
}