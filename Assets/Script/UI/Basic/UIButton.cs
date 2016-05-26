using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class UIButton :UISensor {

	[SerializeField] UnityEvent focusEvent;
	[SerializeField] UnityEvent confirmEvent;

	override public void OnFocus()
	{
		base.OnFocus();

		if ( focusEvent != null )
			focusEvent.Invoke();
	}

	override public void OnConfirm()
	{
		base.OnFocus();

		if ( confirmEvent != null )
			confirmEvent.Invoke();
	}


}
