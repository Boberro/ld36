using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (AudioSource))]
public class BuildingBlockController : MonoBehaviour {

	Color highlightColor = new Color (203, 0, 225, 255);
	Color grabColor = new Color (0, 255, 0, 255);

	Renderer outlineRenderer;

	Transform ufoTransform;
	Vector3 ufoPreviousPos;
	Quaternion ufoPreviousRotation;

	Rigidbody rigidBody;
	AudioSource audioSource;

	AudioClip rockOnGroundClip;
	AudioClip rockOnRockClip;

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

		audioSource = GetComponent<AudioSource> ();

		rockOnGroundClip = Resources.Load<AudioClip> ("Sfx/rock_on_ground");
		rockOnRockClip = Resources.Load<AudioClip> ("Sfx/rock_on_rock");

		ufoTransform = GameObject.Find ("Ufo").transform;
		ufoPreviousPos = ufoTransform.position;
		ufoPreviousRotation = ufoTransform.rotation;

		Highlighted = false;
		Grabbed = false;
	}

	void FixedUpdate () {
		if (Grabbed) {
			Quaternion ufoRotated = Quaternion.Inverse(ufoPreviousRotation) * ufoTransform.rotation;
			rigidBody.transform.RotateAround(ufoTransform.position, ufoTransform.up, ufoRotated.eulerAngles.y);

			Vector3 ufoMoved = ufoTransform.position - ufoPreviousPos;
			rigidBody.MovePosition (transform.position + ufoMoved);
		}
		ufoPreviousPos = ufoTransform.position;
		ufoPreviousRotation = ufoTransform.rotation;
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Terrain") {
			audioSource.PlayOneShot (rockOnGroundClip);
			if (Mathf.Abs (rigidBody.velocity.magnitude) > 0.01f) {
				audioSource.Play ();
			}
		} else if (collision.gameObject.tag == "Building Block") {
			audioSource.PlayOneShot (rockOnRockClip);
			if (Mathf.Abs (rigidBody.velocity.magnitude) > 0.01f) {
				audioSource.Play ();
			}
		}
	}
}