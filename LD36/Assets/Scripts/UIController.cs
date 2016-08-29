using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

	void Start () {
		if (Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXEditor) {
			GameObject.Find ("Canvas/Quit Button").SetActive(false);
		}
	}

	public void OnRestartClicked () {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	public void OnQuitClicked () {
		Application.Quit ();
	}

}
