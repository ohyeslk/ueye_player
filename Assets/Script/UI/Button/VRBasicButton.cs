using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class VRBasicButton : MonoBehaviour {
	[SerializeField] public SubAnimation subButtonAnimation;

	[SerializeField] Color normalColor = Color.white;
	[SerializeField] Color disableColor = Color.gray;

	bool inner_enable = true;
	public bool m_Enable{
		get {
			return inner_enable;
		}
		set {
			inner_enable = value;
			if ( subButtonAnimation.subButton != null )
				subButtonAnimation.subButton.raycastTarget = inner_enable;
			if ( inner_enable )
			{
				float alpha = subButtonAnimation.subButton.color.a;
				Color toCol = normalColor;
				toCol.a = alpha;
				subButtonAnimation.subButton.color = toCol;
			}
			else
			{
				float alpha = subButtonAnimation.subButton.color.a;
				Color toCol = disableColor;
				toCol.a = alpha;
				subButtonAnimation.subButton.color = toCol;
			}
		}
	}

	virtual public void OnFucus( )
	{
		if ( m_Enable )
		{
			if ( subButtonAnimation.FocusSound != null )
			{
				subButtonAnimation.FocusSound.Play();
			}
			if ( subButtonAnimation.UpdateHoverSound != null )
			{
				subButtonAnimation.UpdateHoverSound.Play();
			}
		}
	}

	virtual public void OnConfirm ()
	{
		if ( m_Enable)
		{
			if ( subButtonAnimation.subButtonRing != null )
			{
				subButtonAnimation.subButtonRing.transform.DOScale( subButtonAnimation.subRingScaleUp , subButtonAnimation.subRingScaleUpTime ).OnComplete(ResetSubButton);
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

			if ( subButtonAnimation.ConfirmSound != null )
			{
				subButtonAnimation.ConfirmSound.Play();
			}

			if ( subButtonAnimation.UpdateHoverSound != null )
			{
				subButtonAnimation.UpdateHoverSound.Stop();
			}

			VREvents.FireUIInputResetTarget(new Message(this));
		}
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

			if ( subButtonAnimation.EnterHoverSound != null )
			{
				subButtonAnimation.EnterHoverSound.Play();
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
			}

			if ( subButtonAnimation.ExitHoverSound != null )
			{
				subButtonAnimation.ExitHoverSound.Play();
			}

			if ( subButtonAnimation.UpdateHoverSound != null )
			{
				subButtonAnimation.UpdateHoverSound.DOPitch( 0.5f , time  * subButtonAnimation.subButtonRing.fillAmount );
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

			if ( subButtonAnimation.EnterSubSound != null )
			{
				subButtonAnimation.EnterSubSound.Play();
			}

			if ( subButtonAnimation.UpdateHoverSound != null )
			{
				subButtonAnimation.UpdateHoverSound.Play();
			}
		}
	}

	virtual public void OnExitSub()
	{
		if ( m_Enable )
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

			}

			if ( subButtonAnimation.ExitSubSound != null )
			{
				subButtonAnimation.ExitSubSound.Play();
			}

			if ( subButtonAnimation.UpdateHoverSound != null )
			{
				subButtonAnimation.UpdateHoverSound.DOPitch(0.5f,time);

			}
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

			if ( subButtonAnimation.UpdateHoverSound != null )
			{
				subButtonAnimation.UpdateHoverSound.pitch = 0.5f + process * 1f;
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
				col.a = 0.01f;
				subButtonAnimation.subButton.color = col;
			}
		}

		if ( subButtonAnimation.subButtonRing )
		{
			subButtonAnimation.subButtonRing.fillAmount = 0;
			subButtonAnimation.subButtonRing.enabled = false;
		}

		if ( subButtonAnimation.UpdateHoverSound != null )
		{
			subButtonAnimation.UpdateHoverSound.Stop();
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

	public AudioSource EnterHoverSound;
	public AudioSource ExitHoverSound;
	public AudioSource UpdateHoverSound;
	public AudioSource EnterSubSound;
	public AudioSource ExitSubSound;
	public AudioSource FocusSound;
	public AudioSource ConfirmSound;
}