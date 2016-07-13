using UnityEngine;
using System.Collections;

public class PresentUnit : MonoBehaviour {

	[SerializeField] PresentEffect effect;

	public enum PresentState
	{
		Normal,
		Attached,
	}
	public PresentState state = PresentState.Normal;

	void Start()
	{
		Collider childeCol = GetComponentInChildren<Collider>();
		if ( childeCol != null )
		{
			PresentSensor sensor =  childeCol.gameObject.AddComponent<PresentSensor>();
			sensor.Init( this );
		}
	}

	public void OnFingerDown( FingerDownEvent e )
	{
//		Debug.Log("Finger Down " + e.Selection.name );
		Attach();
	}

	void Attach()
	{
		if ( effect != null )
			effect.EmitTouchFeedbackParticle();

		state = PresentState.Attached;


	}

	public void OnFingerMove( FingerMotionEvent e )
	{
		if ( e.Selection != null )
		{
			Vector3 deltaPos = e.Finger.DeltaPosition;

			Vector3 myScreenPos = Camera.main.WorldToScreenPoint( transform.position );

			Vector3 newScreenPos = myScreenPos + deltaPos;

			transform.position = Camera.main.ScreenToWorldPoint( newScreenPos );
		}
	}

	public void OnFingerUp( FingerUpEvent e )
	{
		DisAttach();
	}

	void DisAttach()
	{
		if ( effect != null )
			effect.StopTouchFeedbackParticle();

		state = PresentState.Normal;
	}


	public void OnFingerHover( FingerHoverEvent e )
	{
		
	}
}
