using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CategoryWindow : UIWindow {
	[SerializeField] GameObject CategoryButtonPrefab;
	[SerializeField] GridLayoutGroup Panel;
	List<CategoryButton> cateButtons = new List<CategoryButton>();
<<<<<<< HEAD
	public int column;
	public int row;
=======
>>>>>>> 223bc20077c7b09e94d589709d9b76fe007d3996

	override protected void OnDisable()
	{
		base.OnDisable();
		VREvents.PostCategory -= GetCategory;
	}

	override protected void OnEnable()
	{
		base.OnEnable();
		VREvents.PostCategory += GetCategory;
	}

	void Awake()
	{
		if ( CategoryButtonPrefab == null )
			CategoryButtonPrefab = (GameObject)Resources.Load("Prefab/UI/CategoryButton");
		if ( Panel == null )
			Debug.LogError("Cannot find Panel");
	}

	void Start() {
		RequestCategory();
	}

	public void GetCategory( Message msg )
	{
		List<CategoryInfo> cateList = (List<CategoryInfo>) msg.GetMessage( Global.MSG_POSTCATEGORY_CATEGORYLIST_KEY );

		foreach( CategoryInfo info in cateList )
		{
			CreateCategoryButton( info );
		}
	}
	public void RequestCategory()
	{
		URLRequestMessage msg = new URLRequestMessage(this);
		VREvents.FireRequestCategory(msg);
	}

	void CreateCategoryButton( CategoryInfo info )
	{
		if ( CategoryButtonPrefab != null )
		{
			GameObject unitObj = Instantiate( CategoryButtonPrefab ) as GameObject;
			unitObj.transform.SetParent( Panel.transform  );
			unitObj.transform.localScale = Vector3.one;
			unitObj.transform.localPosition = Vector3.zero;
			unitObj.transform.localRotation = Quaternion.identity;
<<<<<<< HEAD
			unitObj.name = "CategoryBtn" + info.name;
=======
			unitObj.name = "CateB" + info.name;
>>>>>>> 223bc20077c7b09e94d589709d9b76fe007d3996

			CategoryButton btn = unitObj.GetComponentInChildren<CategoryButton>();

			// initilze the unit
<<<<<<< HEAD
			btn.Init( info  , cateButtons.Count , this );
=======
			btn.Init( info  , this );
>>>>>>> 223bc20077c7b09e94d589709d9b76fe007d3996

			// save the unit in the listx
			cateButtons.Add(btn);
		}
	}

<<<<<<< HEAD
	protected override void OnBecomeInvsible ( float time )
	{
		base.OnBecomeInvsible ( time );
		Panel.gameObject.SetActive(false);
	}

	protected override void OnBecomeVisible ( float time )
	{
		base.OnBecomeVisible ( time 
		);
=======
	protected override void OnBecomeInvsible ()
	{
		base.OnBecomeInvsible ();
		Panel.gameObject.SetActive(false);
	}

	protected override void OnBecomeVisible ()
	{
		base.OnBecomeVisible ();
>>>>>>> 223bc20077c7b09e94d589709d9b76fe007d3996
		Panel.gameObject.SetActive(true);
	}

}
