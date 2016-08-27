using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class UfoController : MonoBehaviour {

	Vector3 movementDir;
	Vector3 targetMovementAmount;
	Vector3 movementAmount;
	Vector3 smoothMovementVelocity;

	Vector3 targetDeviation;
	Vector3 deviation;
	Vector3 smoothDeviationVelocity;

	float targetRoll;
	float rollAmount;
	float smoothRollVelocity;

//	Transform cameraTransform;
	float verticalLookRotation;

	float movementSpeed = 4f;
	float deviationMultiplier = .5f;
	float rollSpeed = 1f;
//	float mouseSensitivityX = 1f;
//	float mouseSensitivityY = 1f;

	Rigidbody rigidBody;
	Transform modelTransform;

	void Start() {
		modelTransform = transform.FindChild ("ufo");
	}

	void Awake() {
//		Cursor.lockState = CursorLockMode.Locked;
//		Cursor.visible = false;
//		cameraTransform = Camera.main.transform;

		// set up rigid body
		rigidBody = GetComponent<Rigidbody> ();
		rigidBody.useGravity = false;
		rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
	}
	
	// Update is called once per frame
	void Update () {
		#region movement
		targetRoll = Input.GetAxisRaw ("Roll") * rollSpeed;
		rollAmount = Mathf.SmoothDamp(rollAmount, targetRoll, ref smoothRollVelocity, .15f);

		movementDir = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical")).normalized;
		targetMovementAmount = movementDir * movementSpeed;
		movementAmount = Vector3.SmoothDamp (movementAmount, targetMovementAmount, ref smoothMovementVelocity, .15f);

		Vector3 targetDeviation = (transform.up + movementDir * deviationMultiplier).normalized;
		deviation = Vector3.SmoothDamp(deviation, targetDeviation, ref smoothDeviationVelocity, .15f);
		#endregion
	}

	void FixedUpdate ()
	{
		Vector3 localMove = transform.TransformDirection (movementAmount) * Time.fixedDeltaTime;
		rigidBody.MovePosition (transform.position + localMove);

		Vector3 localDeviation = transform.TransformDirection (deviation);
		modelTransform.rotation = Quaternion.FromToRotation (modelTransform.up, localDeviation) * modelTransform.rotation;
		transform.Rotate (transform.up, rollAmount);
	}
}
