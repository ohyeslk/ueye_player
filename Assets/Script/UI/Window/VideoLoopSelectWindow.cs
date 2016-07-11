using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class VideoLoopSelectWindow : VRUIWindow {
	[SerializeField] Transform topPanel;
	[SerializeField] Transform bottomPanel;
	[SerializeField] Transform videoPanel;
	[SerializeField] Text videoTitle;
	[SerializeField] float UnitProcessInterval = 0.2f;

	[SerializeField] GameObject VideoInfoUnitPrefab;
	VideoLoopUnitManager videoUnitManager = new VideoLoopUnitManager();

	protected override void OnBecomeInvsible (float time)
	{
		base.OnBecomeInvsible (time);
	}

	protected override void OnBecomeVisible (float time)
	{
		base.OnBecomeVisible (time);
	}

	VideoLoopInfoManager videoInfoManager = new VideoLoopInfoManager();

	override protected void OnDisable()
	{
		base.OnDisable();
		VREvents.PostVideoList -= RecieveVideoList;
		VREvents.RequestCategoryVideoList -= ResetVideoList;
		VREvents.RequestVideoList -= ResetVideoList;
	}

	override protected void OnEnable()
	{
		base.OnEnable();
		VREvents.PostVideoList += RecieveVideoList;
		VREvents.RequestCategoryVideoList += ResetVideoList;
		VREvents.RequestVideoList += ResetVideoList;
	}

	void Start() {
		RequestVideoList();
	}

	public void RequestVideoList()
	{
		RequestLatestVideoList( 10 );
		RequestLiveVideoList( 10 );
	}

	public void RequestLatestVideoList ( int number )
	{
		
		URLRequestMessage msg = new URLRequestMessage(this);
		msg.AddMessage(Global.MSG_REQUESTVIDEO_NUMBER_KEY , number.ToString());
		VREvents.FireRequestVideoList(msg);
	}

	public void RequestLiveVideoList ( int number )
	{
		URLRequestMessage msg = new URLRequestMessage(this);
		msg.AddMessage(Global.MSG_REQUESTVIDEO_NUMBER_KEY , number.ToString());
		VREvents.FireRequestLiveVideoList(msg);
	}

	void ResetVideoList ( URLRequestMessage msg)
	{
		ClearVideo();
	}

	void ClearVideo()
	{
		videoUnitManager.ClearVideo();
	}

	void RecieveVideoList( URLRequestMessage msg )
	{
//		Debug.Log("Recieve Video List");
		videoInfoManager.SetVideoList( (List<VideoInfo>)msg.GetMessage( Global.MSG_POSTVIDEO_VIDEO_KEY ) 
			, msg.GetMessage( Global.MSG_POSTVIDEO_NAME_KEY ).ToString() );

		RefreshAndShowVideo( msg.GetMessage( Global.MSG_POSTVIDEO_NAME_KEY ).ToString() );
	}

	void RefreshAndShowVideo( string key )
	{
		ClearVideo();

		List<VideoInfo> videoList = videoInfoManager.GetVideoList( key );

		for( int i = 0 ; i < videoList.Count ; ++ i )
		{
			CreateVideoInfoUnit( videoList[i] );
		}
			
		videoUnitManager.UpdateUnitPosition( 0 , UnitProcessInterval );
	}

	void CreateVideoInfoUnit( VideoInfo info )
	{
		if ( VideoInfoUnitPrefab != null )
		{

			GameObject unitObj = Instantiate( VideoInfoUnitPrefab ) as GameObject;
			unitObj.transform.SetParent( videoPanel  );
			unitObj.transform.localScale = Vector3.one;
			unitObj.transform.localPosition = Vector3.zero;
			unitObj.name = "InfoU" + info.title;

			VideoLoopInfoUnit unit = unitObj.GetComponentInChildren<VideoLoopInfoUnit>();

			videoUnitManager.AddVideoLoopInfoUnit( unit , info );
		}
	}


	float offset = 0;
	void Update()
	{
		if ( Input.GetKey( KeyCode.G ) )
		{
			offset += 0.01f;
			videoUnitManager.UpdateUnitPosition( offset , UnitProcessInterval );
		}
	}
}

public class VideoLoopInfoManager
{
	public string temKey = "";
	Dictionary<string, List<VideoInfo>> videoListDict = new Dictionary<string, List<VideoInfo>>();


	public List<VideoInfo> GetVideoList ( string key ) 
	{
		if ( videoListDict.ContainsKey( key ) )
		{
			return videoListDict[key];
		}

		return new List<VideoInfo>();
	}

	public List<VideoInfo> GetVideoList()
	{
		return GetVideoList( temKey );
	}

	public int GetVideoCount()
	{
		return GetVideoList().Count;
	}

	public void SetVideoList(List<VideoInfo> list , string _name )
	{
		if ( videoListDict.ContainsKey(_name) )
		{
			videoListDict.Remove( _name );
		}

		videoListDict.Add( _name , list );
	}

	public void SetKey( string key )
	{
		temKey = key;
	}


}

public class VideoLoopUnitManager
{
	List<VideoLoopInfoUnit> videoLoopUnitList = new List<VideoLoopInfoUnit>();

	public void AddVideoLoopInfoUnit( VideoLoopInfoUnit unit , VideoInfo info )
	{
		unit.Init( info );
		videoLoopUnitList.Add( unit );
	}

	public void ClearVideo()
	{
		for( int i = videoLoopUnitList.Count - 1 ; i >= 0 ; --i )
		{
			videoLoopUnitList[i].Clear();
		}
	}

	public void UpdateUnitPosition( float offset , float interval )
	{
		float mid = ( videoLoopUnitList.Count - 1 ) * 0.5f + offset ;
		for( int i = videoLoopUnitList.Count - 1 ; i >= 0 ; --i )
		{
			float p = ( mid - i ) * interval ;
			videoLoopUnitList[i].UpdatePosition( p );
		}
	}
}