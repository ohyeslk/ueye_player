using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class VideoSelectWindow : MonoBehaviour  {
	/// <summary>
	/// The prefabe of the unit in the window
	/// </summary>
	[SerializeField] GameObject VideoInfoUnitPrefab;
	/// <summary>
	/// The grid panel to save the units
	/// </summary>
	[SerializeField] GridLayoutGroup Panel;
	/// <summary>
	/// FOR TEST: the lib of the sprite 
	/// </summary>
	[SerializeField] Sprite[] spriteList;

	/// <summary>
	/// the list to save the unit 
	/// </summary>
	List<VideoInfoUnit> unitList = new List<VideoInfoUnit>();
	// Use this for initialization

	void Awake () {
		if ( VideoInfoUnitPrefab == null )
			VideoInfoUnitPrefab = (GameObject)Resources.Load("Prefab/UI/VideoInfoUnit");
		if ( Panel == null )
			Debug.LogError("Cannot find Panel");
	}

	void Start() {
		for( int i = 0 ; i < 8 ; ++ i )
		{
			CreateVideoInfoUnit(i);
		}

	}

	/// <summary>
	/// create a unit according to the prefab
	/// </summary>
	/// <param name="index">the index of the unit </param>
	void CreateVideoInfoUnit( int index )
	{
		if ( VideoInfoUnitPrefab != null )
		{
			GameObject unitObj = Instantiate( VideoInfoUnitPrefab ) as GameObject;
			unitObj.transform.SetParent( Panel.transform  );
			unitObj.transform.localScale = Vector3.one;
			unitObj.transform.localPosition = Vector3.zero;

			VideoInfoUnit unit = unitObj.GetComponent<VideoInfoUnit>();

			//TODO : initilize the video infor online
			VideoInfo info = new VideoInfo();
			info.Post = spriteList[Random.Range(0,spriteList.Length)] ;
			info.name = spriteList[Random.Range(0,spriteList.Length)].name ;

			// set up the animation for the info unit
			VideoUnitInitAnimation animation = new VideoUnitInitAnimation();
			animation.delay = index * 0.1f;
			animation.duration = 0.5f;

			// initilze the unit
			unit.Init( info , animation );

			// save the unit in the listx
			unitList.Add(unit);
		}
	}
	
	// Update is called once per frame
	void Update () {
		CheckSelection();
	}
	/// <summary>
	/// the unit hovered in last frame(update every frame)
	/// </summary>
	VideoInfoUnit lastHoverUnit;
	/// <summary>
	/// check if the user see any unit
	/// </summary>
	void CheckSelection()
	{
		CardboardHead head = Cardboard.Controller.Head;
		RaycastHit hit;
		VideoInfoUnit hoverUnit = null;
		UIHoverEvent hoverEvent = new UIHoverEvent();

		if ( Physics.Raycast(head.Gaze, out hit, Mathf.Infinity) )
		{
			Debug.Log("Hit " + hoverUnit.name );
			hoverUnit = hit.collider.GetComponent<VideoInfoUnit>();
			hoverEvent.point = hit.point;
		}
			
		if ( hoverUnit != null ){
			if ( lastHoverUnit == null )
				hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.Begin;
			else
				hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.Middle;
			
			hoverUnit.OnHover(hoverEvent);

		}
		else if ( lastHoverUnit != null ) {
			hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.End;
			lastHoverUnit.OnHover(hoverEvent);
		}

		lastHoverUnit = hoverUnit;

	}
}