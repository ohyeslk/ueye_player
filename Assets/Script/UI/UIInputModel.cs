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
		{
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
