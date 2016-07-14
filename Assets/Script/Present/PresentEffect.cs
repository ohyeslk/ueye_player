using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PresentEffect : MonoBehaviour {

	PresentUnit parent;
	[SerializeField] ParticleSystem[] particles;
//	[SerializeField] AnimationCurve particleCurve;
	[SerializeField] Animator modelAnimator;
	[SerializeField] SpriteRenderer shadow;
	[SerializeField] ParticleSystem exposion;
	[SerializeField] ParticleSystem birth;

	[SerializeField] float selectEffectDuration = 1f ;
	[SerializeField] float exposeEffectDuration = 1f ;

	const float shadowFloorY = -0.499f;

	public enum State
	{
		Normal,
		Selected,
		Thrown,
		Expose,
	}
	State state = State.Normal;

	public void Init( PresentUnit unit )
	{
		parent = unit;
		BirthEffect();
	}
		

	public void OnFingerDown( FingerDownEvent e ) {
		BeginSelectEffect();
	}

	public void OnFingerUp( FingerUpEvent e ) {

		EndSelectEffect();
	}

	public void OnFingerMove( FingerMotionEvent e ) {

		SelectedThisFrame = true;
	}


	public void OnFingerStationary( FingerMotionEvent e )
	{

		SelectedThisFrame = true;
	}

	void BeginSelectEffect()
	{
		if ( particles != null )
		{
			foreach( ParticleSystem p in particles )
			{
				var emission = p.emission;
				emission.enabled = true;
			}
		}

		if ( modelAnimator != null )
		{
			DOTween.To( () => modelAnimator.speed , (x) => modelAnimator.speed = x , 4f , selectEffectDuration ).SetEase( Ease.InOutCirc );
		}

		state = State.Selected;
		SelectedThisFrame = SelectedLastFrame = true;
	}

	void EndSelectEffect()
	{
		if ( particles != null )
		{
			foreach( ParticleSystem p in particles )
			{
				var emission = p.emission;
				emission.enabled = false;
			}
		}

		if ( modelAnimator != null )
		{
			DOTween.To( () => modelAnimator.speed , (x) => modelAnimator.speed = x , 1f , selectEffectDuration ).SetEase( Ease.InOutCirc );
		}

		parent.EndSelect();
		state = State.Normal;
	}


	bool SelectedLastFrame;
	bool SelectedThisFrame;


	void LateUpdate()
	{

		if ( state == State.Selected && !SelectedLastFrame && !SelectedThisFrame)
		{
			EndSelectEffect();			
		}

		SelectedLastFrame = SelectedThisFrame;
		SelectedThisFrame = false;

	}

	void Update()
	{
		if ( shadow != null )
		{
			Vector3 shadowPos = transform.position;
			shadowPos.y = shadowFloorY;
			shadow.transform.position = shadowPos;
			shadow.transform.rotation = Quaternion.Euler( 90f , 0 , 0 );
		}
	}

	void OnCollisionEnter(Collision collision) {
		Debug.Log("ON COLLISON");
		if ( collision.collider.tag == "PresentCollison" )
		{
			Expose();
		}

	}


	void Expose()
	{
		if ( exposion != null )
		{
			exposion.Emit( state == State.Expose ? 20 : 110 );
		}

		transform.DOScale( Vector3.zero , exposeEffectDuration ).OnComplete( SelfDestory );

		state = State.Expose;
	}

	void BirthEffect()
	{
		if ( birth != null )
		{
			birth.Emit( 65 );	
		}

	}

	void SelfDestory()
	{
		gameObject.SetActive( false );
	}


}
