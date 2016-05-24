using UnityEngine;
using System.Collections;

public class UIWindow : MonoBehaviour {
	[SerializeField] WindowArg.Type myType;

	virtual protected void OnDisable()
	{
		VREvents.ActiveWindow -= OnActiveWindow;
	}

	virtual protected void OnEnable()
	{
		VREvents.ActiveWindow += OnActiveWindow;
	}

	protected void OnActiveWindow( WindowArg arg )
	{
		if ( arg.type == myType )
		{
			OnBecomeVisible();
		}else
		{
			OnBecomeInvsible();
		}

	}

	virtual protected void OnBecomeVisible(){}
	virtual protected void OnBecomeInvsible(){}

}
