using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class UfoController : MonoBehaviour {

	#region define movement related
	Vector3 movementDir;
	Vector3 targetMovementAmount;
	Vector3 movementAmount;
	Vector3 smoothMovementVelocity;

	Vector3 targetDeviation;
	Vector3 rotationDeviation;
	Vector3 smoothDeviationVelocity;

	float targetRoll;
	float rollAmount;
	float smoothRollVelocity;

	Vector3 elevationDir;
	Vector3 targetElevationAmount;
	Vector3 elevationAmount;
	Vector3 smoothElevationVelocity;

	float movementSpeed = 16f;
	float deviationMultiplier = .2f;
	float rollSpeed = 2f;
	float elevationSpeed = 2f;

	#region mouse view
	float mouseSensitivityX = 1f;
	float mouseSensitivityY = 1f;
	float mouseScrollSensitivity = 50f;

	float verticalLookRotationMin = 15f;
	float verticalLookRotationMax = 80f;

	float horizontalLookRotation;
	float verticalLookRotation = 15f;
	float smoothHorizontalLookRotationVelocity;
	float smoothVerticalLookRotationVelocity;

	float verticalTargettingCameraRotationMin = 65f;
	float verticalTargettingCameraRotationMax = 90f;

	float verticalTargettingCameraRotation = 65f;

	Vector3 maxCameraPosition = new Vector3 (0, 3, 0);
	Vector3 initialCameraPosition;
	Vector3 initialTargettingCameraPosition;

	Vector3 maxDisplayScale = new Vector3 (1.25f, 1.25f, 1.25f);
	Vector3 initialDisplayScale;

	Vector3 maxDisplayPosition = new Vector3 (0, 0.75f, 0.01f); //can't be on center, not to confuse the "look at" function
	Vector3 initialDisplayPosition;

	Transform cameraTransform;
	Transform targettingCameraTransform;
	Transform displayTransform;
	#endregion
	#endregion

	Rigidbody rigidBody;
	Transform modelTransform;
	BeamController beamController;
	Renderer cupolaRenderer;

	AudioSource engineSoundSource;
	AudioSource beamSoundSource;
	AudioSource beamGrabbingSoundSource;

	float initialEngineVolume;
	float maxEngineVolume = 1f;

	Canvas canvas;
	bool paused = false;

	void Start() {
		Time.timeScale = 1;
		paused = false;

		modelTransform = transform.FindChild ("ufo");
		beamController = GetComponentInChildren<BeamController> ();
		cupolaRenderer = transform.FindChild ("ufo/ufo_cupola").GetComponent<Renderer> ();

		cameraTransform = transform.FindChild ("Main Camera");
		targettingCameraTransform = transform.FindChild ("Target Camera");
		displayTransform = transform.FindChild ("Display");

		initialCameraPosition = cameraTransform.localPosition;
		initialTargettingCameraPosition = targettingCameraTransform.localPosition;
		initialDisplayPosition = displayTransform.localPosition;

		initialDisplayScale = displayTransform.localScale;

		engineSoundSource = transform.FindChild ("Engine Sound").GetComponent<AudioSource> ();
		initialEngineVolume = engineSoundSource.volume;

		beamSoundSource = transform.FindChild ("Beam Sound").GetComponent<AudioSource> ();
		beamGrabbingSoundSource = transform.FindChild ("Beam Sound (Grabbing)").GetComponent<AudioSource> ();

		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		canvas.enabled = false;
	}

	void Awake() {
		if (!paused) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		} else {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		// set up rigid body
		rigidBody = GetComponent<Rigidbody> ();
		rigidBody.useGravity = false;
		rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
	}
	
	void Update () {
		#region menu
		if (Input.GetKeyUp(KeyCode.Escape)) {
			Time.timeScale = paused ? 1 : 0;
			canvas.enabled = !paused;
			Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
			Cursor.visible = !paused;
			paused = !paused;
		}
		#endregion

		if (!paused) {

			#region camera
			horizontalLookRotation += Input.GetAxis ("Mouse X") * mouseSensitivityX;
			horizontalLookRotation = Mathf.Clamp (horizontalLookRotation, -90, 90);
			if (Mathf.Abs (horizontalLookRotation) <= 5f) {
				horizontalLookRotation = Mathf.SmoothDamp (horizontalLookRotation, 0, ref smoothHorizontalLookRotationVelocity, .15f);
			}
			verticalLookRotation += Input.GetAxis ("Mouse Y") * -mouseSensitivityY;
			verticalLookRotation = Mathf.Clamp (verticalLookRotation, verticalLookRotationMin, verticalLookRotationMax);
			if (verticalLookRotation <= 20f) {
				verticalLookRotation = Mathf.SmoothDamp (verticalLookRotation, 15, ref smoothVerticalLookRotationVelocity, .15f);
			}

			cameraTransform.localEulerAngles = new Vector3 (verticalLookRotation, horizontalLookRotation, 0);
			targettingCameraTransform.localEulerAngles = new Vector3 (verticalTargettingCameraRotation, 0, 0);

			float verticalLookDeviation = (verticalLookRotation - verticalLookRotationMin) / (verticalLookRotationMax - verticalLookRotationMin);
			Vector3 cameraPositionDeviation = (maxCameraPosition - initialCameraPosition) * verticalLookDeviation;
			cameraTransform.localPosition = initialCameraPosition + cameraPositionDeviation;

			#region targetting camera
			verticalTargettingCameraRotation = verticalTargettingCameraRotationMin + ((verticalLookRotation - verticalLookRotationMin) / (verticalLookRotationMax - verticalLookRotationMin)) * (verticalTargettingCameraRotationMax - verticalTargettingCameraRotationMin);
			targettingCameraTransform.localPosition = initialTargettingCameraPosition + cameraPositionDeviation;
			#endregion

			#region display
			Vector3 displayPositionDeviation = (maxDisplayPosition - initialDisplayPosition) * verticalLookDeviation;
			displayTransform.localPosition = initialDisplayPosition + displayPositionDeviation;

			Vector3 displayNewScale = new Vector3 (Mathf.Lerp (initialDisplayScale.x, maxDisplayScale.x, verticalLookDeviation), Mathf.Lerp (initialDisplayScale.y, maxDisplayScale.y, verticalLookDeviation), Mathf.Lerp (initialDisplayScale.z, maxDisplayScale.z, verticalLookDeviation));
			displayTransform.localScale = displayNewScale;

			displayTransform.LookAt (cameraTransform);
			#endregion
			#endregion

			#region movement
			targetRoll = Input.GetAxisRaw ("Roll") * rollSpeed;
			rollAmount = Mathf.SmoothDamp (rollAmount, targetRoll, ref smoothRollVelocity, .15f);

			movementDir = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical")).normalized;
			targetMovementAmount = movementDir * movementSpeed;
			movementAmount = Vector3.SmoothDamp (movementAmount, targetMovementAmount, ref smoothMovementVelocity, .2f);

			// Deviate the model rotation, to get the feel of movement
			Vector3 targetRotationDeviation = (transform.up + movementDir * deviationMultiplier).normalized;
			rotationDeviation = Vector3.SmoothDamp (rotationDeviation, targetRotationDeviation, ref smoothDeviationVelocity, .15f);

			elevationDir = new Vector3 (0, Input.GetAxisRaw ("Elevation"), 0).normalized;
			targetElevationAmount = elevationDir * elevationSpeed;
			elevationAmount = Vector3.SmoothDamp (elevationAmount, targetElevationAmount, ref smoothElevationVelocity, .15f);
			#endregion

			#region beam control
			#region beam size control
			float beamResize = Input.GetAxisRaw ("Mouse ScrollWheel") * mouseScrollSensitivity * Time.deltaTime;
			beamController.ChangeBeamSize (beamResize);
			#endregion

			if (Input.GetButtonDown ("Jump")) {
				if (beamController.BeamStatus == BeamStatus.idle) {
					beamController.BeamStatus = BeamStatus.grabbing;
				} else {
					beamController.BeamStatus = BeamStatus.idle;
				}
			}
			if (Input.GetButtonUp ("Jump")) {
				if (beamController.BeamStatus == BeamStatus.grabbing) {
					beamController.BeamStatus = BeamStatus.grabbed;
					beamController.GrabHighlightedNow = true;
				} else {
					beamController.BeamStatus = BeamStatus.idle;
				}
			}

			#region cupola color
			float beamScale = (beamController.beamScale - beamController.beamScaleMin) / (beamController.beamScaleMax - beamController.beamScaleMin);
			Color cupolaColor = cupolaRenderer.material.GetColor ("_TintColor");
			cupolaRenderer.material.SetColor ("_TintColor", new Color (cupolaColor.r, cupolaColor.g, cupolaColor.b, .1f + beamScale * .8f)); // 10%-90%
			#endregion
			#endregion
	
			#region sounds
			float enginePower = movementAmount.magnitude / movementSpeed;
			float engineVolume = Mathf.Lerp (initialEngineVolume, maxEngineVolume, enginePower);
			engineSoundSource.volume = engineVolume;

			if (beamController.BeamStatus == BeamStatus.grabbing) {
				if (!beamGrabbingSoundSource.isPlaying) {
					beamGrabbingSoundSource.Play ();
					beamSoundSource.Play ();
				}
			} else {
				if (beamGrabbingSoundSource.isPlaying) {
					beamGrabbingSoundSource.Stop ();
				}
				if ((beamController.BeamStatus == BeamStatus.idle) && (beamSoundSource.isPlaying)) {
					beamSoundSource.Stop ();
				}
			}
			#endregion

		}
	}

	void FixedUpdate ()
	{
		Vector3 localMove = transform.TransformDirection (movementAmount) * Time.fixedDeltaTime;
		Vector3 localElevation = transform.TransformDirection (elevationAmount) * Time.deltaTime;
		rigidBody.MovePosition (transform.position + localMove + localElevation);

		Vector3 localRotationDeviation = transform.TransformDirection (rotationDeviation);
		modelTransform.rotation = Quaternion.FromToRotation (modelTransform.up, localRotationDeviation) * modelTransform.rotation;

		transform.Rotate (transform.up, rollAmount);
	}
}
