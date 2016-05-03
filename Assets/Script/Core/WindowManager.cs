using UnityEngine;
using System.Collections;

public class WindowManager : MonoBehaviour {

	public WindowManager() { s_Instance = this; }
	public static WindowManager Instance { get { return s_Instance; } }
	private static WindowManager s_Instance;

	[SerializeField] VideoSelectWindow VideoSelectWindow;
	[SerializeField] VideoPlayWindow VideoPlayWindow;

	void Start()
	{
		if ( VideoSelectWindow == null )
		{
			GameObject tem = GameObject.Find("VideoSelectWindow");
			if ( tem != null )
				VideoSelectWindow = tem.GetComponent<VideoSelectWindow>();
		}

		if ( VideoPlayWindow == null )
		{
			GameObject tem = GameObject.Find("VideoPlayWindow");
			if ( tem != null )
				VideoPlayWindow = tem.GetComponent<VideoPlayWindow>();
		}
	}

	void OnDisable()
	{
		VREvents.UIConfirm -= VREvents_UIConfirm;
	}

	void OnEnable()
	{
		VREvents.UIConfirm += VREvents_UIConfirm;;
	}

	void VREvents_UIConfirm (UISensorArg varg)
	{
		if ( varg.type == UISensorArg.SensorType.VideoUnit)
		{
			Message msg = new Message(this);
			msg.AddMessage( "window" , "play" );

			VideoInfoUnit unit = (VideoInfoUnit)varg.sender;
			if ( unit != null ) {
				msg.AddMessage( "info" , unit.Info );
			}

			VREvents.FireActiveWindow(msg);
		}else if ( varg.type == UISensorArg.SensorType.Return )
		{

			Message msg = new Message(this);
			msg.AddMessage( "window" , "select" );

			VREvents.FireActiveWindow(msg);
		}
	}
}
