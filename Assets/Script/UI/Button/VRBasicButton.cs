using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class VRBasicButton : MonoBehaviour {
	[SerializeField] protected SubAnimation subButtonAnimation;
<<<<<<< HEAD
	public bool m_Enable = true;

	virtual public void OnFucus( )
	{
		if ( m_Enable )
		{
		}
=======


	virtual public void OnFucus( )
	{

>>>>>>> 223bc20077c7b09e94d589709d9b76fe007d3996
	}

	virtual public void OnConfirm ()
	{
<<<<<<< HEAD
		if ( m_Enable)
		{
=======
>>>>>>> 223bc20077c7b09e94d589709d9b76fe007d3996
		if ( subButtonAnimation.subButtonRing != null )
		{
			subButtonAnimation.subButtonRing.transform.DOScale( subButtonAnimation.subRingScaleUp , subButtonAnimation.subRingScaleUpTime );
			subButtonAnimation.subButtonRing.DOFade( 0 , subButtonAnimation.subRingScaleUpTime );
		}
<<<<<<< HEAD
		if ( subButtonAnimation.subButton != null )
		{
			if ( subButtonAnimation.subButtonFade )
			{
					subButtonAnimation.subButton.DOKill();	
				subButtonAnimation.subButton.DOFade( 0 , subButtonAnimation.subRingScaleUpTime );
			}
		}
		}
=======
>>>>>>> 223bc20077c7b09e94d589709d9b76fe007d3996
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
<<<<<<< HEAD
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
=======
			subButtonAnimation.subButton.DOKill();
			subButtonAnimation.subButton.transform.DOKill();
			subButtonAnimation.subButton.enabled = true;
			subButtonAnimation.subButton.DOFade( 1f , time );
			subButtonAnimation.subButton.transform.DOLocalMoveY( subButtonAnimation.posY + subButtonAnimation.moveY , 0 );
			subButtonAnimation.subButton.transform.DOLocalMoveY( subButtonAnimation.posY , time );
>>>>>>> 223bc20077c7b09e94d589709d9b76fe007d3996
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
<<<<<<< HEAD
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
=======
			subButtonAnimation.subButton.DOKill();
			subButtonAnimation.subButton.transform.DOKill();
			subButtonAnimation.subButton.transform.DOLocalMoveY( subButtonAnimation.posY + subButtonAnimation.moveY , time );
			subButtonAnimation.subButton.DOFade( 0 , time  );
>>>>>>> 223bc20077c7b09e94d589709d9b76fe007d3996
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
<<<<<<< HEAD
=======

>>>>>>> 223bc20077c7b09e94d589709d9b76fe007d3996
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
<<<<<<< HEAD
		if ( m_Enable )
		{
=======
>>>>>>> 223bc20077c7b09e94d589709d9b76fe007d3996
		if ( subButtonAnimation.subButtonRing != null )
		{
			subButtonAnimation.subButtonRing.fillAmount = subButtonAnimation.confirmCurve.Evaluate( process );
		}
<<<<<<< HEAD
		}
=======
>>>>>>> 223bc20077c7b09e94d589709d9b76fe007d3996
	}

	protected void ResetSubButton()
	{
		if ( subButtonAnimation.subButton != null )
		{
<<<<<<< HEAD
			if ( subButtonAnimation.subButtonFade )
			{
				Color col = subButtonAnimation.subButton.color;
				col.a = 0.01f;
				subButtonAnimation.subButton.color = col;
			}
=======
			Color col = subButtonAnimation.subButton.color ;
			col.a = 0.01f;
			subButtonAnimation.subButton.color = col;
>>>>>>> 223bc20077c7b09e94d589709d9b76fe007d3996
		}
		if ( subButtonAnimation.subButtonRing )
		{
			subButtonAnimation.subButtonRing.fillAmount = 0;
			subButtonAnimation.subButtonRing.enabled = false;
		}
	}

<<<<<<< HEAD
	public void OnBecomeVisible( float time )
	{
		if ( subButtonAnimation.subButton != null )
		{
			float t = ( time <= 0 ) ? subButtonAnimation.showTime : time ;
			subButtonAnimation.subButton.DOKill();
			subButtonAnimation.subButton.DOFade( 1f , t );
		}
	}

	public void OnBecomeInvisible( float time )
	{
		if ( subButtonAnimation.subButton != null )
		{
			float t = ( time <= 0 ) ? subButtonAnimation.hideTime : time;
			subButtonAnimation.subButton.DOKill();
			subButtonAnimation.subButton.DOFade( 0 , t );
		}
	}
=======
>>>>>>> 223bc20077c7b09e94d589709d9b76fe007d3996

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
<<<<<<< HEAD
	public bool subButtonFade;
	public bool subButtonMove;
=======
>>>>>>> 223bc20077c7b09e94d589709d9b76fe007d3996
}