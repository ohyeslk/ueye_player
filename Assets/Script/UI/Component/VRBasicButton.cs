using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class VRBasicButton : MonoBehaviour {
	[SerializeField] protected Image img;
	[SerializeField] protected Text text;
	[SerializeField] ConfirmAnimation confirmAni;


	virtual public void OnFucus( )
	{

	}

	virtual public void OnConfirm ()
	{
		confirmAni.confirmRing.transform.DOScale( confirmAni.confirmRingScaleUp , confirmAni.confirmRingScaleUpTime );
		confirmAni.confirmRing.DOFade( 0 , confirmAni.confirmRingScaleUpTime );
	}

	virtual public void OnHover(UIHoverEvent e)
	{
		
	}

	/// <summary>
	/// Show the confirm button 
	/// </summary>
	public void OnShowConfirm()
	{
		confirmAni.confirm.DOKill();
		confirmAni.confirmRing.DOKill();
		confirmAni.confirm.transform.DOKill();
		float time = confirmAni.showTime;
		confirmAni.confirm.enabled = true;
		confirmAni.confirmRing.enabled = true;
		confirmAni.confirm.DOFade( 1f , time );
		confirmAni.confirm.transform.DOLocalMoveY( confirmAni.posY + confirmAni.moveY , 0 );
		confirmAni.confirm.transform.DOLocalMoveY( confirmAni.posY , time );

	}

	/// <summary>
	/// Hide the confirm button
	/// </summary>
	public void OnHideConfirm()
	{
		confirmAni.confirm.DOKill();
		confirmAni.confirmRing.DOKill();
		confirmAni.confirm.transform.DOKill();
		float time = confirmAni.hideTime;
		confirmAni.confirm.transform.DOLocalMoveY( confirmAni.posY + confirmAni.moveY , time );
		confirmAni.confirm.DOFade( 0 , time  );
		confirmAni.confirmRing.DOFillAmount( 0 , time ).OnComplete(ResetConfirm);
	}

	/// <summary>
	/// hide the confirm ring
	/// </summary>
	public void OnHideConfirmRing()
	{
		confirmAni.confirm.transform.DOKill();
		float time = confirmAni.hideTime;
		confirmAni.confirmRing.DOFillAmount( 0 , time );
	}

	public void OnResetConfirmRing()
	{
		Color col = confirmAni.confirmRing.color ; 
		col.a = 1f ; 
		confirmAni.confirmRing.color = col;
		confirmAni.confirmRing.transform.localScale = Vector3.one;
	}
		
	public void UpdateConfirmRing( float process )
	{
		confirmAni.confirmRing.fillAmount = confirmAni.confirmCurve.Evaluate( process );
	}

	protected void ResetConfirm()
	{
		confirmAni.confirmRing.fillAmount = 0;
		Color col = confirmAni.confirm.color ;
		col.a = 0.01f;
		confirmAni.confirm.color = col;
		confirmAni.confirmRing.enabled = false;
	}


	virtual public void Clear()
	{
	}

}

[System.Serializable]
public struct ConfirmAnimation
{
	public Image confirm;
	public Image confirmRing;
	public AnimationCurve confirmCurve;
	public float showTime;
	public float hideTime;
	public float posY;
	public float moveY;
				public float confirmRingScaleUp;
				public float confirmRingScaleUpTime;
}