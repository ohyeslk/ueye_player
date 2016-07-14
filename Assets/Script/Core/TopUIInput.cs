using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TopUIInput : MonoBehaviour {
	[SerializeField] Image resetBtn;
	[SerializeField] Image switchBtn;
	[SerializeField] Canvas canvas;

	void OnEnable()
	{
		VREvents.SwitchVRMode += OnSwitchVRMode;
	}

	void OnDisable()
	{
		VREvents.SwitchVRMode -= OnSwitchVRMode;
	}

	void OnSwitchVRMode (Message msg)
	{

		VRMode to = (VRMode)msg.GetMessage(Global.MSG_SWITCHVRMODE_MODE_KEY);
		bool ifShow = to == VRMode.VR_2D;

		resetBtn.gameObject.SetActive( ifShow );


		if ( Application.platform == RuntimePlatform.Android )
		{
			// TODO: This is required when change back to the Google Cardboard Android Manifesto
//			switchBtn.gameObject.SetActive( ifShow );
		}
	}

	void Start()
	{
		Cardboard cardboard = FindObjectOfType<Cardboard>();
		cardboard.OnBackButton += OnBackButton;
	}

	void OnBackButton ()
	{
		DoSwitchButton();
	}

	public void OnFingerDown( FingerDownEvent e )
	{
		DealPosition(e.Position);
	}

	void DealPosition( Vector2 pos )
	{
		Vector2 pointPos = pos;
		Vector2 switchBtnPos = new Vector2( Screen.width , 0  ) + switchBtn.rectTransform.anchoredPosition * canvas.scaleFactor ;
		Vector2 switchRect =  new Vector2( switchBtn.rectTransform.rect.width * canvas.scaleFactor
			, switchBtn.rectTransform.rect.height * canvas.scaleFactor );

//		debugStr = "Down ";
//		debugStr += pointPos.ToString() +  switchBtnPos.ToString() + switchRect.ToString();

		if ( CheckInBox( pointPos , switchBtnPos , switchRect * 1.2f ) )
		{
			DoSwitchButton();
		}

		Vector2 resetBtnPos = resetBtn.rectTransform.anchoredPosition * canvas.scaleFactor;
		Vector2 resetRect =  new Vector2( resetBtn.rectTransform.rect.width * canvas.scaleFactor
			, resetBtn.rectTransform.rect.height * canvas.scaleFactor );

		if ( CheckInBox( pointPos , resetBtnPos , resetRect * 1.2f ) )
		{
			DoResetButton();
		}
	}

	void DoSwitchButton()
	{
		LogicManager.SwitchVRMode();
	}

	void DoResetButton()
	{
		CardboardHead.ResetCenter();
	}


	bool CheckInBox ( Vector2 point , Vector2 checkCenter , Vector2 checkBox )
	{
		return point.x < checkCenter.x + checkBox.x / 2f 
			&& point.x > checkCenter.x - checkBox.x / 2f 
			&& point.y < checkCenter.y + checkBox.y / 2f 
			&& point.y > checkCenter.y - checkBox.y / 2f;	
	}

//	string debugStr = "";
//	string touch = "";
//
//	void OnGUI()
//	{
//		GUILayout.TextField(debugStr);
//		GUILayout.TextField(touch);
//	}
//
//	void Update()
//	{
//		if ( Input.touches.Length > 0 )
//		{
//			touch = Input.GetTouch(0).position + " ";
//		}
//	}


}
