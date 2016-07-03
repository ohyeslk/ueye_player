using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.IO;
using SocketIO;

public class SocketManager : MonoBehaviour {
//	static string url = "http://balala-dev.us-west-1.elasticbeanstalk.com";
//	public string url = "http://54.183.94.108";

	public SocketIOComponent socketIO;
	//	public Player	playerGameObj;

	void OnEnable()
	{
		VREvents.PostChatMessageToServer += OnPostChatMessageToServer;
		VREvents.UserVote += OnUserVote;
	}

	void OnUserVote (Message msg)
	{
		Debug.Log("On UserVote");
	}

	void OnPostChatMessageToServer (ChatArg msg)
	{
		Debug.Log("Message To Server");
		postMessage( msg );
	}

	void OnDisable()
	{
		VREvents.PostChatMessageToServer -= OnPostChatMessageToServer;
		VREvents.UserVote -= OnUserVote;
	}

	void Awake()
	{
		if ( socketIO == null )
		{
			socketIO = gameObject.AddComponent<SocketIOComponent>();
			socketIO.autoConnect = true;
		}

		socketIO.url = Global.ChatSocketURL;
	}

	void Start () {
		socketIO.On( "response" , OnResponse );
		socketIO.On( "message" , OnMessage );
		socketIO.On( "newVoteCreated" , OnVoteCreated );
		socketIO.On( "voteUpdate" , OnVoteUpdate );
	}

	public void EnterChanel( string id )
	{
		JSONObject data = new JSONObject();
		data.AddField("id" , id );
		Debug.Log("Enter Channel " + data["id"].ToString());
		socketIO.Emit("enterChannel" , data );
	}

//	public void postMessage( string msgData )
//	{
//		JSONObject data = new JSONObject();
//		data.AddField("data" , msgData );
//		data.AddField("userid" , UserManager.UserName.ToString() );
//		socketIO.Emit("sendmessage" , data );
//	}

	public void postMessage( ChatArg msgChatArg )
	{
		JSONObject data = new JSONObject();
		data.AddField("data" , msgChatArg.message );
		data.AddField("directionX" , msgChatArg.cameraForward.x );
		data.AddField("directionY" , msgChatArg.cameraForward.y );
		data.AddField("directionZ" , msgChatArg.cameraForward.z );
		data.AddField("userid" , UserManager.UserName.ToString() );

		JSONObject send = new JSONObject();
		send.AddField("payload" , data );

		socketIO.Emit("sendmessage" , send );
	}

	void OnVoteCreated( SocketIOEvent obj )
	{
		JSONObject data = new JSONObject();

		VoteArg msg = new VoteArg( this );
		VREvents.FireNewVoteCreated( msg );
	}

	void OnVoteUpdate( SocketIOEvent obj )
	{

		VoteArg msg = new VoteArg( this );
		VREvents.FireNewVoteUpdate( msg );
	}


	void OnMessage( SocketIOEvent obj )
	{
		ChatArg chatMessage = new ChatArg(this);
		JSONObject payload = obj.data.GetField("payload");
		chatMessage.message = payload.GetField("data").str;
		chatMessage.userName = payload.GetField("userid").str;
		chatMessage.cameraForward = new Vector3( payload.GetField("directionX").f , payload.GetField("directionY").f ,payload.GetField("directionZ").f );
		VREvents.FireChatMessageRecieve(chatMessage);
	}

	void OnResponse( SocketIOEvent obj )
	{
		string message = obj.data.GetField("message").str;
		if ( message == "I got you!")
		{
			EnterChanel("r");
		}

		Debug.Log("On Response " + obj.data.GetField("message").str );
	}

	void Update()
	{
		if ( Input.GetKeyDown( KeyCode.C ) && Input.GetKey( KeyCode.LeftControl ) )
		{
			EnterChanel("r");
		}

//		if ( Input.GetKeyDown( KeyCode.M ) && Input.GetKey( KeyCode.LeftControl ) )
//		{
//			postMessage("hahaha");
//		}

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
