using UnityEngine;
using System.Collections;

public class PresentEffect : MonoBehaviour {

	[SerializeField] ParticleSystem[] TouchFeedbackParticle;

	public void EmitTouchFeedbackParticle()
	{
		foreach( ParticleSystem ps in TouchFeedbackParticle )
		{
			var em = ps.emission;
			em.enabled = true;
		}
	}

	public void StopTouchFeedbackParticle()
	{
		foreach( ParticleSystem ps in TouchFeedbackParticle )
		{
			var em = ps.emission;
			em.enabled = false;
		}
		
	}
}
