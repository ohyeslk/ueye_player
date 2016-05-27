using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class TopButton : VRFunctionButton {
	[SerializeField] Text title;
	[SerializeField] Color buttonNormalColor;
	[SerializeField] Color buttonHighLightColor;
	[SerializeField] Color textNormalColor;
	[SerializeField] Color textHighLightColor;

	[SerializeField] bool isHighedOnAwake;
	void Awake()
	{
		subButtonAnimation.subButton.color = buttonNormalColor;
		SetHighlighted( isHighedOnAwake );
	}

	void OnEnable()
	{
		
	}

	void OnDiable()
	{
	}

	public override void OnConfirm ()
	{
		base.OnConfirm ();
	}

	public void SetName( string name )
	{
		title.text = name;
	}

	public void SetHighlighted( bool to )
	{
		if ( to )
		{
			subButtonAnimation.subButton.color = buttonHighLightColor;
			title.color = textHighLightColor;
		}
		else
		{
			subButtonAnimation.subButton.color = buttonNormalColor;
			title.color = textNormalColor;
		}
	}
}
