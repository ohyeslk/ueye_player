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


	public float FocusTime
	{
		get {
			return ( HoveredTime > focusTime )? HoveredTime - focusTime : 0;
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
		if ( CheckStartTime(e) )
			UpdateState();
	}

	virtual public void OnMotion( FingerMotionEvent e ) {
		
	}

	/// <summary>
	/// Checks the start time.
	/// </summary>
	/// <returns><c>true</c>, if in the middle of the hover, <c>false</c> otherwise.</returns>
	/// <param name="e">E.</param>
	protected bool CheckStartTime(UIHoverEvent e)
	{
		if ( e.hoverPhase == UIHoverEvent.HoverPhase.Middle )
			return true;
		
		if ( e.hoverPhase == UIHoverEvent.HoverPhase.Begin )
			StartHover();
		else if ( e.hoverPhase == UIHoverEvent.HoverPhase.End )
			EndHover();
		
		return false;
	}

	protected void StartHover()
	{
		startHoverTime = Time.time;
	}

	protected void EndHover()
	{
		startHoverTime = Mathf.Infinity;
		m_time_state = TimeBaseActionState.None;
	}
		
	protected void UpdateState()
	{
		if ( FocusTime <= 0 )
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

	public void Reset()
	{
		startHoverTime = Mathf.Infinity;
		m_time_state = TimeBaseActionState.None;
	}

	public float GetTotalFocusTime () { return focusTime ; }
	public float GetTotalConfirmTime () { return confirmTime ; }
		
}


public enum TimeBaseActionState
{
	None,
	Hovered,
	Focused,
	Confirmed,
}
