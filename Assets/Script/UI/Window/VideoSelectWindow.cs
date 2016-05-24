using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
		{
			ClearVideos();

			List<VideoInfo> videoList = (List<VideoInfo>)msg.GetMessage(Global.MSG_POSTVIDEO_VIDEO_KEY );

			Debug.Log("Get Video List " + videoList.Count  );

			foreach(VideoInfo info in videoList )
			{
				// Debug.Log("Video " + info.title + " URL " + info.playUrl );
				CreateVideoInfoUnit( info );
			}
		}

	}

	protected override void OnBecomeVisible ()
	{
		base.OnBecomeVisible ();
		Panel.gameObject.SetActive( true );
	}
	protected override void OnBecomeInvsible ()
	{
		base.OnBecomeInvsible ();
		Panel.gameObject.SetActive( false );
	}

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
		msg.AddMessage(Global.MSG_REQUESTVIDEO_NUMBER_KEY , "6");
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



}