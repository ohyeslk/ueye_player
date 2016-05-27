using UnityEngine;
using System.Collections;

public class Global {
	
	public static string MSG_VIDEO_INFO_KEY = "info";

	public static string MSG_REQUESTVIDEO_NUMBER_KEY = "number";

	public static string MSG_POSTVIDEO_VIDEO_KEY = "videolist";

	public static string MSG_REQUEST_TEXTURE_TEXTURE_KEY = "sprite";

	public static string MSG_SWITCHVRMODE_MODE_KEY = "VRMode";

	public static string MSG_POSTCATEGORY_CATEGORYLIST_KEY = "categoryList";
	public static string MSG_REQUEST_CATEGORYVIDEO_CATEGORY_KEY = "category";


	public static string VideoRequestURL = "http://balala-dev-beta.us-west-1.elasticbeanstalk.com/api/v1/channels/recordedstreams?days=NUMBER";

	public static string CategoryRequstURL = "http://balala-dev-beta.us-west-1.elasticbeanstalk.com/api/v1/channels/category/";

	public static string CategoryVideoRequestURL = "http://balala-dev-beta.us-west-1.elasticbeanstalk.com/api/v1/channels/category/CATEGORY";

	public static VRMode initMode = VRMode.VR_2D;

	public static Vector3 ONHOVERV3_PHASE_EXIT = Vector3.zero;
}


[System.Serializable]
public struct VideoInfo
{
	public Sprite Post;
	public string title;
	public string playUrl;
	public string coverUrl;
	public string description;
}

[System.Serializable]
public struct CategoryInfo
{
	public Sprite Post;
	public string name;
	public string bgUrl;
}