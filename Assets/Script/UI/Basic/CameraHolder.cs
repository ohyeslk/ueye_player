using UnityEngine;
using System.Collections;

public class CameraHolder : MonoBehaviour {

	[SerializeField] GameObject selectionWindow;
	[SerializeField] CardboardHead head;

	void OnDisable()
	{
		VREvents.SwitchVRMode -= OnSwitchVRMode;
	}

	void OnEnable()
	{
		VREvents.SwitchVRMode += OnSwitchVRMode;
	}

	void OnSwitchVRMode( Message msg )
	{
		VRMode to = (VRMode)msg.GetMessage( Global.MSG_SWITCHVRMODE_MODE_KEY );

		if ( head == null )
		{
			head = FindObjectOfType(typeof(CardboardHead) ) as CardboardHead;
		}

		if ( to == VRMode.VR_2D )
		{
			head.isLockVertical = true;
		}else if ( to == VRMode.VR_3D )
		{
			head.isLockVertical = false;
		}
	}
		
	void Start()
	{
		RotateTowardSelection();

	}

	void RotateTowardSelection( )
	{
		if ( selectionWindow == null )
			selectionWindow = GameObject.FindGameObjectWithTag( "SelectionWindow" );
		if ( selectionWindow != null )
			transform.LookAt( selectionWindow.transform.position );
	}

	void OnFingerMove( FingerMotionEvent e ) {
		
		Vector3 from = Camera.main.ScreenPointToRay( e.Position - e.Finger.DeltaPosition ).direction;
		Vector3 to = Camera.main.ScreenPointToRay( e.Position ).direction;

		from.y = to.y = 0;
		transform.rotation *= Quaternion.FromToRotation( from , to );
	}

}
