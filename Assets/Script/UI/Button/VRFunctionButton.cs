using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class VRFunctionButton : VRBasicButton {

	[SerializeField] UnityEvent confirmFunc;

	public override void OnConfirm ()
	{
		base.OnConfirm();
<<<<<<< HEAD
		if ( m_Enable )
		{
			confirmFunc.Invoke();
		}
=======
		confirmFunc.Invoke();
>>>>>>> 223bc20077c7b09e94d589709d9b76fe007d3996
	}
}
