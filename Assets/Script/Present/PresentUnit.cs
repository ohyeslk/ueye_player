using UnityEngine;
using System.Collections;

public class PresentUnit : MonoBehaviour {

	void Start()
	{
		Collider col = GetComponentInChildren<Collider>();
		PresentSensor sensor = col.gameObject.AddComponent<PresentSensor>();
		sensor.Init( this );
	}
}
