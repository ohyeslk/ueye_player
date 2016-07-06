using UnityEngine;
using System.Collections;

public class ScreenResize : MonoBehaviour {

	float ScreenWidthUnit ;

	void Awake()
	{
		ScreenWidthUnit = transform.localScale.x;
		ResizeScreen();
	}

	public void ResizeScreen()
	{
		transform.localScale = new Vector3( ScreenWidthUnit , ScreenWidthUnit / Screen.height  * Screen.width * 0.125f );
	}
}
