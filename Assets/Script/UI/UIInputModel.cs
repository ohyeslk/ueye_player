using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class UIInputModel : GazeInputModule {

	public override void Process ()
	{
		base.Process ();
		ProcessUI();
	}

	protected UISensor lastSensor = null;
	float hoverDuration = 0;
	public void ProcessUI()
	{
		// deal with the hover 
			UIHoverEvent hoverEvent = new UIHoverEvent();
			hoverEvent.point = GetIntersectionPosition();
			UISensor hoverSensor = null ;

			GameObject currentObj = GetCurrentGameObject();
			if  ( currentObj != null )
				hoverSensor = currentObj.GetComponent<UISensor>();


			if ( hoverSensor != null ) {
				if ( lastSensor != hoverSensor ) {
					hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.Begin;
					hoverDuration = 0;
				}
				else
				{
					hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.Middle;
					hoverDuration += Time.deltaTime;
				}
				hoverEvent.duration = hoverDuration;
				hoverSensor.OnHover(hoverEvent);
			}
			if ( lastSensor != null )
			{
				if ( lastSensor != hoverSensor )
				{
					hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.End;
					lastSensor.OnHover(hoverEvent);
				}
			}

			lastSensor = hoverSensor;
	}


	public GameObject m_fingerTargetObject;

	PointerEventData FingerPoint;

	public void OnFingerDown(FingerDownEvent e )
	{
		if ( FingerPoint == null ) {
			FingerPoint = new PointerEventData(eventSystem);
		}

		FingerPoint.Reset();
		FingerPoint.position = e.Position;
		eventSystem.RaycastAll(FingerPoint, m_RaycastResultCache);
		FingerPoint.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
		m_fingerTargetObject = FingerPoint.pointerCurrentRaycast.gameObject;
		m_RaycastResultCache.Clear();

		ExecuteEvents.Execute (m_fingerTargetObject, new PointerEventData (eventSystem), ExecuteEvents.pointerClickHandler);

	}

}

[System.Serializable]
public class UIEvent
{
	
}

[System.Serializable]
public class UIHoverEvent : UIEvent
{

	public enum HoverPhase
	{
		Begin,
		Middle,
		End,
	}
	public HoverPhase hoverPhase;

	public Vector3 point;

	public float duration;
}
