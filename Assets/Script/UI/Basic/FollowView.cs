using UnityEngine;
using System.Collections;

public class FollowView : MonoBehaviour {

	float initDistance;

	void Awake()
	{
		Vector3 dis = transform.position - Camera.main.transform.position;
		dis.y = 0;
		initDistance = dis.magnitude;
	}

	void LateUpdate()
	{
		Vector3 cameraForward = Camera.main.transform.forward;
		cameraForward.y = 0;
		Vector3 toPos = cameraForward.normalized * initDistance;
		toPos.y = transform.position.y;
		transform.position = toPos;

		transform.rotation = Quaternion.LookRotation( - cameraForward );

	}


}
