using UnityEngine;
using System.Collections;

public class CameraHolder : MonoBehaviour {

	[SerializeField] GameObject selectionWindow;

	void Awake()
	{
		RotateTowardSelection();
	}

	void RotateTowardSelection( )
	{
		if ( selectionWindow == null )
			selectionWindow = GameObject.FindGameObjectWithTag( "SelectionWindow" );
		
		transform.LookAt( selectionWindow.transform.position );

	}

	void OnFingerMove( FingerMotionEvent e ) {
		
		Vector3 from = Camera.main.ScreenPointToRay( e.Position - e.Finger.DeltaPosition ).direction;
		Vector3 to = Camera.main.ScreenPointToRay( e.Position ).direction;

		from.y = to.y = 0;
		transform.rotation *= Quaternion.FromToRotation( from , to );
	}

}
