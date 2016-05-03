using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class VREvents
{
	/// <summary>
	/// Event handler. handle the event with basic arg
	/// </summary>
	public delegate void EventHandler(BasicArg arg);


	public static event EventHandler StartApp;
	public static void FireStartApp(BasicArg arg){if ( StartApp != null ) StartApp(arg) ; }


	/// <summary>
	///  Message event handler. handle the event with message arg
	/// </summary>
	public delegate void MessageEventHandler(Message msg); 


	public static event MessageEventHandler ActiveWindow;
	public static void FireActiveWindow(Message arg){if ( ActiveWindow != null ) ActiveWindow(arg) ; }

	/// <summary>
	/// Video event handler. handle the event with UI message
	/// </summary>
	public delegate void UISensorEventHandler (UISensorArg arg);

	public static event UISensorEventHandler UIFocus;
	public static void FireUIFocus(UISensorArg arg){if ( UIFocus != null ) UIFocus(arg) ; }

	public static event UISensorEventHandler UIConfirm;
	public static void FireUIConfirm(UISensorArg arg){if ( UIConfirm != null ) UIConfirm(arg) ; }




}

public class BasicArg : EventArgs
{
	public BasicArg(object _this){m_sender = _this;}
	object m_sender;
	public object sender{get{return m_sender;}}
}

public class UISensorArg : BasicArg
{
	public UISensorArg(object _this ): base ( _this ){}
	public float focusTime;
	public float confirmTime;
	public enum SensorType
	{
		None,
		VideoUnit,
		Return,
		Play,
	}
	public SensorType type;
}

	
public class Message : BasicArg
{
	public Message(object _this):base(_this){}
	Dictionary<string,object> m_dict;
	Dictionary<string,object> dict
	{
		get {
			if ( m_dict == null )
				m_dict = new Dictionary<string, object>();
			return m_dict;
		}
	}
	public void AddMessage(string key, object val)
	{
		dict.Add(key, val);
	}
	public object GetMessage(string key)
	{
		object res;
		dict.TryGetValue(key , out res);
		return res;
	}
	public bool ContainMessage(string key)
	{
		return dict.ContainsKey(key);
	}
}