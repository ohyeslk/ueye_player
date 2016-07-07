using UnityEngine;
using System.Collections;

public class GroupButton : VRFloatButton {

	[SerializeField] Group group;
	const string MSG_GROUP = "group";

	public void OnEnable()
	{
		VREvents.GroupButtonConfirm += OnGroupButtonConfirm;
	}

	void OnGroupButtonConfirm (Message msg)
	{
		Group group = (Group)msg.GetMessage( MSG_GROUP );
	}

	public void OnDisable()
	{
		VREvents.GroupButtonConfirm -= OnGroupButtonConfirm;
	}

	public override void OnConfirm ()
	{
		
	}

}

public enum Group
{
	TOP_BUTTON,
	CATEGORY_BUTTON,
}