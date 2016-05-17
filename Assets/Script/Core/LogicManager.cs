using UnityEngine;
using System.Collections;

public class LogicManager : MonoBehaviour {

	static VRMode m_vr_mode;
	static public VRMode VRMode
	{
		get {
			return m_vr_mode ;
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
					instance = new GameObject("tk2dUIAudioManager").AddComponent<LogicManager>();
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
		if ( to == VRMode.VR_2D )
		{
			m_vr_mode = VRMode.VR_2D;
			if ( CardBoard != null ) CardBoard.VRModeEnabled = false;
		}else // to 3d mode
		{
			m_vr_mode = VRMode.VR_3D;
			if ( CardBoard != null ) CardBoard.VRModeEnabled = true;
			
		}
	}
}


public enum VRMode
{
	VR_2D,
	VR_3D,
}
