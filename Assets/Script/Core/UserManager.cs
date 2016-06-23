using UnityEngine;
using System.Collections;

public class UserManager : MonoBehaviour {

	static public string TOKEN
	{
		get {
			return m_token;
		}
	}
	static string m_token = "";

	static public string UserName
	{
		get {
			return "HaHaHa ~ Undifined";
		}
	}

	void Start()
	{
		Login();
	}

	void OnEnable()
	{
		VREvents.PostLogin += PostLogin;
	}
		
	void OnDisable()
	{
		VREvents.PostLogin -= PostLogin;
	}

	void PostLogin (URLRequestMessage msg)
	{
		if ( msg.postObj == this )
		{
			m_token = msg.GetMessage(Global.MSG_LOGIN_TOKEN ).ToString();
		}
	}
		
	void Login()
	{
		URLRequestMessage msg = new URLRequestMessage(this);
		msg.AddMessage(Global.MSG_LOGIN_USERNAME , "areshero");
		msg.AddMessage(Global.MSG_LOGIN_PASSWORD , "balala");
		VREvents.FireRequestLogin(msg);
	}
}
