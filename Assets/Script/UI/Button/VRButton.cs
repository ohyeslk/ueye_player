﻿using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class VRButton : UISensor {

	[SerializeField] UnityEvent onConfirm;
	[SerializeField] UnityEvent onEnterHover;
	[SerializeField] UnityEvent onExitHover;
	[SerializeField] VRFloatEvent onUpdateHover;
	[SerializeField] UnityEvent onFocus;
	[SerializeField] VRMode mode;

	void OnDisable()
	{
		VREvents.SwitchVRMode -= OnSwitchVRMode;
	}

	void OnEnable()
	{
		VREvents.SwitchVRMode += OnSwitchVRMode;
	}

	void OnSwitchVRMode( Message msg )
	{
		Reset();
		VRMode to = (VRMode) msg.GetMessage( Global.MSG_SWITCHVRMODE_MODE_KEY );
		Image img = GetComponent<Image>();
		if ( ( to & mode ) > 0 )
		{
			if ( img != null ) img.raycastTarget = true;
		}else
		{
			if ( img != null ) img.raycastTarget = false;
		}
	}

	public override void OnConfirm ()
	{
		base.OnConfirm();
		if ( ( LogicManager.VRMode & mode ) > 0  )
		{
			if ( onConfirm != null ) onConfirm.Invoke();
		}
	}

	public override void OnHover (UIHoverEvent e)
	{
		if ( ( LogicManager.VRMode & mode ) > 0 )
		{
			base.OnHover (e);
			if ( e.hoverPhase == UIHoverEvent.HoverPhase.Begin )
			{
				if ( onEnterHover != null ) onEnterHover.Invoke();
			}
			else if ( e.hoverPhase == UIHoverEvent.HoverPhase.Middle )
			{
				if ( onUpdateHover != null ) 
				{
					float process = FocusTime / GetTotalConfirmTime();
					onUpdateHover.Invoke(process);
				}
			}else if ( e.hoverPhase == UIHoverEvent.HoverPhase.End )
			{
				if ( onExitHover != null ) onExitHover.Invoke();
			}
		}
	}
	public override void OnFocus ()
	{
//		Debug.Log(" LM " + LogicManager.VRMode + " mode " + mode + " Res " + ( LogicManager.VRMode & mode ) );
		if ( ( LogicManager.VRMode & mode ) > 0 )
		{
			base.OnFocus ();
			if ( onFocus != null ) onFocus.Invoke();
		}

	}

}


[System.Serializable]
public class VRFloatEvent : UnityEvent<float>{}