using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class VRBasicButton : MonoBehaviour {
	[SerializeField] protected Image img;
	[SerializeField] protected Text text;
	[SerializeField] protected SubAnimation subButtonAnimation;


	virtual public void OnFucus( )
	{

	}

	virtual public void OnConfirm ()
	{
		subButtonAnimation.subButtonRing.transform.DOScale( subButtonAnimation.subRingScaleUp , subButtonAnimation.subRingScaleUpTime );
		subButtonAnimation.subButtonRing.DOFade( 0 , subButtonAnimation.subRingScaleUpTime );
	}

	virtual public void OnHover(UIHoverEvent e)
	{
		
	}

	/// <summary>
	/// Show the confirm button 
	/// </summary>
	virtual public void OnEnterHover()
	{
		Debug.Log("Show Button ");
		subButtonAnimation.subButton.DOKill();
		subButtonAnimation.subButtonRing.DOKill();
		subButtonAnimation.subButton.transform.DOKill();
		float time = subButtonAnimation.showTime;
		subButtonAnimation.subButton.enabled = true;
		subButtonAnimation.subButtonRing.enabled = true;
		subButtonAnimation.subButton.DOFade( 1f , time );
		subButtonAnimation.subButton.transform.DOLocalMoveY( subButtonAnimation.posY + subButtonAnimation.moveY , 0 );
		subButtonAnimation.subButton.transform.DOLocalMoveY( subButtonAnimation.posY , time );

	}

	/// <summary>
	/// Hide the confirm button
	/// </summary>
	virtual public void OnExitHover()
	{
		Debug.Log("Hide Button ");
		subButtonAnimation.subButton.DOKill();
		subButtonAnimation.subButtonRing.DOKill();
		subButtonAnimation.subButton.transform.DOKill();
		float time = subButtonAnimation.hideTime;
		subButtonAnimation.subButton.transform.DOLocalMoveY( subButtonAnimation.posY + subButtonAnimation.moveY , time );
		subButtonAnimation.subButton.DOFade( 0 , time  );
		subButtonAnimation.subButtonRing.DOFillAmount( 0 , time ).OnComplete(ResetSubButton);
	}

	/// <summary>
	/// hide the confirm ring
	/// </summary>
	virtual public void OnEnterSub()
	{
		subButtonAnimation.subButton.transform.DOKill();
		float time = subButtonAnimation.hideTime;
		subButtonAnimation.subButtonRing.DOFillAmount( 0 , time );
	}

	virtual public void OnExitSub()
	{
		Color col = subButtonAnimation.subButtonRing.color ; 
		col.a = 1f ; 
		subButtonAnimation.subButtonRing.color = col;
		subButtonAnimation.subButtonRing.transform.localScale = Vector3.one;
	}
		
	virtual public void UpdateHover( float process )
	{
		subButtonAnimation.subButtonRing.fillAmount = subButtonAnimation.confirmCurve.Evaluate( process );
	}

	protected void ResetSubButton()
	{
		subButtonAnimation.subButtonRing.fillAmount = 0;
		Color col = subButtonAnimation.subButton.color ;
		col.a = 0.01f;
		subButtonAnimation.subButton.color = col;
		subButtonAnimation.subButtonRing.enabled = false;
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