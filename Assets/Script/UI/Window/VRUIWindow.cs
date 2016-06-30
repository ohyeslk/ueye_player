using UnityEngine;
using System.Collections;

public class VRUIWindow : MonoBehaviour {
	[SerializeField] WindowArg.Type myType;
	[SerializeField] float becomeVisibleTime = 1f;
	[SerializeField] float becomeInvisbleTime = 0.5f;

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
			OnBecomeVisible( becomeVisibleTime );
		}else
		{
			OnBecomeInvsible( becomeInvisbleTime );
		}

	}

	virtual protected void OnBecomeVisible( float time ){}
	virtual protected void OnBecomeInvsible( float time ){}

}
