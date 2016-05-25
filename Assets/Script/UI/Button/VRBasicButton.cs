using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class VRBasicButton : MonoBehaviour {
	[SerializeField] protected SubAnimation subButtonAnimation;


	virtual public void OnFucus( )
	{

	}

	virtual public void OnConfirm ()
	{
		if ( subButtonAnimation.subButtonRing != null )
		{
			subButtonAnimation.subButtonRing.transform.DOScale( subButtonAnimation.subRingScaleUp , subButtonAnimation.subRingScaleUpTime );
			subButtonAnimation.subButtonRing.DOFade( 0 , subButtonAnimation.subRingScaleUpTime );
		}
	}

	virtual public void OnHover(UIHoverEvent e)
	{
		
	}

	/// <summary>
	/// Show the confirm button 
	/// </summary>
	virtual public void OnEnterHover()
	{
		float time = subButtonAnimation.showTime;
		if ( subButtonAnimation.subButton != null )
		{
			subButtonAnimation.subButton.DOKill();
			subButtonAnimation.subButton.transform.DOKill();
			subButtonAnimation.subButton.enabled = true;
			subButtonAnimation.subButton.DOFade( 1f , time );
			subButtonAnimation.subButton.transform.DOLocalMoveY( subButtonAnimation.posY + subButtonAnimation.moveY , 0 );
			subButtonAnimation.subButton.transform.DOLocalMoveY( subButtonAnimation.posY , time );
		}

		if ( subButtonAnimation.subButtonRing != null )
		{
			subButtonAnimation.subButtonRing.DOKill();
			subButtonAnimation.subButtonRing.enabled = true;
			subButtonAnimation.subButtonRing.DOFade( 1f , 0 );
			subButtonAnimation.subButtonRing.transform.localScale = Vector3.one;
		}

	}

	/// <summary>
	/// Hide the confirm button
	/// </summary>
	virtual public void OnExitHover()
	{
		float time = subButtonAnimation.hideTime;
		if ( subButtonAnimation.subButton != null )
		{
			subButtonAnimation.subButton.DOKill();
			subButtonAnimation.subButton.transform.DOKill();
			subButtonAnimation.subButton.transform.DOLocalMoveY( subButtonAnimation.posY + subButtonAnimation.moveY , time );
			subButtonAnimation.subButton.DOFade( 0 , time  );
		}
		if ( subButtonAnimation.subButtonRing != null )
		{
			subButtonAnimation.subButtonRing.DOKill();
			subButtonAnimation.subButtonRing.DOFillAmount( 0 , time  * subButtonAnimation.subButtonRing.fillAmount ).OnComplete(ResetSubButton);
		}
	}


	virtual public void OnEnterSub()
	{
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
		}
	}
		
	virtual public void UpdateHover( float process )
	{
		if ( subButtonAnimation.subButtonRing != null )
		{
			subButtonAnimation.subButtonRing.fillAmount = subButtonAnimation.confirmCurve.Evaluate( process );
		}
	}

	protected void ResetSubButton()
	{
		if ( subButtonAnimation.subButton != null )
		{
			Color col = subButtonAnimation.subButton.color ;
			col.a = 0.01f;
			subButtonAnimation.subButton.color = col;
		}
		if ( subButtonAnimation.subButtonRing )
		{
			subButtonAnimation.subButtonRing.fillAmount = 0;
			subButtonAnimation.subButtonRing.enabled = false;
		}
	}


	virtual public void Clear()
	{
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
}