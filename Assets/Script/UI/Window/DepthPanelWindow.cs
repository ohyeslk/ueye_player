using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DepthPanelWindow : MonoBehaviour {

	public void OnBack()
	{
		SceneManager.LoadScene("VideoSelect");
	}
}
