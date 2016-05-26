using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HTTPManager : MonoBehaviour {

	public HTTPManager() { s_Instance = this; }
	public static HTTPManager Instance { get { return s_Instance; } }
	private static HTTPManager s_Instance;

	public delegate void RequestHandler(WWW www, URLRequestMessage postMsg );

	/// <summary>
	/// Save the texture in in the dictionary
	/// </summary>
	Dictionary<string,Texture2D> textureCache = new Dictionary<string, Texture2D>();

	public void OnEnable()
	{
		VREvents.RequestVideoList += RequestVideoInfo;
		VREvents.RequesTexture += RequestTexture;
		VREvents.RequestCategory += RequestCategory;
		VREvents.RequestCategoryVideoList += RequestCategoryVideoList;
	}

	public void OnDisable()
	{
		VREvents.RequestVideoList -= RequestVideoInfo;
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
			Debug.Log("Request c v l " + url );
		}
		StartCoroutine( WaitForRequest( url , CategoryVideoHandeler , msg ));
	}

	/// <summary>
	/// Deal with the request video info
	/// </summary>
	/// <param name="msg">Message.</param>
	void RequestVideoInfo( URLRequestMessage msg )
	{
		Debug.Log(" Request Video Info ");
		string url = msg.url;
		if ( url == null || url == "")
		{
			string number = msg.GetMessage(Global.MSG_REQUESTVIDEO_NUMBER_KEY ).ToString();
			url = Global.VideoRequestURL.Replace( "NUMBER" , number );
		}

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
			Texture2D texture;
			if ( textureCache.TryGetValue( url , out texture) )
			{
				msg.AddMessage( Global.MSG_REQUEST_TEXTURE_TEXTURE_KEY , texture );
				VREvents.FirePostTexture( msg );
				return;
			}
		}
		StartCoroutine( WaitForRequest( url , TextureHandler , msg));
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
		Texture2D texture = www.texture;
		if ( !textureCache.ContainsKey( postMsg.url ))
			textureCache.Add( postMsg.url , texture );
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
