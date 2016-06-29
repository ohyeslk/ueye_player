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
		VREvents.RequestLogin += RequestLogin;
		VREvents.RequestBaiduYuyinToken += RequestBaiduYuyin;
		VREvents.RequestBaiduYuyinTranslate += RequestBaiduYuyinTranslate;

	}

	void RequestBaiduYuyinTranslate (URLRequestMessage msg)
	{
		string strJson= msg.GetMessage(Global.MSG_BAIDU_YYIN_TRANSLATE_JSON).ToString();

		string strURL = msg.url;
		System.Net.HttpWebRequest request;
		request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
		request.Method = "POST";
		// add header
		request.Headers.Add("apikey", "9scx4TOCR18A2EZNlXkOYjUK");
		request.ContentType = "application/json";

		byte[] payload;

		payload = System.Text.Encoding.UTF8.GetBytes(strJson);
		request.ContentLength = payload.Length;

		Stream writer = request.GetRequestStream();
		writer.Write(payload, 0, payload.Length);
		writer.Close();

		StartCoroutine( ReadStream( msg , request ));

	}

	IEnumerator ReadStream(URLRequestMessage msg, System.Net.HttpWebRequest request )
	{
		HttpHelperStream helper = new HttpHelperStream(request );
		helper.AsyBegin();

		while( !helper.Done ) { yield return null ; }

		string strValue = helper.Result;
		msg.AddMessage( Global.MSG_BAIDU_YYIN_TRANSLATE_RESULT , strValue );
		VREvents.FirePostBaiduYuyinTranslate( msg );

//		System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request;
//
//		System.IO.Stream s;
//		Debug.Log("Time " + Time.time );
//		s = response.GetResponseStream();
//		string StrDate = "";
//		string strValue = "";
//		StreamReader Reader = new StreamReader(s, Encoding.UTF8);
//		Debug.Log("Time " + Time.time );
//		while ((StrDate = Reader.ReadLine()) != null)
//		{
//			strValue += StrDate + "\r\n";
//			yield return null;
//		}
	}

	void RequestBaiduYuyin (URLRequestMessage msg)
	{
		string url = msg.url;
		if ( string.IsNullOrEmpty(url) )
		{
			url = Global.BaiduYuyinURL;
		}
		StartCoroutine( WaitForRequest( url , BaiduYuyinTokenHandler , msg ));
	}
		
	void RequestLogin (URLRequestMessage msg)
	{
		WWWForm form = new WWWForm();
		form.AddField("username" , msg.GetMessage(Global.MSG_LOGIN_USERNAME ).ToString() );
		form.AddField("password" , msg.GetMessage(Global.MSG_LOGIN_PASSWORD ).ToString() );

		string url = msg.url;
		if ( string.IsNullOrEmpty(url) )
		{
			url = Global.LoginURL;
		}
		StartCoroutine( WaitForRequest ( url , LoginHandler , msg , form ));
	}

	void RequestCategory( URLRequestMessage msg )
	{
		string url = msg.url;
		if (string.IsNullOrEmpty(url) )
		{
			url = Global.CategoryRequstURL;
		}
		StartCoroutine( WaitForRequest ( url , CategoryHandler , msg ));

	}

	void RequestCategoryVideoList( URLRequestMessage msg )
	{
		string url = msg.url;
		if ( string.IsNullOrEmpty(url) )
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
		if (string.IsNullOrEmpty(url) )
		{
			string number = msg.GetMessage(Global.MSG_REQUESTVIDEO_NUMBER_KEY ).ToString();
			url = Global.VideoRequestURL.Replace( "NUMBER" , number );
		}
		msg.AddMessage( Global.MSG_POSTVIDEO_NAME_KEY , "Latest" );

		StartCoroutine( WaitForRequest( url , DayVideoInfoHandler , msg));
	}

	void RequestLiveVideoInfo( URLRequestMessage msg )
	{
//		Debug.Log(" Request Live Video Info ");
		string url = msg.url;
		if ( string.IsNullOrEmpty(url) )
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
		
	public void OnDisable()
	{
		VREvents.RequestVideoList -= RequestVideoInfo;
		VREvents.RequestLiveVideoList -= RequestLiveVideoInfo;
		VREvents.RequesTexture -= RequestTexture;
		VREvents.RequestCategory -= RequestCategory;
		VREvents.RequestCategoryVideoList -= RequestCategoryVideoList;
		VREvents.RequestLogin -= RequestLogin;
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
				info.isLive = video.GetField("islive").i == 1;
				if ( info.isLive )
					Debug.Log( info.title + " is live " + info.isLive );
				res.Add( info );
			}
		}
		return res;
	}

	void BaiduYuyinTokenHandler( WWW www , URLRequestMessage msg )
	{
		JSONObject info = new JSONObject( www.text );

		string token = info.GetField("access_token").str;

		msg.AddMessage(Global.MSG_BAIDU_YUYIN_TOKEN , token );
		VREvents.FirePostBaiduYuyinToken(msg);
	}

	void LoginHandler( WWW www , URLRequestMessage msg )
	{
		JSONObject info = new JSONObject( www.text );

		string token = info.GetField("token").str;

		msg.AddMessage(Global.MSG_LOGIN_TOKEN , token );
		VREvents.FirePostLogin( msg );

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

	IEnumerator WaitForRequest(string url , RequestHandler handler , URLRequestMessage postMsg , WWWForm form = null )
	{
		string path = url;

		if ( Application.platform == RuntimePlatform.IPhonePlayer )
		{
			if ( File.Exists( HttpHelper.GetLocalFilePath( url ) ) )
				path = "file://" + HttpHelper.GetLocalFilePath( url );
		}

		WWW www;
		if ( form == null )
		{
			www= new WWW(path);
		}
		else
		{
//			Debug.Log("WWW with form");
			www = new WWW(path,form);
		}
		
		yield return www;

		if ( string.IsNullOrEmpty(www.error) ){
			handler(www, postMsg);
		} else {
			Debug.Log("WWW Error" + www.error );
		}

		www.Dispose();
	}

	IEnumerator WaitForRequestAsy( string url , RequestHandler handler , URLRequestMessage postMsg )
	{
		if ( !File.Exists( HttpHelper.GetLocalFilePath( url ) ) )
		{
			HttpHelper httpHelper = new HttpHelper( url );

			httpHelper.AsyDownload();

			while( !httpHelper.Done )
			{
				yield return null;
			}
		}

		string path = "file://" + HttpHelper.GetLocalFilePath( url );

		if ( Application.platform == RuntimePlatform.IPhonePlayer )
		{
			path = "file://" + HttpHelper.GetLocalFilePath( url );
		}

		WWW www = new WWW( path );
		yield return www;

		if ( string.IsNullOrEmpty(www.error) ){
			handler(www, postMsg);
		}else{

			Debug.Log("WWW Error" + www.error );
		}

		www.Dispose();
	}
}
