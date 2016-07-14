using UnityEngine;
using System.Collections;

public class PresentEffect : MonoBehaviour {

	PresentUnit parent;
	[SerializeField] ParticleSystem[] particles;
	[SerializeField] AnimationCurve particleCurve;
	[SerializeField] Animator modelAnimator;
	[SerializeField] AnimationCurve modelAnimatiorCurve;

	public void Init( PresentUnit unit )
	{
		parent = unit;
	}

	float fingerDownTimer ;
	public void OnFingerDown( FingerDownEvent e ) {
		fingerDownTimer = 0;

		if ( particles != null )
		{
			foreach( ParticleSystem p in particles )
			{
				var emission = p.emission;
				emission.enabled = true;
			}
		}

	}

	public void OnFingerUp( FingerUpEvent e ) {

	}

	public void OnFingerMove( FingerMotionEvent e ) {

		if ( modelAnimator != null && modelAnimatiorCurve != null )
		{
			modelAnimator.speed = modelAnimatiorCurve.Evaluate( fingerDownTimer );
		}
	}

	void Update()
	{
		fingerDownTimer += Time.deltaTime;
	}
}
