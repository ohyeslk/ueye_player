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
		if ( Instance != this )
		{
			Destroy( this.gameObject );
		}else
		{
			
			SetVRMode(Global.initMode);
			if ( gameObject.GetComponent<UserManager>() == null )
				gameObject.AddComponent<UserManager>();
			if ( gameObject.GetComponent<SocketManager>() == null )
				gameObject.AddComponent<SocketManager>();
			if ( gameObject.GetComponent<VoiceManager>() == null )
				gameObject.AddComponent<VoiceManager>();
			if ( gameObject.GetComponent<HTTPManager>() == null )
				gameObject.AddComponent<HTTPManager>();

			DontDestroyOnLoad(this.gameObject);
		}
		// for test
		// TODO remove this codes
//		WindowArg arg = new WindowArg(this);
//		arg.type = WindowArg.Type.PLAY_WINDOW;
//		VREvents.FireActiveWindow( arg );
	
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

	public float sensity;
	public float threshod;
	public GUIStyle style;

	void OnGUI()
	{
		CardboardHead.SetData( threshod , sensity );
		GUILayout.Label(" ");
		GUILayout.Label(" ");
		sensity = GUILayout.HorizontalScrollbar( sensity * 100f  , 10.0f , 0 , 100.0f , GUILayout.Height(50f), GUILayout.ExpandHeight( true ) ) / 100f ;
		threshod = GUILayout.HorizontalScrollbar( threshod * 100f  , 10.0f , 0 , 100.0f , GUILayout.MinHeight(50f) ,  GUILayout.ExpandHeight( true ) ) / 100f ;
		GUILayout.Label( "Sensity " + CardboardHead.MoveSensity + " | threshod " + CardboardHead.FreeAngleThreshod , style );

//		sensity = GUI.HorizontalScrollbar( new Rect(25, 25, 500, 150) , sensity * 100f  , 10.0f , 0 , 100.0f , style) / 100f ;
//		threshod = GUI.HorizontalScrollbar( new Rect(25, 75, 500, 150) ,  threshod * 100f , 10.0f , 0 , 100.0f , style) / 100f ;
//		CardboardHead.SetData( threshod , sensity );
//		GUI.Label( new Rect(25, 125, 500, 150) ,  "Sensity " + CardboardHead.MoveSensity + " | threshod " + CardboardHead.FreeAngleThreshod , style);
	}

}


public enum VRMode
{
	VR_None = 0,
	VR_2D = 1,
	VR_3D = 2,
	VR_All = 3,
}
