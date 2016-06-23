using UnityEngine;
using System.Collections;
using System;
using SocketIOClient;
using System.Net.Sockets;
using System.IO;

public class SocketManager : MonoBehaviour {
//	static string url = "http://balala-dev.us-west-1.elasticbeanstalk.com";
	static string url = "http://54.183.94.108";
	Client client;

	void Start()
	{
		Connect();
	}

	void Connect()
	{
		client = new Client(url);

		client.Opened += SocketOpened;
		client.Message += SocketMessage;
		client.SocketConnectionClosed += SocketClosed;
		client.Error += SocketError;

		client.Connect();
	}

	void SocketOpened(object sender, EventArgs e)
	{
		Debug.Log("Opened");
	}

	void SocketMessage (object sender, MessageEventArgs e) {
		Debug.Log("Get messsage " + e.Message.MessageText );
		if ( e!= null && e.Message.Event == "message") {
			string msg = e.Message.MessageText;
		}
	}

	void SocketClosed(object sender, EventArgs e)
	{
		Debug.Log("Socket Closed");
	}

	void SocketError( object sender , SocketIOClient.ErrorEventArgs e )
	{
		Debug.Log("Socket Error " + e.Message );
	}

	void SendMessage( string key , string content )
	{
		Debug.Log("Send " + key + " " + content );
		client.Emit( key , content );
	}

	void Update()
	{
		if ( Input.GetKeyDown( KeyCode.C ) )
		{
			SendMessage( "enterChannel" , "1" );
		}

		if ( Input.GetKeyDown( KeyCode.M ) )
		{
			SendMessage( "sendMsg" , "hahaha" );
		}
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
