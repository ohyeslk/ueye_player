﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ModeSwitchButton : MonoBehaviour {

	[SerializeField] Sprite VRIcon;
	[SerializeField] Sprite EyeIcon;
	[SerializeField] Image image;

	void Start()
	{
		if ( image == null )
			image = GetComponent<Image>();
		UpdateIcon();
	}


	void UpdateIcon()
	{
		if ( image != null )
		{
			image.sprite = LogicManager.VRMode == VRMode.VR_2D ? VRIcon : EyeIcon;
		}
	}

	public void OnSwitch()
	{
		LogicManager.SwitchVRMode();
		UpdateIcon();
	}

}
