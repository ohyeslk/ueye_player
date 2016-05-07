using UnityEngine;
using System.Collections;

public class UISensor : MonoBehaviour {

	[SerializeField] protected float focusTime;
	[SerializeField] protected float confirmTime;

	TimeBaseActionState inner_time_state = TimeBaseActionState.None;
	TimeBaseActionState m_time_state {
		get { return inner_time_state ; }
		set {
			if ( inner_time_state != value )
			{
				if ( value == TimeBaseActionState.Focused )
				{
					OnFocus();
				}else if ( value == TimeBaseActionState.Confirmed )
				{
					OnConfirm();
				}
			}

			inner_time_state = value;
		}
	}
	TimeBaseActionState TimeState{ get { return m_time_state;}}


	float startHoverTime = Mathf.Infinity;
	public float HoveredTime
	{
		get {
			return (Time.time > startHoverTime)? Time.time - startHoverTime : 0;
		}
	}

	virtual public void OnFocus()
	{
		var varg = new UISensorArg(this);
		varg.focusTime = focusTime;
		varg.confirmTime = confirmTime;
		VREvents.FireUIFocus(varg);
	}

	virtual public void OnConfirm()
	{
		var varg = new UISensorArg(this);
		varg.focusTime = focusTime;
		varg.confirmTime = confirmTime;
		VREvents.FireUIConfirm(varg);
	}

	virtual public void OnHover( UIHoverEvent e ) {
		UpdateStartTime(e);
		UpdateTimeRelatedAction(e);
	}

	protected void UpdateStartTime(UIHoverEvent e)
	{
		if ( e.hoverPhase == UIHoverEvent.HoverPhase.Begin )
			startHoverTime = Time.time;
		else if ( e.hoverPhase == UIHoverEvent.HoverPhase.End )
			startHoverTime = Mathf.Infinity;
	}

	protected void UpdateTimeRelatedAction(UIHoverEvent e)
	{
		if ( e.hoverPhase == UIHoverEvent.HoverPhase.Middle )
		{
			if ( HoveredTime < focusTime )
			{
				m_time_state = TimeBaseActionState.Hovered;
			}
			else if ( HoveredTime < focusTime + confirmTime )
			{
				m_time_state = TimeBaseActionState.Focused;
			}
			else
			{
				m_time_state = TimeBaseActionState.Confirmed;
			}
		}
		else if ( e.hoverPhase == UIHoverEvent.HoverPhase.End )
		{
			m_time_state = TimeBaseActionState.None;
		}
	}

}


public enum TimeBaseActionState
{
	None,
	Hovered,
	Focused,
	Confirmed,
}
