using UnityEngine;
using System.Collections;

public class ReturnButton :UISensor {
	override public void OnFocus()
	{
		var varg = new UISensorArg(this);
		varg.focusTime = focusTime;
		varg.confirmTime = confirmTime;
		varg.type = UISensorArg.SensorType.Return;
		VREvents.FireUIFocus(varg);
	}

	override public void OnConfirm()
	{
		var varg = new UISensorArg(this);
		varg.focusTime = focusTime;
		varg.confirmTime = confirmTime;
		varg.type = UISensorArg.SensorType.Return;
		VREvents.FireUIConfirm(varg);
	}
}
