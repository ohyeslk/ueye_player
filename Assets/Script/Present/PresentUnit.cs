﻿using UnityEngine;
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
		}
		else if ( e.Phase == FingerMotionPhase.Ended )
		{
			Vector3 screenPos = Camera.main.WorldToScreenPoint( transform.position );
			Vector3 newScreenPos = screenPos + new Vector3( e.Position.x , e.Position.y , 0 );

			float distance = ( transform.position - Camera.main.transform.position ).magnitude;
		}
		else if ( e.Phase == FingerMotionPhase.Updated )
		{
			col.radius /= 3f;
		}
	}
}
