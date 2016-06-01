﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class VRBasicButton : MonoBehaviour {
	[SerializeField] public SubAnimation subButtonAnimation;

	bool inner_enable = true;
	public bool m_Enable{
		get {
			return inner_enable;
		}
		set {
			inner_enable = value;
			if ( subButtonAnimation.subButton != null )
				subButtonAnimation.subButton.raycastTarget = inner_enable;
		}
	}

	virtual public void OnFucus( )
	{
		if ( m_Enable )
		{
		}
	}

	virtual public void OnConfirm ()
	{
		if ( m_Enable)
		{
		if ( subButtonAnimation.subButtonRing != null )
		{
			subButtonAnimation.subButtonRing.transform.DOScale( subButtonAnimation.subRingScaleUp , subButtonAnimation.subRingScaleUpTime );
			subButtonAnimation.subButtonRing.DOFade( 0 , subButtonAnimation.subRingScaleUpTime );
		}
		if ( subButtonAnimation.subButton != null )
		{
			if ( subButtonAnimation.subButtonFade )
			{
					subButtonAnimation.subButton.DOKill();	
				subButtonAnimation.subButton.DOFade( 0 , subButtonAnimation.subRingScaleUpTime );
			}
		}
		}
	}

	void LateUpdate()
	{
		if ( subButtonAnimation.subButton.color.a < 0.02f )
			subButtonAnimation.subButton.enabled = false;
		else
			subButtonAnimation.subButton.enabled = true;

		if ( subButtonAnimation.subButtonRing.color.a < 0.02f || subButtonAnimation.subButtonRing.fillAmount < 0.02f )
			subButtonAnimation.subButtonRing.enabled = false;
		else
			subButtonAnimation.subButtonRing.enabled = true;
	}


	/// <summary>
	/// Show the confirm button 
	/// </summary>
	virtual public void OnEnterHover()
	{
		if ( m_Enable )
		{
			float time = subButtonAnimation.showTime;
			if ( subButtonAnimation.subButton != null )
			{
				if ( subButtonAnimation.subButtonFade )
				{
					subButtonAnimation.subButton.DOKill();
					subButtonAnimation.subButton.enabled = true;
					subButtonAnimation.subButton.DOFade( 1f , time );
				}
				if ( subButtonAnimation.subButtonMove )
				{
					subButtonAnimation.subButton.transform.DOKill();
					subButtonAnimation.subButton.transform.DOLocalMoveY( subButtonAnimation.posY + subButtonAnimation.moveY , 0 );
					subButtonAnimation.subButton.transform.DOLocalMoveY( subButtonAnimation.posY , time );
				}
			}

			if ( subButtonAnimation.subButtonRing != null )
			{
				subButtonAnimation.subButtonRing.DOKill();
				subButtonAnimation.subButtonRing.enabled = true;
				subButtonAnimation.subButtonRing.DOFade( 1f , 0 );
				subButtonAnimation.subButtonRing.transform.localScale = Vector3.one;
			}
		}

	}

	/// <summary>
	/// Hide the confirm button
	/// </summary>
	virtual public void OnExitHover()
	{
		if ( m_Enable )
		{
		float time = subButtonAnimation.hideTime;
		if ( subButtonAnimation.subButton != null )
		{
			if ( subButtonAnimation.subButtonFade)
			{
				subButtonAnimation.subButton.DOKill();
				subButtonAnimation.subButton.DOFade( 0 , time  );
			}
			if ( subButtonAnimation.subButtonMove )
			{
				subButtonAnimation.subButton.transform.DOKill();
				subButtonAnimation.subButton.transform.DOLocalMoveY( subButtonAnimation.posY + subButtonAnimation.moveY , time );
			}
		}
		if ( subButtonAnimation.subButtonRing != null )
		{
			subButtonAnimation.subButtonRing.DOKill();
			subButtonAnimation.subButtonRing.DOFillAmount( 0 , time  * subButtonAnimation.subButtonRing.fillAmount ).OnComplete(ResetSubButton);
				subButtonAnimation.subButtonRing.DOFade( 0 , time  * subButtonAnimation.subButtonRing.fillAmount);
		}
		}
	}


	virtual public void OnEnterSub()
	{
		if ( m_Enable )
		{
			float time = subButtonAnimation.showTime;
			if ( subButtonAnimation.subButton != null )
			{
				if ( subButtonAnimation.subButtonFade )
				{
					subButtonAnimation.subButton.DOKill();
					subButtonAnimation.subButton.enabled = true;
					subButtonAnimation.subButton.DOFade( 1f , time );
				}
			}
		}
	}

	virtual public void OnExitSub()
	{
		float time = subButtonAnimation.hideTime;
		if ( subButtonAnimation.subButtonRing != null )
		{
			Color col = subButtonAnimation.subButtonRing.color ; 
			col.a = 1f ; 
			subButtonAnimation.subButtonRing.color = col;
			subButtonAnimation.subButtonRing.transform.localScale = Vector3.one;

			subButtonAnimation.subButtonRing.DOKill();
			subButtonAnimation.subButtonRing.DOFillAmount( 0 , time );
			subButtonAnimation.subButtonRing.DOFade( 0 , time );

		}
	}
		
	virtual public void UpdateHover( float process )
	{
		if ( m_Enable )
		{
		if ( subButtonAnimation.subButtonRing != null )
		{
			subButtonAnimation.subButtonRing.fillAmount = subButtonAnimation.confirmCurve.Evaluate( process );
		}
		}
	}

	protected void ResetSubButton()
	{
		if ( subButtonAnimation.subButton != null )
		{
			if ( subButtonAnimation.subButtonFade )
			{
				Color col = subButtonAnimation.subButton.color;
				col.a = 0.00f;
				subButtonAnimation.subButton.color = col;
			}
		}
		if ( subButtonAnimation.subButtonRing )
		{
			subButtonAnimation.subButtonRing.fillAmount = 0;
			subButtonAnimation.subButtonRing.enabled = false;
		}
	}

	public void OnBecomeVisible( float time , bool setEnableTo)
	{
		gameObject.SetActive( true );
		m_Enable = setEnableTo;
		OnBecomeVisible( time );
	}

	virtual public void OnBecomeVisible( float time )
	{
		if ( subButtonAnimation.subButton != null )
		{
			float t = ( time <= 0 ) ? subButtonAnimation.showTime : time ;
			subButtonAnimation.subButton.DOKill();
			subButtonAnimation.subButton.DOFade( 1f , t );
		}
	}

	public void OnBecomeInvisible( float time , bool setEnableTo )
	{
		m_Enable = setEnableTo ;
		if ( subButtonAnimation.subButton != null )
		{
			float t = ( time <= 0 ) ? subButtonAnimation.hideTime : time;
			subButtonAnimation.subButton.DOKill();
			subButtonAnimation.subButton.DOFade( 0 , t ).OnComplete( SelfDisable );
		}
	}

	virtual public void OnBecomeInvisible( float time )
	{
		if ( subButtonAnimation.subButton != null )
		{
			float t = ( time <= 0 ) ? subButtonAnimation.hideTime : time;
			subButtonAnimation.subButton.DOKill();
			subButtonAnimation.subButton.DOFade( 0 , t );
		}
	}

	void SelfDisable()
	{
		gameObject.SetActive( false );
	}

}

[System.Serializable]
public struct SubAnimation
{
	public Image subButton;
	public Image subButtonRing;
	public AnimationCurve confirmCurve;
	public float showTime;
	public float hideTime;
	public float posY;
	public float moveY;
	public float subRingScaleUp;
	public float subRingScaleUpTime;
	public bool subButtonFade;
	public bool subButtonMove;
}