using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class VideoSelectWindow : VRUIWindow  {
	/// <summary>
	/// The prefabe of the unit in the window
	/// </summary>
	[SerializeField] GameObject VideoInfoUnitPrefab;
	[SerializeField] GameObject SidePatternPrefab;
	/// <summary>
	/// The grid panel to save the units
	/// </summary>
	[SerializeField] GridLayoutGroup Panel;
	[SerializeField] GridLayoutGroup helpPanel;
	[SerializeField] GridLayoutGroup sidePanel;
	const int sidePatternNumber = 10;
	const float videoCreateDelay = 0.15f;

	/// <summary>
	/// the list to save the unit 
	/// </summary>
	List<VideoInfoUnit> unitList = new List<VideoInfoUnit>();
	/// <summary>
	/// save the side pattern
	/// </summary>
	List<SidePattern> sidePatternList = new List<SidePattern>();
	// Use this for initialization

	public int row;
	public int column;

	/// <summary>
	/// the number of the video info shown in per page
	/// </summary>
	/// <value>The video per page.</value>
	public int VideoPerPage { get { return row * column ; } }

	/// <summary>
	///  the number of the video pero row in temperary page
	/// </summary>
	/// <value>The video per row.</value>
	public int VideoPerRow {
		get{
			return ( m_videoManager.GetVideoCount() - VideoPerPage * temPage ) < column ? ( m_videoManager.GetVideoCount() - VideoPerPage * temPage ) : column;
		}
	}

	/// <summary>
	/// record the temperary page number
	/// </summary>
	int temPage = 0;


	[SerializeField] VRFunctionButton UpButton;
	[SerializeField] VRFunctionButton DownButton;
	[SerializeField] VRFunctionButton homeButton;
	[SerializeField] TopButton videoTopButton;
	[SerializeField] TopButton liveTopButton;
	VRBasicButton[] buttonList;

	public class VideoListManager
	{
		public string videoListName;

		/// <summary>
		/// temperary type of the video list
		/// </summary>
		enum Type
		{
			Video,
			Live,
		}
		Type type;

		public string name;

		List<VideoInfo> m_videoList = new List<VideoInfo>();

		/// <summary>
		/// save the live video list
		/// </summary>
		List<VideoInfo> m_liveVideoList = new List<VideoInfo>();
		/// <summary>
		/// save the normal video list
		/// </summary>
		List<VideoInfo> m_normalVideoList = new List<VideoInfo>();

		public List<VideoInfo> GetVideoList () 
		{
			if ( type == Type.Video )
				return m_normalVideoList;
			if ( type == Type.Live )
				return m_liveVideoList;
	
			return new  List<VideoInfo>();
		}

		public int GetVideoCount()
		{
			return GetVideoList().Count;
		}

		/// <summary>
		/// save the video list
		/// </summary>
		/// <returns><c>true</c>,if need refresh <c>false</c> otherwise.</returns>
		/// <param name="list"> the list of vide info.</param>
		/// <param name="_name"> the name of the video list.</param>
		public bool SetVideoList(List<VideoInfo> list , string _name )
		{
			name = _name;
			if ( name == Global.LIVE_VIDEOLIST_NAME ) 
				m_liveVideoList = list;
			else
				m_normalVideoList = list;
			
			if ( name == Global.LIVE_VIDEOLIST_NAME ) 
				return type == Type.Live;
			else
				return type == Type.Video;
			
		}

		public void ToVideo()
		{
			type = Type.Video;
		}

		public void ToLive()
		{
			type = Type.Live;
		}
	}

	VideoListManager m_videoManager = new VideoListManager();

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

	void ResetVideoList (URLRequestMessage msg)
	{
		ClearVideos( ClearType.Disapper );
	}

	void RecieveVideoList( URLRequestMessage msg )
	{
		if ( m_videoManager.SetVideoList( (List<VideoInfo>)msg.GetMessage(Global.MSG_POSTVIDEO_VIDEO_KEY ) 
			, msg.GetMessage( Global.MSG_POSTVIDEO_NAME_KEY).ToString() ) )
		{
			RefreshAndShowVideo();
		}
		if ( msg.GetMessage( Global.MSG_POSTVIDEO_NAME_KEY ).ToString() != Global.LIVE_VIDEOLIST_NAME )
		{
			videoTopButton.SetName( msg.GetMessage( Global.MSG_POSTVIDEO_NAME_KEY ).ToString() );
		}
	}

	/// <summary>
	/// set the tem page to 0 and show the video on this page
	/// </summary>
	void RefreshAndShowVideo()
	{
		temPage = 0;
		ShowVideoOnTempPage();
	}

	/// <summary>
	/// Clear the videos if there is any,
	/// Create the new video info units,
	/// update the page change buttons,
	/// update the side pattern,
	/// </summary>
	void ShowVideoOnTempPage()
	{
		if ( unitList.Count > 0 ) 
			ClearVideos( ClearType.Disapper );
		UpdatePageChangeButtons();
		UpdateSidePattern();

		for ( int i = temPage * VideoPerPage ; i < ( temPage + 1 ) *  VideoPerPage && i < m_videoManager.GetVideoCount() ; ++ i ) 
		{
			CreateVideoInfoUnit( m_videoManager.GetVideoList()[i] );
		}
	}

	/// <summary>
	/// reset the m_Enable of the page change buttons,
	/// according to whether the page can be turned up/down,
	/// </summary>
	void UpdatePageChangeButtons()
	{
		UpButton.m_Enable = true;
		DownButton.m_Enable = true;

		if ( temPage <= 0 ) 
		{
			UpButton.m_Enable = false;
		}
		if ( ( temPage + 1 ) * VideoPerPage >= m_videoManager.GetVideoCount() ) DownButton.m_Enable = false;
	}

	protected override void OnBecomeVisible ( float time )
	{
		base.OnBecomeVisible ( time );
		Panel.gameObject.SetActive( true );

		foreach( VideoInfoUnit unit in unitList )
		{
			unit.OnBecomeVisible( time );
		}
			
		foreach( VRBasicButton btn in buttonList )
			btn.OnBecomeVisible( time , true );
		
		foreach( SidePattern p in sidePatternList ) p.OnBecomeVisible( time );
	}

	protected override void OnBecomeInvsible ( float time )
	{
		base.OnBecomeInvsible ( time );

		foreach( VideoInfoUnit unit in unitList )
		{
			unit.OnBecomeInvisible( time );
		}

		Sequence seq = DOTween.Sequence();
		seq.AppendInterval( time  );
		seq.AppendCallback( DisablePanel );

		foreach( VRBasicButton btn in buttonList )
			btn.OnBecomeInvisible( time , false );
		
		foreach( SidePattern p in sidePatternList ) p.OnBecomeInvisible( time );
	}

	public void DisablePanel() { Panel.gameObject.SetActive(false); }

	void Awake () {
		if ( VideoInfoUnitPrefab == null )
			VideoInfoUnitPrefab = (GameObject)Resources.Load("Prefab/UI/VideoInfoUnit");
		if ( Panel == null )
			Debug.LogError("Cannot find Panel");
		buttonList = GetComponentsInChildren<VRBasicButton>();
	}

	void Start() {
		CreateSidePattern();
		RequestVideoList();
	}

	public void RequestVideoList()
	{
		RequestLatestVideoList( 50 );
		RequestLiveVideoList( 50 );
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

	public void ShowVideoDetail( VideoInfo info )
	{
		Message msg = new Message(this);
		msg.AddMessage( Global.MSG_VIDEO_INFO_KEY , info );
		VREvents.FireShowDetail( msg );


		WindowArg arg = new WindowArg(this);
		arg.type = WindowArg.Type.DETAIL_WINDOWS;
		VREvents.FireActiveWindow( arg );
	}

	/// <summary>
	/// create a unit according to the prefab
	/// </summary>
	/// <param name="index">the index of the unit </param>
	void CreateVideoInfoUnit( VideoInfo info )
	{
		if ( VideoInfoUnitPrefab != null )
		{
			GameObject unitObj = Instantiate( VideoInfoUnitPrefab ) as GameObject;
			unitObj.transform.SetParent( Panel.transform  );
			unitObj.transform.localScale = Vector3.one;
			unitObj.transform.localPosition = Vector3.zero;
			unitObj.name = "InfoU" + info.title;

			VideoInfoUnit unit = unitObj.GetComponentInChildren<VideoInfoUnit>();

			// initilze the unit
			unit.Init( info , unitList.Count , this );

			// save the unit in the listx
			unitList.Add(unit);
		}
	}

	/// <summary>
	/// create <sidePatternNumer>(=10) side patterns and set to disabled, for save
	/// </summary>
	void CreateSidePattern()
	{
		for( int i = 0 ;i < sidePatternNumber ; ++i )
		{
			GameObject sideObj = Instantiate( SidePatternPrefab ) as GameObject;
			sideObj.transform.SetParent( sidePanel.transform );
			sideObj.transform.localScale = Vector3.one;
			sideObj.transform.localPosition = Vector3.zero;
			sideObj.transform.localRotation = Quaternion.identity;
			sideObj.name = "side" + i.ToString();
			sideObj.SetActive( false );

			SidePattern pattern = sideObj.GetComponent<SidePattern>();

			sidePatternList.Add( pattern );
		}
	}

	void UpdateSidePattern()
	{
		int totalPage = ( m_videoManager.GetVideoCount() - 1 ) / VideoPerPage + 1;

		// set the the pattern
		for( int i = sidePatternNumber -1; i >= 0 ; -- i )
		{
			if ( i < totalPage )
			{
				sidePatternList[i].gameObject.SetActive( true );
			}else
			{
				sidePatternList[i].gameObject.SetActive( false );
			}
			if ( i == temPage )
			{
				sidePatternList[i].PatternEnable = true;
			}else
			{
				sidePatternList[i].PatternEnable = false;
			}
		}
	}

	public enum ClearType
	{
		Up,
		Down,
		Disapper,
		Side
	}
	public void ClearVideos( ClearType type )
	{
		
		for( int i = 0; i < unitList.Count ; ++i )
		{
			unitList[i].transform.SetParent( helpPanel.transform , true );
			unitList[i].Clear(type);
		}

		unitList.Clear();
	}


/***************
 * called by button
 * *************/

	public void OnLastPage()
	{
		if ( temPage > 0 ) 
		{
			temPage --;
			ClearVideos( ClearType.Down);
			ShowVideoOnTempPage();
		}
	}

	public void OnNextPage()
	{
		if ( temPage * VideoPerPage < m_videoManager.GetVideoCount() )
		{
			temPage ++ ;
			ClearVideos( ClearType.Up );
			ShowVideoOnTempPage();
		}
	}

	public void OnTopVideo()
	{
		m_videoManager.ToVideo();
		RefreshAndShowVideo();
		videoTopButton.SetHighlighted( true );
		liveTopButton.SetHighlighted( false );
	}

	public void OnTopLive()
	{
		RequestLiveVideoList( 50 );
		m_videoManager.ToLive();
//		RefreshAndShowVideo();
		videoTopButton.SetHighlighted( false );
		liveTopButton.SetHighlighted( true );
	}

	public void StartRecord()
	{
		Debug.Log("Start Record");
		if ( Application.platform == RuntimePlatform.Android )
		{
			Debug.Log("Begin Record in Android");
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			jo.Call("showRecord", "rtmp://ec2-54-183-98-223.us-west-1.compute.amazonaws.com/liveout/s");
		}
	}

}