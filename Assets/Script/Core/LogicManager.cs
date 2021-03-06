﻿using UnityEngine;
using System.Collections;

public class LogicManager : MonoBehaviour {

	static VRMode m_vr_mode;
	static public VRMode VRMode
	{
		get {
			return m_vr_mode ;
		}
	}

	static public WindowArg.Type m_window_type;
	static public WindowArg.Type ActiveWindow
	{
		get {
			return m_window_type;
		}
	}

	static public bool isLockVerticle
	{
		get {
			return ( VRMode == VRMode.VR_2D ) && ( ActiveWindow != WindowArg.Type.PLAY_WINDOW );
		}
	}
		
	private static LogicManager instance;
	public static LogicManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType(typeof(LogicManager)) as LogicManager;
				if (instance == null)
				{
					instance = new GameObject("LogicManager").AddComponent<LogicManager>();
				}
			}
			return instance;
		}
	}

	static Cardboard m_cardboard;
	public static Cardboard CardBoard
	{
		get {
			if ( m_cardboard == null )
			{
				m_cardboard = FindObjectOfType( typeof(Cardboard)) as Cardboard;
			}
			return m_cardboard;
		}
	}


	static public void SwitchVRMode()
	{
		Debug.Log( "Switch VRMode " );
		if ( m_vr_mode == VRMode.VR_3D )
		{
			SetVRMode( VRMode.VR_2D);
		}else
		{
			SetVRMode( VRMode.VR_3D);
		}
	}

	static public void SetVRMode(VRMode to)
	{
		Debug.Log("Set VRMode to " + to + " Cardboard " + CardBoard );
		m_vr_mode = to;
		if ( to == VRMode.VR_2D )
		{
			if ( CardBoard != null ) CardBoard.VRModeEnabled = false;
		}else // to 3d mode
		{
			if ( CardBoard != null ) CardBoard.VRModeEnabled = true;
		}

		Message msg = new Message( Instance );
		msg.AddMessage(Global.MSG_SWITCHVRMODE_MODE_KEY, to );
		VREvents.FireSwitchVRMode( msg );
	}

	void Start()
	{
		SetVRMode(Global.initMode);
	}

	void OnEnable()
	{
		VREvents.ActiveWindow +=  OnActiveWindow;
	}

	void OnDisable()
	{
		VREvents.ActiveWindow -=  OnActiveWindow;
	}

	void OnActiveWindow (WindowArg arg)
	{
		m_window_type = arg.type;
	}

}


public enum VRMode
{
	VR_None = 0,
	VR_2D = 1,
	VR_3D = 2,
	VR_All = 3,
}
