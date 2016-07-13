using UnityEngine;
using System.Collections;

public class CameraHolder : MonoBehaviour {

	[SerializeField] GameObject selectionWindow;
	[SerializeField] Cardboard cardboard;
	[SerializeField] CardboardHead head;

	void OnDisable()
	{
		VREvents.SwitchVRMode -= OnSwitchVRMode;
		VREvents.ActiveWindow -= OnActiveWindow;
	}

	void OnEnable()
	{
		VREvents.SwitchVRMode += OnSwitchVRMode;
		VREvents.ActiveWindow += OnActiveWindow;
	}

	void OnActiveWindow (WindowArg arg)
	{
		if ( LogicManager.VRMode == VRMode.VR_2D && arg.type != WindowArg.Type.PLAY_WINDOW )
		{
			head.isLockVertical = true;
		}else
		{
			head.isLockVertical = false;
		}
	}

	void OnSwitchVRMode( Message msg )
	{
		VRMode to = (VRMode)msg.GetMessage( Global.MSG_SWITCHVRMODE_MODE_KEY );

		if ( head == null )
		{
			head = FindObjectOfType(typeof(CardboardHead) ) as CardboardHead;
		}

		if ( to == VRMode.VR_2D && LogicManager.ActiveWindow != WindowArg.Type.PLAY_WINDOW )
		{
			head.isLockVertical = true;

		}else
		{
			head.isLockVertical = false;
		}
	}
		
	void Start()
	{
		RotateTowardSelection();
	}
		

	void Update()
	{
		if ( Input.GetKeyDown( KeyCode.R ) )
		{
			Debug.Log(" Press R ");
			RotateTowardSelection();
		}
	}

	void RotateTowardSelection( )
	{
		if ( selectionWindow == null )
			selectionWindow = GameObject.FindGameObjectWithTag( "SelectionWindow" );
		if ( selectionWindow != null )
			transform.LookAt( selectionWindow.transform.position );
	}

	void OnFingerDown( FingerDownEvent e ) {
		Debug.Log("Finger Down " + e.Finger );
	}

	void OnFingerUp( FingerUpEvent e ) {
		Debug.Log("Finger Up " + e.Finger );
	}

	void OnFingerMove( FingerMotionEvent e ) {
		if ( e.Phase == FingerMotionPhase.Started )
		{
			Debug.Log( "Start ");
		}

		if ( e.Phase == FingerMotionPhase.Ended )
		{
			Debug.Log("End ");
		}

		if ( e.Phase == FingerMotionPhase.Updated )
		{
			Debug.Log("Finger Move");
			Vector2 delta = e.Finger.DeltaPosition;
	
			head.UpdateHead( delta );
		}


//		Vector3 to = Camera.main.ScreenPointToRay( e.Position - e.Finger.DeltaPosition ).direction;
//		Vector3 from = Camera.main.ScreenPointToRay( e.Position ).direction;
//
//		if ( LogicManager.isLockVerticle )
//		{
//			from.y = to.y = 0;
//		}
//		transform.rotation *= Quaternion.FromToRotation( from , to );
	}

}
