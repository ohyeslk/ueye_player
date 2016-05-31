using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UIInputModel : GazeInputModule {

	public override void Process ()
	{
		base.Process ();
		ProcessUI();
	}

	protected UISensor lastSensor = null;
	protected UISensor lastSubSensor = null;
	float hoverDuration = 0;
	float subHoverDuration = 0;
	public void ProcessUI()
	{
		// deal with the hover 
		UIHoverEvent hoverEvent = new UIHoverEvent();
		hoverEvent.point = GetIntersectionPosition();
		UISensor hoverSensor = null ;

		GameObject currentObj = GetCurrentGameObject();
		if  ( currentObj != null )
			hoverSensor = currentObj.GetComponent<UISensor>();

		if ( hoverSensor == null )
		{
			if ( lastSensor != null )
			{
				hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.End;
				hoverEvent.duration = hoverDuration;
				lastSensor.OnHover(hoverEvent);
				hoverDuration = 0;
				lastSensor = null;
			}
			if ( lastSubSensor != null )
			{
				hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.End;
				hoverEvent.duration = subHoverDuration;
				lastSubSensor.OnHover(hoverEvent);
				subHoverDuration = 0;
				lastSubSensor = null;
			}
		}
		else if ( hoverSensor.GetSensorType() == SensorType.Normal )
		{
			if ( lastSensor != hoverSensor )
			{
				if ( lastSensor != null )
				{
					hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.End;
					hoverEvent.duration = hoverDuration;
					lastSensor.OnHover( hoverEvent );
				}

				hoverDuration = 0;
				hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.Begin;
				hoverEvent.duration = hoverDuration;
				hoverSensor.OnHover( hoverEvent );
				lastSensor = hoverSensor;
			}
			else
			{
				hoverDuration += Time.deltaTime;

				hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.Middle;
				hoverEvent.duration = hoverDuration;
				hoverSensor.OnHover( hoverEvent );
			}

			// sub exit 
			if ( lastSubSensor != null )
			{
				hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.End;
				hoverEvent.duration = subHoverDuration;
				lastSubSensor.OnHover(hoverEvent);
				subHoverDuration = 0;
				lastSubSensor = null;
			}
		}
		else if ( hoverSensor.GetSensorType () == SensorType.Sub )
		{
			if ( lastSensor != null )
			{
				hoverDuration += Time.deltaTime;
				hoverEvent.duration = hoverDuration;
				hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.Middle;
				lastSensor.OnHover( hoverEvent );
			}

			if ( lastSubSensor != hoverSensor )
			{
				if ( lastSubSensor != null )
				{
					hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.End;
					hoverEvent.duration = subHoverDuration;
					lastSensor.OnHover( hoverEvent );
				}

				subHoverDuration = 0;
				hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.Begin;
				hoverEvent.duration = subHoverDuration;
				hoverSensor.OnHover( hoverEvent );
				lastSubSensor = hoverSensor;
			}
			else
			{
				subHoverDuration += Time.deltaTime;

				hoverEvent.hoverPhase = UIHoverEvent.HoverPhase.Middle;
				hoverEvent.duration = subHoverDuration;
				hoverSensor.OnHover( hoverEvent );
			}
		}
	}

	protected override GameObject GetCurrentGameObject ()
	{
		if ( LogicManager.VRMode == VRMode.VR_3D )
			return base.GetCurrentGameObject();
		else
		{
			if (FingerPointData != null && FingerPointData.enterEventCamera != null) {
				return FingerPointData.pointerCurrentRaycast.gameObject;
			}

		}
		return null;
	}

	protected override Vector3 GetIntersectionPosition ()
	{
		if ( LogicManager.VRMode == VRMode.VR_3D ) 
			return base.GetIntersectionPosition ();
		else
		{
			if ( FingerPointData != null )
			{
				Camera cam = FingerPointData.enterEventCamera;
				if (cam == null) {
					return Vector3.zero;
				}

				float intersectionDistance = FingerPointData.pointerCurrentRaycast.distance + cam.nearClipPlane;
				Vector3 intersectionPosition = cam.transform.position + cam.transform.forward * intersectionDistance;

				return intersectionPosition;
			}
			return Vector3.zero;
		}
	}

	public GameObject m_fingerTargetObject;

	/// <summary>
	/// pointer event data used in 2d mode
	/// </summary>
	private PointerEventData FingerPointData;

	public void OnFingerDown(FingerDownEvent e )
	{
		{
			if ( FingerPointData == null ) {
				FingerPointData = new PointerEventData(eventSystem);
			}

//			Debug.Log( "On Down" + e.Name );

			List<RaycastResult> rayCastResults = new List<RaycastResult>();
			FingerPointData.Reset();
			FingerPointData.position = e.Position;
			eventSystem.RaycastAll(FingerPointData, rayCastResults);
			FingerPointData.pointerCurrentRaycast = FindFirstRaycast(rayCastResults);
			m_fingerTargetObject = FingerPointData.pointerCurrentRaycast.gameObject;
			rayCastResults.Clear();

			ExecuteEvents.Execute (m_fingerTargetObject, new PointerEventData (eventSystem), ExecuteEvents.pointerClickHandler);
		}
	}

	public void OnFingerMove( FingerMotionEvent e )
	{
		{
			if ( FingerPointData == null ) {
				FingerPointData = new PointerEventData(eventSystem);
			}
			FingerPointData.Reset();

			if ( e.Phase != FingerMotionPhase.Ended )
			{
				FingerPointData.position = e.Position;
				List<RaycastResult> rayCastResults = new List<RaycastResult>();
				eventSystem.RaycastAll( FingerPointData, rayCastResults );
				FingerPointData.pointerCurrentRaycast = FindFirstRaycast(rayCastResults);
				m_fingerTargetObject = FingerPointData.pointerCurrentRaycast.gameObject;
				rayCastResults.Clear();
			}else
			{
				FingerPointData = null;
			}

	//		ExecuteEvents.Execute (m_fingerTargetObject, new PointerEventData (eventSystem), ExecuteEvents.updateSelectedHandler);
		}
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
