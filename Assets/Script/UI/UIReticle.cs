using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIReticle : MonoBehaviour , ICardboardPointer {

	[SerializeField] Image mainCircle;
	[SerializeField] Image secCircle;

	void OnEnable() {
		GazeInputModule.cardboardPointer = this;
	}

	void OnDisable() {
		if (GazeInputModule.cardboardPointer == this) {
			GazeInputModule.cardboardPointer = null;
		}
	}

	/// This is called when the 'BaseInputModule' system should be enabled.
	public void OnGazeEnabled(){}
	/// This is called when the 'BaseInputModule' system should be disabled.
	public void OnGazeDisabled(){}

	/// Called when the user is looking on a valid GameObject. This can be a 3D
	/// or UI element.
	///
	/// The camera is the event camera, the target is the object
	/// the user is looking at, and the intersectionPosition is the intersection
	/// point of the ray sent from the camera on the object.
	public void OnGazeStart(Camera camera, GameObject targetObject, Vector3 intersectionPosition)
	{
		mainCircle.enabled = true;
		secCircle.enabled = true;
		transform.position = intersectionPosition;
	}

	/// Called every frame the user is still looking at a valid GameObject. This
	/// can be a 3D or UI element.
	///
	/// The camera is the event camera, the target is the object the user is
	/// looking at, and the intersectionPosition is the intersection point of the
	/// ray sent from the camera on the object.
	public void OnGazeStay(Camera camera, GameObject targetObject, Vector3 intersectionPosition)
	{
		transform.position = intersectionPosition;
	}

	/// Called when the user's look no longer intersects an object previously
	/// intersected with a ray projected from the camera.
	/// This is also called just before **OnGazeDisabled** and may have have any of
	/// the values set as **null**.
	///
	/// The camera is the event camera and the target is the object the user
	/// previously looked at.
	public void OnGazeExit(Camera camera, GameObject targetObject)
	{
		mainCircle.enabled = false;
		secCircle.enabled = false;
	}

	/// Called when the Cardboard trigger is initiated. This is practically when
	/// the user begins pressing the trigger.
	public void OnGazeTriggerStart(Camera camera)
	{
		
	}

	/// Called when the Cardboard trigger is finished. This is practically when
	/// the user releases the trigger.
	public void OnGazeTriggerEnd(Camera camera)
	{
	}
}
