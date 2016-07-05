using UnityEngine;
using System.Collections;
using DG.Tweening;

public class VoiceButton : VRBasicButton {
	[SerializeField] UnityEngine.UI.Image img;
	[SerializeField] Sprite normalSprite;
	[SerializeField] Sprite ListenSprite;
	[SerializeField] Sprite ScanSprite;

	public void Reset()
	{
		img.sprite = normalSprite;
	}

	public override void OnConfirm ()
	{
		base.OnConfirm ();

		Message msg = new Message(this);
		msg.AddMessage("isOn" , true);
		VREvents.FireVoiceRecord(msg);

		img.sprite = ListenSprite;
	}
		
	public override void OnExitHover ()
	{
		base.OnExitHover ();

		OnEndRecord();
	}

	public void OnEndRecord()
	{
		Message msg = new Message(this);
		msg.AddMessage("isOn" , false);
		VREvents.FireVoiceRecord(msg);
	
		img.sprite = ScanSprite;

		VREvents.FireUIInputResetTarget(new Message(this));
	}
}
