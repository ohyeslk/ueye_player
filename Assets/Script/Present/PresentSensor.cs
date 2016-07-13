using UnityEngine;
using System.Collections;

public class PresentSensor : MonoBehaviour {

	PresentUnit parent;

	public void Init ( PresentUnit _p )
	{
		parent = _p;	
	}

	void OnFingerDown( FingerDownEvent e ) {
		parent.OnFingerDown( e );	
	}

	void OnFingerUp( FingerUpEvent e ) {
		parent.OnFingerUp( e );
	}

	void OnFingerMove( FingerMotionEvent e ) {
		parent.OnFingerMove( e );
	}
}
