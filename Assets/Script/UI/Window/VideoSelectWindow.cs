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


	void OnDisable()
	{
		VREvents.ActiveWindow -= VREvents_ActiveWindow;
		VREvents.PostVideoList -= RecieveVideoList;
	}

	void OnEnable()
	{
		VREvents.ActiveWindow += VREvents_ActiveWindow;
		VREvents.PostVideoList += RecieveVideoList;
	}

	void RecieveVideoList( URLRequestMessage msg )
	{
		if ( msg.postObj == this )
		{
//			Debug.Log("Recieve video list");
			List<VideoInfo> videoList = (List<VideoInfo>)msg.GetMessage(Global.MSG_POSTVIDEO_VIDEO_KEY );

			foreach(VideoInfo info in videoList )
			{
				// Debug.Log("Video " + info.title + " URL " + info.playUrl );
				CreateVideoInfoUnit( info );
			}
		}
	}

	void VREvents_ActiveWindow (WindowArg arg)
	{
		if ( arg.type == WindowArg.Type.SELECT_WINDOW )
		{
			Panel.gameObject.SetActive(true);
		}else{
			Panel.gameObject.SetActive(false);
		}
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
		msg.AddMessage(Global.MSG_REQUESTVIDEO_NUMBER_KEY , "8");
		VREvents.FireRequestVideoList(msg);
	}

	public void PlayVideo( VideoInfo info )
	{
		Debug.Log("PlayVideo " + info.title );
		Message msg = new Message(this);
		msg.AddMessage( Global.MSG_PLAYVIDEO_INFO_KEY , info );
		VREvents.FirePlayVideo( msg );


		WindowArg arg = new WindowArg(this);
		arg.type = WindowArg.Type.PLAY_WINDOW;
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

			// set up the animation for the info unit
			VideoUnitInitAnimation animation = new VideoUnitInitAnimation();
			animation.delay = unitList.Count * 0.1f;
			animation.duration = 0.5f;

			// initilze the unit
			unit.Init( info , animation , this );

			// save the unit in the listx
			unitList.Add(unit);
		}
	}
	
	// Update is called once per frame
	void Update () {
		// CheckSelection();
	}
	/// <summary>
	/// the unit hovered in last frame(update every frame)
	/// </summary>
	VideoInfoUnit lastHoverUnit;
	/// <summary>
	/// check if the user see any unit
	/// </summary>
//	void CheckSelection()
//	{
//		CardboardHead head = Cardboard.Controller.Head;
//		RaycastHit hit;
//		VideoInfoUnit hoverUnit = null;
//		UIHoverEvent hoverEvent = new UIHoverEvent();
//
//		if ( Physics.Raycast(head.Gaze, out hit, Mathf.Infinity) )
//		{
//			Debug.Log("Hit " + hoverUnit.name );
//			hoverUnit = hit.collider.GetComponent<VideoInfoUnit>();
//			hoverEvent.point = hit.point;
//		}
//			
//		if ( hoverUnit != null ){
//			if ( lastHoverUnit == null )
//				hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.Begin;
//			else
//				hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.Middle;
//			
//			hoverUnit.OnHover(hoverEvent);
//
//		}
//		else if ( lastHoverUnit != null ) {
//			hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.End;
//			lastHoverUnit.OnHover(hoverEvent);
//		}
//
//		lastHoverUnit = hoverUnit;
//	}
}