using UnityEngine;
using System.Collections;
using DG.Tweening;

public class VoiceButton : VRBasicButton {

	public override void OnConfirm ()
	{
		base.OnConfirm ();

		Message msg = new Message(this);
		msg.AddMessage("isOn" , true);
		VREvents.FireVoiceRecord(msg);

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
	
	}
}
