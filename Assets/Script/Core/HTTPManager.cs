using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HTTPManager : MonoBehaviour {

	public HTTPManager() { s_Instance = this; }
	public static HTTPManager Instance { get { return s_Instance; } }
	private static HTTPManager s_Instance;

	public delegate void RequestHandler(WWW www, URLRequestMessage postMsg );

	public void OnEnable()
	{
		VREvents.RequestVideoList += RequestVideoInfo;
		VREvents.RequesTexture += RequestTexture;
	}

	public void OnDisable()
	{
		VREvents.RequestVideoList -= RequestVideoInfo;
		VREvents.RequesTexture += RequestTexture;
	}

	/// <summary>
	/// Deal with the request video info
	/// </summary>
	/// <param name="msg">Message.</param>
	void RequestVideoInfo( URLRequestMessage msg )
	{
		Debug.Log(" Request Video Info ");
		string url = msg.url;
		if ( url == null )
		{
			string number = msg.GetMessage(Global.MSG_REQUESTVIDEO_NUMBER_KEY ).ToString();
			url = Global.VideoRequestURL.Replace( "NUMBER" , number );
		}

		StartCoroutine( WaitForRequest( url , VideoInfoHandler , msg));
	}

	/// <summary>
	/// Deal with the request texture 
	/// </summary>
	/// <param name="msg">Message.</param>
	void RequestTexture( URLRequestMessage msg )
	{
		string url = msg.url;
		StartCoroutine( WaitForRequest( url , TextureHandler , msg));
	}

	/// <summary>
	/// Transfrom the video info from jason(string) to VideoInfo(class),
	/// save in the message with key Global.MSG_POSTVIDEO_VIDEO_KEY 
	/// and post the 'PostVideoList' event
	/// </summary>
	/// <param name="www">Www.</param>
	/// <param name="postMsg">Post message.</param>
	void  VideoInfoHandler( WWW www , URLRequestMessage postMsg)
	{
		List<VideoInfo> list = new List<VideoInfo>();

		JSONObject jason = new JSONObject( www.text );


		JSONObject dailyList = jason.GetField( "dailyList");
		if ( dailyList.IsArray )
		{
			foreach( JSONObject day in dailyList.list )
			{
//				long total  = day.GetField("total").IsNumber ? day.GetField("total").i : 0;
				JSONObject videoList = day.GetField("videoList");
				if ( videoList.IsArray )
				{
					foreach( JSONObject video in videoList.list )
					{
						VideoInfo info = new VideoInfo();
						info.title = video.GetField("title").str;
						info.description = video.GetField("description").str;
						info.playUrl = video.GetField("playUrl").str;
						info.coverUrl = video.GetField("coverForFeed").str;

						list.Add( info );
					}
				}
			}
		}

		postMsg.AddMessage(Global.MSG_POSTVIDEO_VIDEO_KEY , list );
		VREvents.FirePostVideoList( postMsg );
	}

	/// <summary>
	/// extract the texture in the www, 
	/// save in the message with key Global.MSG_REQUEST_TEXTURE_TEXTURE_KEY, 
	/// and post the event 'PostTexture'
	/// </summary>
	/// <param name="www">Www.</param>
	/// <param name="postMsg">Post message.</param>
	void TextureHandler( WWW www , URLRequestMessage postMsg )
	{
		Texture2D texture = www.texture;

		postMsg.AddMessage( Global.MSG_REQUEST_TEXTURE_TEXTURE_KEY , texture );
		VREvents.FirePostTexture( postMsg );
	}

	IEnumerator WaitForRequest(string url , RequestHandler handler , URLRequestMessage postMsg)
	{
//		Debug.Log( "Wait for request " + url  + " " + postMsg.postObj);
		WWW www = new WWW(url);
		yield return www;

		handler(www, postMsg);

		if ( www.error != null )
			Debug.Log("WWW Error" + www.error );
	}


}
