using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VRFloatButton : VRFunctionButton {

	[SerializeField] Image flowImage;
	[SerializeField] Animator flowImageAnimator;

	protected float flowProcess;

	Coroutine fadeOut;
	protected bool isSelected = false;

	void Start()
	{
		if ( flowImageAnimator != null )
		{
			flowImageAnimator.speed = 0f;
			flowImageAnimator.Play("CurveAnimation" , 0 , 0);
		}
	}

	public override void OnConfirm ()
	{
		base.OnConfirm ();
		isSelected = true;
	}

	public override void OnFocus ()
	{
		base.OnFocus ();
		if ( fadeOut != null )	
			StopCoroutine(fadeOut );
	}

	public override void UpdateHover (float process)
	{
		base.UpdateHover (process);
		flowProcess = subButtonAnimation.confirmCurve.Evaluate( process );

		UpdateByProcess();
	}

	public override void OnExitHover ()
	{
		base.OnExitHover ();
		if ( !isSelected )
		{
			FlowFadeOut();
		}
	}

	public override void OnExitSub ()
	{
		base.OnExitSub ();
		if ( !isSelected )
		{
			FlowFadeOut();
		}
	}

	protected virtual void UpdateByProcess()
	{
		if ( flowImageAnimator != null )
		{
			flowImageAnimator.Play("CurveAnimation" , 0 , flowProcess );
		}
	}

	public void FlowFadeOut()
	{
		if ( fadeOut == null )
			fadeOut = StartCoroutine(FlowFadeOutDo(subButtonAnimation.hideTime));
	}

	IEnumerator FlowFadeOutDo( float totalTime )
	{
		while( true )
		{
			UpdateByProcess();

			flowProcess -= Time.deltaTime / totalTime;

			if ( flowProcess < 0 )
				break;
			
			yield return null;
		}
		flowProcess = 0 ;

		UpdateByProcess();

		fadeOut = null;
	}

	public override void OnBecomeInvisible (float time)
	{
		base.OnBecomeInvisible (time);

		if ( flowImage != null )
		{
			StartCoroutine( FlowFadeOutDo( time ));
		}
	}

	public override void OnBecomeVisible (float time)
	{
		base.OnBecomeVisible (time);

		if ( flowImage != null )
		{
			StartCoroutine( FlowFadeOutDo( 0 ) );
		}
	}

}
