using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class VideoSelectWindow : UIWindow  {
	/// <summary>
	/// The prefabe of the unit in the window
	/// </summary>
	[SerializeField] GameObject VideoInfoUnitPrefab;
	/// <summary>
	/// The grid panel to save the units
	/// </summary>
	[SerializeField] GridLayoutGroup Panel;

	/// <summary>
	/// the list to save the unit 
	/// </summary>
	List<VideoInfoUnit> unitList = new List<VideoInfoUnit>();
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
			return ( m_videoList.Count - VideoPerPage * temPage ) < column ? ( m_videoList.Count - VideoPerPage * temPage ) : column;
		}
	}

	/// <summary>
	/// record the temperary page number
	/// </summary>
	int temPage = 0;

	/// <summary>
	/// cache the video list
	/// </summary>
	List<VideoInfo> m_videoList = new List<VideoInfo>();

	[SerializeField] VRFunctionButton UpButton;
	[SerializeField] VRFunctionButton DownButton;

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
		ClearVideos();
	}

	void RecieveVideoList( URLRequestMessage msg )
	{
		m_videoList = (List<VideoInfo>)msg.GetMessage(Global.MSG_POSTVIDEO_VIDEO_KEY );
		ShowVideoOnTempPage();
	}

	void ShowVideoOnTempPage()
	{
		ClearVideos();

		for ( int i = temPage * VideoPerPage ; i < ( temPage + 1 ) *  VideoPerPage && i < m_videoList.Count ; ++ i ) 
		{
			CreateVideoInfoUnit( m_videoList[i] );
		}

		UpdatePageChangeButtons();
	}

	void UpdatePageChangeButtons()
	{
		UpButton.m_Enable = true;
		DownButton.m_Enable = true;

		if ( temPage <= 0 ) UpButton.m_Enable = false;
		if ( temPage * VideoPerPage >= m_videoList.Count ) DownButton.m_Enable = false;
	}

	protected override void OnBecomeVisible ( float time )
	{
		base.OnBecomeVisible ( time );
		Panel.gameObject.SetActive( true );

		foreach( VideoInfoUnit unit in unitList )
		{
			unit.PlayFadeInAnimation( time );
		}

		UpButton.OnBecomeVisible( time );
		DownButton.OnBecomeVisible( time );
	}

	protected override void OnBecomeInvsible ( float time )
	{
		base.OnBecomeInvsible ( time );

		foreach( VideoInfoUnit unit in unitList )
		{
			unit.PlayFadeOutAnimation( time );
		}

		Sequence seq = DOTween.Sequence();
		seq.AppendInterval( time  );
		seq.AppendCallback( DisablePanel );


		UpButton.OnBecomeInvisible( time );
		DownButton.OnBecomeInvisible( time );
	}

	public void DisablePanel() { Panel.gameObject.SetActive(false); }

	void Awake () {
		if ( VideoInfoUnitPrefab == null )
			VideoInfoUnitPrefab = (GameObject)Resources.Load("Prefab/UI/VideoInfoUnit");
		if ( Panel == null )
			Debug.LogError("Cannot find Panel");
	}

	void Start() {
		RequestVideoList();
	}

	public void RequestVideoList()
	{
		URLRequestMessage msg = new URLRequestMessage(this);
		msg.AddMessage(Global.MSG_REQUESTVIDEO_NUMBER_KEY , "12");
		VREvents.FireRequestVideoList(msg);
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

	public void ClearVideos()
	{
		for( int i = unitList.Count - 1 ; i >= 0 ; --i )
		{
			unitList[i].Clear();
		}

		unitList.Clear();
	}

	public void OnLastPage()
	{
		if ( temPage > 0 ) 
		{
			temPage --;
			ShowVideoOnTempPage();
		}
	}

	public void OnNextPage()
	{
		if ( temPage * VideoPerPage < m_videoList.Count )
		{
			temPage ++ ;
			ShowVideoOnTempPage();
		}
	}

}