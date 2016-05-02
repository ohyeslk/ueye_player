using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Events
{
	public delegate void EventHandler(Message msg);

	public static event EventHandler StartApp;
	public static void FireStartApp(Message msg){if ( StartApp != null ) StartApp(msg) ; }
}

	
public class Message : EventArgs
{
	public Message(object _this){m_sender = _this;}
	object m_sender;
	Dictionary<string,object> m_dict;
	Dictionary<string,object> dict
	{
		get {
			if ( m_dict == null )
				m_dict = new Dictionary<string, object>();
			return m_dict;
		}
	}

	public object sender{get{return m_sender;}}

	public void SetSender(object sender){
		m_sender = sender;
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