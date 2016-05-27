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
	[SerializeField] GameObject SidePatternPrefab;
	/// <summary>
	/// The grid panel to save the units
	/// </summary>
	[SerializeField] GridLayoutGroup Panel;
	[SerializeField] GridLayoutGroup helpPanel;
	[SerializeField] GridLayoutGroup sidePanel;
	const int sidePatternNumber = 10;

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
	[SerializeField] VRFunctionButton homeButton;

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
		m_videoList = (List<VideoInfo>)msg.GetMessage(Global.MSG_POSTVIDEO_VIDEO_KEY );
		temPage = 0;

		ShowVideoOnTempPage();
	}

	void ShowVideoOnTempPage()
	{
		if ( unitList.Count > 0 ) 
			ClearVideos( ClearType.Disapper );
		
		for ( int i = temPage * VideoPerPage ; i < ( temPage + 1 ) *  VideoPerPage && i < m_videoList.Count ; ++ i ) 
		{
			CreateVideoInfoUnit( m_videoList[i] );
		}

		UpdatePageChangeButtons();
		UpdateSidePattern();
	}

	void UpdatePageChangeButtons()
	{
		UpButton.m_Enable = true;
		DownButton.m_Enable = true;

		if ( temPage <= 0 ) UpButton.m_Enable = false;
		if ( ( temPage + 1 ) * VideoPerPage >= m_videoList.Count ) DownButton.m_Enable = false;
	}

	protected override void OnBecomeVisible ( float time )
	{
		base.OnBecomeVisible ( time );
		Panel.gameObject.SetActive( true );

		foreach( VideoInfoUnit unit in unitList )
		{
			unit.OnBecomeVisible( time );
		}

		UpButton.OnBecomeVisible( time );
		DownButton.OnBecomeVisible( time );
		homeButton.OnBecomeVisible( time );
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


		UpButton.OnBecomeInvisible( time );
		DownButton.OnBecomeInvisible( time );
		homeButton.OnBecomeInvisible( time );
		foreach( SidePattern p in sidePatternList ) p.OnBecomeInvisible( time );
	}

	public void DisablePanel() { Panel.gameObject.SetActive(false); }

	void Awake () {
		if ( VideoInfoUnitPrefab == null )
			VideoInfoUnitPrefab = (GameObject)Resources.Load("Prefab/UI/VideoInfoUnit");
		if ( Panel == null )
			Debug.LogError("Cannot find Panel");
		
	}

	void Start() {
		CreateSidePattern();
		RequestVideoList();
	}

	public void RequestVideoList()
	{
		RequestVideoList( 50 );
	}
	public void RequestVideoList ( int number )
	{
		URLRequestMessage msg = new URLRequestMessage(this);
		msg.AddMessage(Global.MSG_REQUESTVIDEO_NUMBER_KEY , number.ToString());
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

	/// <summary>
	/// create 10 side patterns and set to disabled, for save
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
		int totalPage = ( m_videoList.Count - 1 ) / VideoPerPage + 1;

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
		Disapper
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
		if ( temPage * VideoPerPage < m_videoList.Count )
		{
			temPage ++ ;
			ClearVideos( ClearType.Up);
			ShowVideoOnTempPage();
		}
	}

}