using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class FloatButtonWithEffect : VRFloatButton {

	[SerializeField] GrowAnimation growAnimation;

	public void StartGrowColor()
	{
		StartCoroutine( GrowColorDo() );
	}

	public override void OnConfirm ()
	{
		base.OnConfirm ();

		StartGrowColor();
	}

	IEnumerator GrowColorDo ()
	{
		float time = 0 ; 
		growAnimation.FlowEffect.enabled = true;

//		{
//			Color col = growAnimation.FlowEffect.color;
//			col.a = 0.5f;
//			growAnimation.FlowEffect.color = col;
//		}

		while ( true )
		{
			if ( time >= growAnimation.growTime )
				break;

			growAnimation.FlowEffect.transform.localScale = Vector3.one * (growAnimation.scaleCurve.Evaluate( time / growAnimation.growTime ) * growAnimation.growScale + 1f);
		
			{
				Color col = growAnimation.FlowEffect.color;
				col.r = col.g = col.b = growAnimation.grayCurve.Evaluate( time / growAnimation.growTime );
				col.a = growAnimation.alphaCurve.Evaluate( time / growAnimation.growTime );
				growAnimation.FlowEffect.color = col;
			}

			time += Time.deltaTime;

			yield return null;
		}

		{
			Color col = growAnimation.FlowEffect.color;
			col.a = 0;
			growAnimation.FlowEffect.color = col;
		}

		growAnimation.FlowEffect.enabled = false;
	}

	public override void OnBecomeInvisible (float time)
	{
		base.OnBecomeInvisible (time);

		if ( growAnimation.FlowEffect != null )
		{
			growAnimation.FlowEffect.DOFade( 0 , time );
		}
	}

	public override void OnBecomeVisible (float time)
	{
		base.OnBecomeVisible (time);

		if ( growAnimation.FlowEffect != null )
		{
			growAnimation.FlowEffect.enabled = false;
		}
	}

	[System.Serializable]
	public struct GrowAnimation
	{
		public Image FlowEffect;
		public AnimationCurve grayCurve;
		public AnimationCurve alphaCurve;
		public float growTime;
		public float growScale;
		public AnimationCurve scaleCurve;
	}
}
