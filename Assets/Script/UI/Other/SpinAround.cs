using UnityEngine;
using System.Collections;

public class SpinAround : MonoBehaviour {

	[SerializeField] AnimationCurve spinCurve;
	[SerializeField] float radius;
	[SerializeField] float Interval;

	Vector3 center;
	void Awake()
	{
		center = transform.localPosition;
	}


	void LateUpdate()
	{
		float angle = Mathf.PI * 2f * spinCurve.Evaluate( Time.time / Interval );
		transform.localPosition = new Vector3( Mathf.Sin( angle ) , Mathf.Cos( angle )) * radius + center;
	}
}
