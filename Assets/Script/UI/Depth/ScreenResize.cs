using UnityEngine;
using System.Collections;

public class ScreenResize : MonoBehaviour {

	const float ScreenWidthUnit = 20f;

	void Awake()
	{
		ResizeScreen();
	}

	public void ResizeScreen()
	{
		transform.localScale = new Vector3( ScreenWidthUnit , ScreenWidthUnit / Screen.height  * Screen.width * 0.25f );
	}
}
