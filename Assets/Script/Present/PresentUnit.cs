using UnityEngine;
using System.Collections;

public class PresentUnit : MonoBehaviour {

	[SerializeField]SphereCollider col;

	void Start()
	{
		if ( col == null )
			col = GetComponentInChildren<SphereCollider>();

		if ( col != null )
		{
			PresentSensor sensor = col.gameObject.AddComponent<PresentSensor>();
			sensor.Init( this );
		}
	}

	public void OnFingerDown( FingerDownEvent e ) {
		
	}

	public void OnFingerUp( FingerUpEvent e ) {

	}

	public void OnFingerMove( FingerMotionEvent e ) {
		if ( e.Phase == FingerMotionPhase.Started )
		{
			col.radius *= 3f;
			CardboardHead.Lock();
		}
		else if ( e.Phase == FingerMotionPhase.Updated )
		{
			Vector3 screenPos = Camera.main.WorldToScreenPoint( transform.position );
			Vector3 newScreenPos = screenPos + new Vector3( e.Finger.DeltaPosition.x , e.Finger.DeltaPosition.y , 0 );
			Vector3 toward = Camera.main.ScreenPointToRay( newScreenPos ).direction;

			float distance = ( transform.position - Camera.main.transform.position ).magnitude;
			transform.position = Camera.main.transform.position + distance * toward;
		}
		else if ( e.Phase == FingerMotionPhase.Ended )
		{
			col.radius /= 3f;
			CardboardHead.UnLock();
		}
	}
}
