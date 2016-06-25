using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class HoverSensor : UISensor {

	[SerializeField] UnityEvent onConfirm;
	[SerializeField] UnityEvent onEnterHover;
	[SerializeField] UnityEvent onExitHover;
	[SerializeField] VRFloatEvent onUpdateHover;
	[SerializeField] VRVec3Event onUpdateHoverV3;
	[SerializeField] UnityEvent onFocus;
	[SerializeField] UnityEvent onFingerUp;
	[SerializeField] VRMode mode;
	[SerializeField] bool ifUseUpdateEvent = false;

	void OnDisable()
	{
		VREvents.SwitchVRMode -= OnSwitchVRMode;
	}

	void OnEnable()
	{
		VREvents.SwitchVRMode += OnSwitchVRMode;
		SetToMode( LogicManager.VRMode );
	}

	void OnSwitchVRMode( Message msg )
	{
		Reset();
		VRMode to = (VRMode) msg.GetMessage( Global.MSG_SWITCHVRMODE_MODE_KEY );
		SetToMode( to );
	}

	void SetToMode( VRMode to )
	{

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

				if ( ifUseUpdateEvent )
				{
					//TODO add c# event here 

				}
				else
				{
					if ( onUpdateHover != null ) 
					{
						float process = FocusTime / GetTotalConfirmTime();
						onUpdateHover.Invoke(process);
					}

					if ( onUpdateHoverV3 != null )
					{
						onUpdateHoverV3.Invoke(e.point);
					}
				}
			}else if ( e.hoverPhase == UIHoverEvent.HoverPhase.End )
			{
//				Debug.Log("OnHover End " + name + " " + transform.parent.name );
				if ( onExitHover != null ) onExitHover.Invoke();
				if ( onUpdateHoverV3 != null ) onUpdateHoverV3.Invoke( Global.ONHOVERV3_PHASE_EXIT );
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


	public override void OnFingerUp()
	{
		if ( ( LogicManager.VRMode & mode ) > 0 )
		{
			base.OnFingerUp ();
			if ( onFingerUp != null ) onFingerUp.Invoke();
		}		
	}

}


[System.Serializable]
public class VRFloatEvent : UnityEvent<float>{}

[System.Serializable]
public class VRVec3Event : UnityEvent<Vector3>{}