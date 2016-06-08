using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System;
using System.Text;

public class HTTPManager : MonoBehaviour {

	public HTTPManager() { s_Instance = this; }
	public static HTTPManager Instance { get { return s_Instance; } }
	private static HTTPManager s_Instance;

	public delegate void RequestHandler(WWW www, URLRequestMessage postMsg );

	/// <summary>
	/// Save the texture in in the dictionary
	/// </summary>
	Dictionary<string,Sprite> textureCache = new Dictionary<string, Sprite>();

	void Awake()
	{
		HttpHelper.Init();


	}

	public void OnEnable()
	{
		VREvents.RequestVideoList += RequestVideoInfo;
		VREvents.RequestLiveVideoList += RequestLiveVideoInfo;
		VREvents.RequesTexture += RequestTexture;
		VREvents.RequestCategory += RequestCategory;
		VREvents.RequestCategoryVideoList += RequestCategoryVideoList;
	}

	public void OnDisable()
	{
		VREvents.RequestVideoList -= RequestVideoInfo;
		VREvents.RequestLiveVideoList -= RequestLiveVideoInfo;
		VREvents.RequesTexture -= RequestTexture;
		VREvents.RequestCategory -= RequestCategory;
		VREvents.RequestCategoryVideoList -= RequestCategoryVideoList;
	}

	void RequestCategory( URLRequestMessage msg )
	{
		string url = msg.url;
		if ( url == null || url == "" )
		{
			url = Global.CategoryRequstURL;
		}
		StartCoroutine( WaitForRequest ( url , CategoryHandler , msg ));

	}

	void RequestCategoryVideoList( URLRequestMessage msg )
	{
		string url = msg.url;
		if ( url == null || url == "" )
		{
			string category = msg.GetMessage(Global.MSG_REQUEST_CATEGORYVIDEO_CATEGORY_KEY ).ToString();
			url = Global.CategoryVideoRequestURL.Replace("CATEGORY" , category );
			msg.AddMessage( Global.MSG_POSTVIDEO_NAME_KEY , category );
		}
		StartCoroutine( WaitForRequest( url , CategoryVideoHandeler , msg ));
	}

	/// <summary>
	/// Deal with the request video info
	/// </summary>
	/// <param name="msg">Message.</param>
	void RequestVideoInfo( URLRequestMessage msg )
	{
//		Debug.Log(" Request Video Info ");
		string url = msg.url;
		if ( url == null || url == "")
		{
			string number = msg.GetMessage(Global.MSG_REQUESTVIDEO_NUMBER_KEY ).ToString();
			url = Global.VideoRequestURL.Replace( "NUMBER" , number );
		}
		msg.AddMessage( Global.MSG_POSTVIDEO_NAME_KEY , "Latest" );

		StartCoroutine( WaitForRequest( url , DayVideoInfoHandler , msg));
	}

	void RequestLiveVideoInfo( URLRequestMessage msg )
	{
//		Debug.Log(" Request Video Info ");
		string url = msg.url;
		if ( url == null || url == "")
		{
			string number = msg.GetMessage(Global.MSG_REQUESTVIDEO_NUMBER_KEY ).ToString();
			url = Global.LiveVideoRequestURL.Replace( "NUMBER" , number );
		}
		msg.AddMessage( Global.MSG_POSTVIDEO_NAME_KEY , "Live" );

		StartCoroutine( WaitForRequest( url , DayVideoInfoHandler , msg));
	}

	/// <summary>
	/// Deal with the request texture 
	/// </summary>
	/// <param name="msg">Message.</param>
	void RequestTexture( URLRequestMessage msg )
	{
		string url = msg.url;
		{
			Sprite sprite;
			if ( textureCache.TryGetValue( url , out sprite) )
			{
				msg.AddMessage( Global.MSG_REQUEST_TEXTURE_SPRITE_KEY , sprite );
				VREvents.FirePostTexture( msg );
				return;
			}
				
		}
		StartCoroutine( WaitForRequestAsy( url , TextureHandler , msg));
	}

	/// <summary>
	/// Transfrom the video info from jason(string) to VideoInfo(class),
	/// save in the message with key Global.MSG_POSTVIDEO_VIDEO_KEY 
	/// and post the 'PostVideoList' event
	/// </summary>
	/// <param name="www">Www.</param>
	/// <param name="postMsg">Post message.</param>
	void  DayVideoInfoHandler( WWW www , URLRequestMessage postMsg)
	{
		List<VideoInfo> list = new List<VideoInfo>();

		JSONObject json = new JSONObject( www.text );

		JSONObject dailyList = json.GetField( "dailyList");
		if ( dailyList.IsArray )
		{
			foreach( JSONObject day in dailyList.list )
			{
				JSONObject videoList = day.GetField("videoList");
				list.AddRange( Json2VideoList( videoList ) );
			}
		}

		postMsg.AddMessage(Global.MSG_POSTVIDEO_VIDEO_KEY , list );
		VREvents.FirePostVideoList( postMsg );
	}

	void CategoryVideoHandeler( WWW www , URLRequestMessage postMsg )
	{
		JSONObject json = new JSONObject( www.text );
		List<VideoInfo> list = Json2VideoList( json.GetField("videoList") );
		string name = postMsg.GetMessage( Global.MSG_REQUEST_CATEGORYVIDEO_CATEGORY_KEY ).ToString();
		postMsg.AddMessage( Global.MSG_POSTVIDEO_VIDEO_KEY , list );
		VREvents.FirePostVideoList( postMsg );
	}
	
	List<VideoInfo> Json2VideoList( JSONObject obj )
	{
		List<VideoInfo> res = new List<VideoInfo>();
		if ( obj.IsArray )
		{
			foreach( JSONObject video in obj.list )
			{
				VideoInfo info = new VideoInfo();
				info.title = video.GetField("title").str;
				info.description = video.GetField("description").str;
				info.description = info.description.Replace( "\\r" , "\r");
				info.description = info.description.Replace( "\\n" , "\n");
				info.playUrl = video.GetField("playUrl").str;
				info.coverUrl = video.GetField("coverForFeed").str;

				res.Add( info );
			}
		}
		return res;
	}


	void CategoryHandler( WWW www , URLRequestMessage msg )
	{
		List<CategoryInfo> list = new List<CategoryInfo>();

		JSONObject categorys = new JSONObject( www.text );
		if ( categorys.IsArray )
		{
			foreach( JSONObject category in categorys.list )
			{
				CategoryInfo info = new CategoryInfo();
				info.name = category.GetField("name").str;
				info.bgUrl = category.GetField("bgPicture").str;

				list.Add( info );
			}
		}

		msg.AddMessage(Global.MSG_POSTCATEGORY_CATEGORYLIST_KEY , list );
		VREvents.FirePostCategory( msg );
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
		Texture2D tex = www.texture;
		Rect rec = new Rect(0,0,tex.width ,tex.height );
	
		Sprite sprite = Sprite.Create( tex , rec , new Vector2(0.5f,0.5f) , 100);

		if ( !textureCache.ContainsKey( postMsg.url ) && HttpHelper.GetDownloadState(postMsg.url) == HttpHelper.DownloadState.Finished)
			textureCache.Add( postMsg.url , sprite );
		
		postMsg.AddMessage( Global.MSG_REQUEST_TEXTURE_SPRITE_KEY , sprite );
		VREvents.FirePostTexture( postMsg );
	}

	IEnumerator WaitForRequest(string url , RequestHandler handler , URLRequestMessage postMsg)
	{
//		Debug.Log( "Wait for request " + url  + " " + postMsg.postObj);
		string path = url;

		if ( File.Exists( HttpHelper.GetLocalFilePath( url ) ) )
			path = "file://" + HttpHelper.GetLocalFilePath( url );
		
		WWW www = new WWW(path);
		yield return www;

		if ( string.IsNullOrEmpty(www.error) ){
			handler(www, postMsg);
		}else{
			
			Debug.Log("WWW Error" + www.error );
		}

		www.Dispose();
	}

	IEnumerator WaitForRequestAsy( string url , RequestHandler handler , URLRequestMessage postMsg )
	{
		//		Debug.Log("WaitForRequestAsy URL " + url );
		if ( !File.Exists( HttpHelper.GetLocalFilePath( url ) ) )
		{
			HttpHelper httpHelper = new HttpHelper( url );

			httpHelper.AsyDownload();

			while( !httpHelper.Done )
			{
				yield return null;
			}
		}
			
		Debug.Log("WaitForRequestAsy Local file path " + HttpHelper.GetLocalFilePath( url ) );
		WWW www = new WWW( "file://" + HttpHelper.GetLocalFilePath( url ) );
		yield return www;

		if ( string.IsNullOrEmpty(www.error) ){
			handler(www, postMsg);
		}else{

			Debug.Log("WWW Error" + www.error );
		}

		www.Dispose();
	}
}
