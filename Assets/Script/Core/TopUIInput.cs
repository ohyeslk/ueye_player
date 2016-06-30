using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TopUIInput : MonoBehaviour {
	[SerializeField] Image resetBtn;
	[SerializeField] Image switchBtn;
	[SerializeField] Canvas canvas;

	public void OnFingerDown( FingerDownEvent e )
	{

		Vector2 pointPos = e.Position;
		Vector2 switchBtnPos = new Vector2( Screen.width , 0  ) + switchBtn.rectTransform.anchoredPosition * canvas.scaleFactor ;
		Vector2 switchRect =  new Vector2( switchBtn.rectTransform.rect.width * canvas.scaleFactor
			, switchBtn.rectTransform.rect.height * canvas.scaleFactor );

		debugStr = "Down ";
		debugStr += pointPos.ToString() +  switchBtnPos.ToString() + switchRect.ToString();

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
	string debugStr = "";
	string touch = "";

	void OnGUI()
	{
		GUILayout.TextField(debugStr);
		GUILayout.TextField(touch);
	}

	bool CheckInBox ( Vector2 point , Vector2 checkCenter , Vector2 checkBox )
	{
		return point.x < checkCenter.x + checkBox.x / 2f 
			&& point.x > checkCenter.x - checkBox.x / 2f 
			&& point.y < checkCenter.y + checkBox.y / 2f 
			&& point.y > checkCenter.y - checkBox.y / 2f;
		
	}


	void Update()
	{
		if ( Input.touches.Length > 0 )
		{
			touch += Input.GetTouch(0).position + " ";
		}
	}
}
