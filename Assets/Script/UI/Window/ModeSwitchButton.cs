using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ModeSwitchButton : MonoBehaviour {

	[SerializeField] Cardboard cardboard;
	[SerializeField] Sprite VRIcon;
	[SerializeField] Sprite EyeIcon;
	[SerializeField] Image image;

	void Start()
	{
		if ( cardboard == null )
		{
			if ( GameObject.FindGameObjectWithTag("MainCardboard"))
			{
				cardboard = GameObject.FindGameObjectWithTag("MainCardboard").GetComponent<Cardboard>();
			}
				
		}
		if ( image == null )
			image = GetComponent<Image>();
		UpdateIcon();
	}


	void UpdateIcon()
	{
		if ( cardboard != null && image != null)
			image.sprite =  !cardboard.VRModeEnabled ? VRIcon : EyeIcon;
	}

	public void OnSwitch()
	{
		if ( cardboard != null )
			cardboard.VRModeEnabled = !cardboard.VRModeEnabled;
		UpdateIcon();
	}

}
