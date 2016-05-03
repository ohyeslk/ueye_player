using UnityEngine;
using System.Collections;

public class VideoPlayWindow : UIWindow {

	[SerializeField] GameObject panel;
	void OnDisable()
	{
		VREvents.ActiveWindow -= VREvents_ActiveWindow;
	}

	void OnEnable()
	{
		VREvents.ActiveWindow += VREvents_ActiveWindow;
	}

	void VREvents_ActiveWindow (Message msg)
	{
		if ( msg.GetMessage("window").ToString().Equals( "play") )
		{
			panel.SetActive(true);
		}else{
			panel.SetActive(false);
		}
	}

}
