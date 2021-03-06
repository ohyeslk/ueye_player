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

	public static event MessageEventHandler WindowReturn;
	public static void FireWindowReturn(Message arg){if ( WindowReturn != null ) WindowReturn(arg); }

	public static event MessageEventHandler ShowDetail;
	public static void FireShowDetail(Message arg){if ( ShowDetail != null ) ShowDetail(arg); }

	public static event MessageEventHandler SwitchVRMode;
	public static void FireSwitchVRMode(Message arg) { if ( SwitchVRMode != null ) SwitchVRMode(arg); }

	public static event MessageEventHandler PlayVideo;
	public static void FirePlayVideo(Message arg){if ( PlayVideo != null ) PlayVideo(arg); }

	public static event MessageEventHandler ResetCenter;
	public static void FireResetCenter(Message arg){if ( ResetCenter != null ) ResetCenter(arg); }

	public static event MessageEventHandler UIInputResetTarget;
	public static void FireUIInputResetTarget(Message arg){if ( UIInputResetTarget != null ) UIInputResetTarget(arg); }



	/// <summary>
	/// URL event handler. handle with the URL related events
	/// </summary>
	public delegate void URLEventHandler(URLRequestMessage msg); 

	public static event URLEventHandler RequestCategory;
	public static void FireRequestCategory(URLRequestMessage arg){if ( RequestCategory != null ) RequestCategory(arg) ; }

	public static event URLEventHandler PostCategory;
	public static void FirePostCategory(URLRequestMessage arg){if ( PostCategory != null ) PostCategory(arg) ; }

	public static event URLEventHandler RequestVideoList;
	public static void FireRequestVideoList(URLRequestMessage arg){if ( RequestVideoList != null ) RequestVideoList(arg) ; }

	public static event URLEventHandler RequestLiveVideoList;
	public static void FireRequestLiveVideoList(URLRequestMessage arg){if ( RequestLiveVideoList != null ) RequestLiveVideoList(arg) ; }

	public static event URLEventHandler RequestCategoryVideoList;
	public static void FireRequestCategoryVideoList(URLRequestMessage arg){if ( RequestCategoryVideoList != null ) RequestCategoryVideoList(arg) ; }

	public static event URLEventHandler PostVideoList;
	public static void FirePostVideoList(URLRequestMessage arg){if ( PostVideoList != null ) PostVideoList(arg) ; }

	public static event URLEventHandler RequesTexture;
	public static void FireRequesTexture(URLRequestMessage arg){if ( RequesTexture != null ) RequesTexture(arg) ; }

	public static event URLEventHandler PostTexture;
	public static void FirePostTexture(URLRequestMessage arg){if ( PostTexture != null ) PostTexture(arg) ; }

	public static event URLEventHandler RequestLogin;
	public static void FireRequestLogin(URLRequestMessage arg){if ( RequestLogin != null ) RequestLogin(arg) ; }

	public static event URLEventHandler PostLogin;
	public static void FirePostLogin(URLRequestMessage arg){if ( PostLogin != null ) PostLogin(arg) ; }

	/// <summary>
	/// Active window event handler. Handle the window events
	/// </summary>
	public delegate void WindowEventHandler(WindowArg arg );

	public static event WindowEventHandler ActiveWindow;
	public static void FireActiveWindow(WindowArg arg){if ( ActiveWindow != null ) ActiveWindow(arg) ; }



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
}

public class URLRequestMessage : Message
{
	public URLRequestMessage(object _this ): base ( _this ){ postObj = _this;}
	public string url;
	public object postObj;
}

public class WindowArg : BasicArg
{
	public WindowArg( object _this ): base ( _this ){}
	public enum Type
	{
		SELECT_WINDOW,
		PLAY_WINDOW,
		DETAIL_WINDOWS
	}
	public Type type;

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