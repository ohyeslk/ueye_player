﻿using UnityEngine;
using System.Collections;

public class Global {
	
	public static string MSG_VIDEO_INFO_KEY = "info";

	public static string MSG_REQUESTVIDEO_NUMBER_KEY = "number";

	public static string MSG_POSTVIDEO_VIDEO_KEY = "videolist";
	public static string MSG_POSTVIDEO_NAME_KEY = "name";

	public static string MSG_REQUEST_TEXTURE_SPRITE_KEY = "sprite";

	public static string MSG_SWITCHVRMODE_MODE_KEY = "VRMode";

	public static string MSG_POSTCATEGORY_CATEGORYLIST_KEY = "categoryList";
	public static string MSG_REQUEST_CATEGORYVIDEO_CATEGORY_KEY = "category";

	public static string MSG_LOGIN_USERNAME = "username";
	public static string MSG_LOGIN_PASSWORD = "password";
	public static string MSG_LOGIN_TOKEN = "token";

	public static string MSG_BAIDU_YUYIN_TOKEN = "access_token";

	public static string MSG_BAIDU_YYIN_TRANSLATE_JSON = "json";
	public static string MSG_BAIDU_YYIN_TRANSLATE_RESULT = "result";

	public static string MSG_USER_VOTE_OPTION = "option";

	public static string VideoRequestURL = "http://balala-dev-beta.us-west-1.elasticbeanstalk.com/api/v1/channels/recordedstreams?days=NUMBER";
	public static string CategoryRequstURL = "http://balala-dev-beta.us-west-1.elasticbeanstalk.com/api/v1/channels/category/";
	public static string CategoryVideoRequestURL = "http://balala-dev-beta.us-west-1.elasticbeanstalk.com/api/v1/channels/category/CATEGORY";
	public static string LiveVideoRequestURL = "http://balala-dev-beta.us-west-1.elasticbeanstalk.com/api/v1/channels/livestreams?index=0&days=NUMBER";
	public static string ChatSocketURL = "ws://balala-dev.us-west-1.elasticbeanstalk.com/socket.io/?EIO=3&transport=websocket";
	public static string LoginURL = "http://balala-dev.us-west-1.elasticbeanstalk.com/login";
	public static string BaiduYuyinURL = "https://openapi.baidu.com/oauth/2.0/token?grant_type=client_credentials" +
		"&client_id=" +
		"9scx4TOCR18A2EZNlXkOYjUK" +
		"&client_secret=" +
		"87f007189f2b686e7b40b2a140d585b5";

	public static VRMode initMode = VRMode.VR_2D;

	public static Vector3 ONHOVERV3_PHASE_EXIT = Vector3.zero;

	public static string LIVE_VIDEOLIST_NAME = "Live";

	public static ulong GetHashFromString( string str )
	{
		ulong hash = 8731;
		int i = str.Length;

		while ( i > 0 )
		{
			hash = ( hash * 33 ) ^ (ulong)str[--i];
		}
		return hash >> 0 ;
	}
}

[System.Serializable]
public struct VideoInfo
{
	public Sprite Post;
	public string title;
	public string playUrl;
	public string coverUrl;
	public string description;
	public bool isLive;
	public long id;
}

[System.Serializable]
public struct CategoryInfo
{
	public Sprite Post;
	public string name;
	public string bgUrl;
}