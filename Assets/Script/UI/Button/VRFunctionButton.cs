using UnityEngine;
using System.Collections;

public class VRFunctionButton : VRBasicButton {

	public enum FunctionType
	{
		Back,
	}
	[SerializeField] FunctionType function;

	public override void OnConfirm ()
	{
		switch ( function )
		{
		case FunctionType.Back:
			break;
		default:
			break;
		};
	}
}
