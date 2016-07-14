using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PresentUnit : MonoBehaviour {

	[SerializeField] SphereCollider col;
	[SerializeField] PresentEffect effect;
	[SerializeField] Rigidbody m_rigid;

	float normalRadius;
	float bigRadius;
	void Start()
	{
//		if ( col == null )
//			col = GetComponent<SphereCollider>();
//		
//		if ( col == null )
//			col = GetComponentInChildren<SphereCollider>();
//
//		if ( col != null )
//		{
//			PresentSensor sensor = col.gameObject.AddComponent<PresentSensor>();
//			sensor.Init( this );
//		}

		if ( effect == null )
		{
			effect = gameObject.AddComponent<PresentEffect>();
		}

		if ( effect != null )
		{
			effect.Init( this );
		}

		if ( m_rigid != null )
		{
			m_rigid = GetComponent<Rigidbody>();
		}

		if ( col != null )
		{
			normalRadius = col.radius;
			bigRadius = col.radius * 5f;
		}
	}

	void OnEnable()
	{
		
	}

	void OnDisable()
	{
		
	}

	public void OnFingerDown( FingerDownEvent e ) {
		if ( effect != null )
		{
			effect.OnFingerDown( e );
		}
	}

	public void OnFingerUp( FingerUpEvent e ) {
		if ( effect != null )
		{
			effect.OnFingerUp( e );
		}
		Throw( e.Finger.DeltaPosition );
	}

	public void OnFingerMove( FingerMotionEvent e ) {
		if ( e.Phase == FingerMotionPhase.Started )
		{
			BeginSelect();
		}
		else if ( e.Phase == FingerMotionPhase.Updated )
		{
			Vector3 screenPos = Camera.main.WorldToScreenPoint( transform.position );
			Vector3 newScreenPos = screenPos + new Vector3( e.Finger.DeltaPosition.x , e.Finger.DeltaPosition.y , 0 );
			Vector3 toward = Camera.main.ScreenPointToRay( newScreenPos ).direction;

			float distance = ( transform.position - Camera.main.transform.position ).magnitude;
			transform.position = Camera.main.transform.position + distance * toward;
		}
		else if ( e.Phase == FingerMotionPhase.Ended )
		{
			EndSelect();
		}

		if ( effect != null )
		{
			effect.OnFingerMove( e );
		}
	}

	public void BeginSelect()
	{
		col.radius = bigRadius;
		CardboardHead.Lock();
		
	}

	public void EndSelect()
	{
		col.radius = normalRadius;

		// lock the screen for 0.5 second after end
		Sequence seq = DOTween.Sequence();
		seq.AppendInterval( 0.5f );
		seq.AppendCallback( Unlock );
	}

	public void Unlock()
	{
		CardboardHead.UnLock();
	}

	public void OnFingerStationary( FingerMotionEvent e )
	{

		if ( effect != null )
		{
			effect.OnFingerStationary( e );
		}
		
	}


	float FingerSelectTimer = 0;

	void Throw( Vector2 delta )
	{
		Debug.Log("Throw");
//		Vector3 velocity = delta;
//		Debug.Log("Velocity " + velocity );
//		velocity.z = Mathf.Clamp( FingerSelectTimer , 0 , 2f );
//
//		Vector3 toward = Camera.main.transform.forward;
//
//		Vector3 vel_rigid = (Quaternion.LookRotation( velocity ) * Quaternion.FromToRotation( Vector3.forward , toward )).eulerAngles;

		Vector3 vel_z = Camera.main.transform.forward;
		Vector3 vel_y = Camera.main.transform.up;
		Vector3 vel_x = Camera.main.transform.right;

		Vector3 vel = ( vel_x * delta.x + vel_y * delta.y ) * 0.01f  + vel_z * FingerSelectTimer;

		m_rigid.isKinematic = false;
		m_rigid.velocity = vel;
	}


	void Update()
	{
		if ( FingerSelectTimer <= 2f )
			FingerSelectTimer += Time.deltaTime;

	}
}
