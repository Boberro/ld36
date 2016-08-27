using UnityEngine;
using System.Collections;

public class CameraChaseController : MonoBehaviour {


	[SerializeField]
	Transform followed;

	Camera cam;

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		cam.transform.LookAt (followed);
	}
}
