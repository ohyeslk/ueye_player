using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class GroupButtonWithEffect : GroupButton {

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
		growAnimation.FlowHelp.enabled = true;

		{
			Color col = growAnimation.FlowHelp.color;
			col.a = 0.5f;
			growAnimation.FlowHelp.color = col;
		}

		while ( true )
		{
			if ( time >= growAnimation.growTime )
				break;

			growAnimation.FlowHelp.transform.localScale = Vector3.one * (growAnimation.scaleCurve.Evaluate( time / growAnimation.growTime ) * growAnimation.growScale + 1f);
		
			{
				Color col = growAnimation.FlowHelp.color;
				col.a = growAnimation.colorCurve.Evaluate( time / growAnimation.growTime );
				growAnimation.FlowHelp.color = col;
			}

			time += Time.deltaTime;

			yield return null;
		}

		{
			Color col = growAnimation.FlowHelp.color;
			col.a = 0;
			growAnimation.FlowHelp.color = col;
		}

		growAnimation.FlowHelp.enabled = false;
	}

	public override void OnBecomeInvisible (float time)
	{
		base.OnBecomeInvisible (time);

		if ( growAnimation.FlowHelp != null )
		{
			growAnimation.FlowHelp.DOFade( 0 , time );
		}
	}

	public override void OnBecomeVisible (float time)
	{
		base.OnBecomeVisible (time);

		if ( growAnimation.FlowHelp != null )
		{
			growAnimation.FlowHelp.enabled = false;
		}
	}

	[System.Serializable]
	public struct GrowAnimation
	{
		public Image FlowHelp;
		public AnimationCurve colorCurve;
		public float growTime;
		public float growScale;
		public AnimationCurve scaleCurve;
	}
}
