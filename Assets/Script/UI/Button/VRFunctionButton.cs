using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class VRFunctionButton : VRBasicButton {

	[SerializeField] UnityEvent confirmFunc;

	public override void OnConfirm ()
	{
		base.OnConfirm();
		confirmFunc.Invoke();
	}
}
