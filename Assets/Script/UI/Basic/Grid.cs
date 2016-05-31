using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Grid : MonoBehaviour {

	[SerializeField] List<Transform> children;

	public GridInfo gridInfo;
	public struct GridInfo
	{
		public int row;
		public int column;
		public float width;
		public float height;
	}

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {
		if ( children == null )
			InitilizeChildren();
		
		UpdateChildrenPosition();
	}

	void InitilizeChildren() {
		if ( children == null )
		{
			children = new List<Transform>();
		}

		children.Clear();

		Transform[] trans = GetComponentsInChildren<Transform>();
		foreach( Transform t in trans )
		{
			if ( t.parent == transform )
			{
				children.Add(t);
			}
		}
	}

	void UpdateChildrenPosition() {
		
	}
}
