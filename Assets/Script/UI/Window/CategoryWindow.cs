using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CategoryWindow : UIWindow {
	[SerializeField] GameObject CategoryButtonPrefab;
	[SerializeField] GridLayoutGroup Panel;
	List<CategoryButton> cateButtons = new List<CategoryButton>();

	void OnDisable()
	{
		VREvents.PostCategory -= GetCategory;
	}

	void OnEnable()
	{
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
			unitObj.name = "CateB" + info.name;

			CategoryButton btn = unitObj.GetComponentInChildren<CategoryButton>();

			// initilze the unit
			btn.Init( info  , this );

			// save the unit in the listx
			cateButtons.Add(btn);
		}
	}


}
