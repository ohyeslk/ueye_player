﻿using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.IO;
using SocketIO;

public class SocketManager : MonoBehaviour {
//	static string url = "http://balala-dev.us-west-1.elasticbeanstalk.com";
	public string url = "http://54.183.94.108";

	public SocketIOComponent socketIO;
	//	public Player	playerGameObj;

	void Awake()
	{
		if ( socketIO == null )
		{
			socketIO = gameObject.AddComponent<SocketIOComponent>();
			socketIO.autoConnect = true;
		}

		socketIO.url = url;
	}

	void Start () {
		socketIO.On( "response" , OnResponse );
		socketIO.On( "message" , OnMessage );
	}

	void Update()
	{
		if ( Input.GetKeyDown(KeyCode.E ))
		{
			EnterChanel();
		}

		if ( Input.GetKeyDown(KeyCode.M ))
		{
			SendMessage();
		}
	}

	public void EnterChanel()
	{
		JSONObject data = new JSONObject();
		Debug.Log("Enter Channel " + data["id"].ToString());
		socketIO.Emit("enterChannel" , data );
	}

	public void SendMessage()
	{
		JSONObject data = new JSONObject();
		data.AddField("data" , "hahaha");
		data.AddField("userid" , UserManager.UserName.ToString() );
		socketIO.Emit("sendmessage" , data );
	}

	void OnMessage( SocketIOEvent obj )
	{
		Debug.Log("On Message " + obj.data.GetField("data").str);
		ChatArg chatMessage = new ChatArg(this);
		chatMessage.message = obj.data.GetField("data").str;
		chatMessage.userName = obj.data.GetField("userid").str;
		VREvents.FireChatMessage(chatMessage);
	}

	void OnResponse( SocketIOEvent obj )
	{
		Debug.Log("On Response " + obj.data.GetField("message").str );
	}

///////////////////////////////

//	internal bool socketReady = false;
//	TcpClient client;
//	NetworkStream stream;
//	StreamWriter writer;
//	StreamReader reader;
//
//	static string host = "http://localhost";
//	static int port = 3000;
//	void Start()
//	{
//		Connect();
//	}
//
//	public void Connect()
//	{
//		try {
//			client = new TcpClient(host,port);
//
//			stream = client.GetStream(); 
//			writer = new StreamWriter(stream);
//			reader = new StreamReader(stream);   
//			socketReady = true;
//		}
//		catch (Exception e) {
//			Debug.Log("Socket error: " + e);
//		}
//	}
//
//	void Update()
//	{
//		if ( Input.GetKeyDown( KeyCode.C ) && Input.GetKey( KeyCode.LeftControl ) )
//		{
//			writeSocket( "enterChanel" + "500" );
//		}
//
//		if ( Input.GetKeyDown( KeyCode.M ) && Input.GetKey( KeyCode.LeftControl ) )
//		{
//			writeSocket( "sendMsg" + "hahaha" );
//		}
//
//		readSocket(  );
//	}
//
//	public void writeSocket(string theLine) {
//		if (!socketReady)
//			return;
//		if ( client.Available > 0  )
//			Debug.Log("available");
//		String foo = theLine + "\r\n";
//		writer.Write(foo);
//		writer.Flush();
//
//	}
//
//	public String readSocket() { 
//		if (!socketReady)
//			return ""; 
//		if (stream.DataAvailable){           
//			string message = reader.ReadLine();
//			Debug.Log("[read  socket]" + message );
//			return reader.ReadLine();
//		}
//		else{print("no value");
//			return "";
//		}
//
//	}
//	public void closeSocket() {
//		if (!socketReady)
//			return;
//		stream.Close();
//		reader.Close();
//		writer.Close();
//		socketReady = false;
//	}

}
