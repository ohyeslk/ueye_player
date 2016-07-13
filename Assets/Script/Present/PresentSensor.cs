using UnityEngine;
using System.Collections;

public class PresentSensor : MonoBehaviour {

	PresentUnit m_PresentUnit;

	public void Init( PresentUnit parent_unit )
	{
		m_PresentUnit = parent_unit;
	}

	void OnFingerDown( FingerDownEvent e )
	{
		Debug.Log("On Finger Down");
		m_PresentUnit.OnFingerDown( e );
	}

	void OnFingerMove( FingerMotionEvent e )
	{
		m_PresentUnit.OnFingerMove( e );
	}

	void OnFingerUp( FingerUpEvent e )
	{
		m_PresentUnit.OnFingerUp( e );
	}

	void OnFingerHover( FingerHoverEvent e )
	{
		m_PresentUnit.OnFingerHover( e );
	}
}
