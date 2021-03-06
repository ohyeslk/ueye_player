﻿// Copyright 2015 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[AddComponentMenu ("Cardboard/UI/CardboardReticle")]
[RequireComponent (typeof(Renderer))]
public class CardboardReticle : MonoBehaviour, ICardboardPointer
{
	/// Number of segments making the reticle circle.
	public int reticleSegments = 20;

	/// Growth speed multiplier for the reticle/
	public float reticleGrowthSpeed = 8.0f;

	// Private members
	private Material materialComp;
	private Material materialOut;
	private GameObject targetObj;

	// Current inner angle of the reticle (in degrees).
	private float reticleInnerAngle = 0.0f;
	// Current outer angle of the reticle (in degrees).
	private float reticleOuterAngle = 0.5f;
	// Current distance of the reticle (in meters).
	private float reticleDistanceInMeters = 10.0f;

	// Minimum inner angle of the reticle (in degrees).
	private const float kReticleMinInnerAngle = 0.0f;
	// Minimum outer angle of the reticle (in degrees).
	private const float kReticleMinOuterAngle = 0.5f;
	// Angle at which to expand the reticle when intersecting with an object
	// (in degrees).
	private const float kReticleGrowthAngle = 1.5f;

	// Minimum distance of the reticle (in meters).
	private const float kReticleDistanceMin = 0.75f;
	// Maximum distance of the reticle (in meters).
	private const float kReticleDistanceMax = 10.0f;

	// Current inner and outer diameters of the reticle,
	// before distance multiplication.
	private float reticleInnerDiameter = 0.0f;
	private float reticleOuterDiameter = 0.0f;

	[System.Serializable]
	public struct OutParameter
	{	
		public float outInnerDiameterAngle;
		public float outOuterDiameterAngle;
		public GameObject outObj;
		public float focusTime;
		public float confirmTime;
		public AnimationCurve confirmCurve;
	}
	[SerializeField] OutParameter outParameter;

	private float outAngle = 6f;

	void Start ()
	{
		CreateReticleVertices (gameObject);
		CreateReticleVertices (outParameter.outObj);

		materialComp = gameObject.GetComponent<Renderer> ().material;
		materialOut = outParameter.outObj.GetComponent<Renderer> ().material;
	}

	void OnEnable ()
	{
		GazeInputModule.cardboardPointer = this;
	}

	void OnDisable ()
	{
		if (GazeInputModule.cardboardPointer == this) {
			GazeInputModule.cardboardPointer = null;
		}
	}

	void Update ()
	{
		UpdateDiameters ();
	}

	/// This is called when the 'BaseInputModule' system should be enabled.
	public void OnGazeEnabled ()
	{

	}

	/// This is called when the 'BaseInputModule' system should be disabled.
	public void OnGazeDisabled ()
	{

	}



	float startViewTime = Mathf.Infinity;
	GameObject m_target;
	GameObject TargetObj {
		get {
			return m_target;
		}
		set {
			if (m_target != value) {
				if (value != null) {
					startViewTime = Time.time;
				} else {
					startViewTime = Mathf.Infinity;
				}
			}
			m_target = value;
		}
	}

	/// Called when the user is looking on a valid GameObject. This can be a 3D
	/// or UI element.
	///
	/// The camera is the event camera, the target is the object
	/// the user is looking at, and the intersectionPosition is the intersection
	/// point of the ray sent from the camera on the object.
	public void OnGazeStart (Camera camera, GameObject targetObject, Vector3 intersectionPosition)
	{
		SetGazeTarget (intersectionPosition, targetObject);
	}

	/// Called every frame the user is still looking at a valid GameObject. This
	/// can be a 3D or UI element.
	///
	/// The camera is the event camera, the target is the object the user is
	/// looking at, and the intersectionPosition is the intersection point of the
	/// ray sent from the camera on the object.
	public void OnGazeStay (Camera camera, GameObject targetObject, Vector3 intersectionPosition)
	{
		SetGazeTarget (intersectionPosition, targetObject);
	}

	/// Called when the user's look no longer intersects an object previously
	/// intersected with a ray projected from the camera.
	/// This is also called just before **OnGazeDisabled** and may have have any of
	/// the values set as **null**.
	///
	/// The camera is the event camera and the target is the object the user
	/// previously looked at.
	public void OnGazeExit (Camera camera, GameObject targetObject)
	{
		TargetObj = null;
		reticleDistanceInMeters = kReticleDistanceMax;
		reticleInnerAngle = kReticleMinInnerAngle;
		reticleOuterAngle = kReticleMinOuterAngle;
	}

	/// Called when the Cardboard trigger is initiated. This is practically when
	/// the user begins pressing the trigger.
	public void OnGazeTriggerStart (Camera camera)
	{
		// Put your reticle trigger start logic here :)
	}

	/// Called when the Cardboard trigger is finished. This is practically when
	/// the user releases the trigger.
	public void OnGazeTriggerEnd (Camera camera)
	{
		// Put your reticle trigger end logic here :)
	}

	private void CreateReticleVertices (GameObject obj)
	{
		Mesh mesh = new Mesh ();
		obj.AddComponent<MeshFilter> ();
		obj.GetComponent<MeshFilter> ().mesh = mesh;

		int segments_count = reticleSegments;
		int vertex_count = (segments_count + 1) * 2;

		#region Vertices

		Vector3[] vertices = new Vector3[vertex_count];

		const float kTwoPi = Mathf.PI * 2.0f;
		int vi = 0;
		for (int si = 0; si <= segments_count; ++si) {
			// Add two vertices for every circle segment: one at the beginning of the
			// prism, and one at the end of the prism.
			float angle = (float)si / (float)(segments_count) * kTwoPi;

			float x = Mathf.Sin (angle);
			float y = Mathf.Cos (angle);

			vertices [vi++] = new Vector3 (x, y, 0.0f); // Outer vertex.
			vertices [vi++] = new Vector3 (x, y, 1.0f); // Inner vertex.
		}
		#endregion

		#region Triangles
		int indices_count = (segments_count + 1) * 3 * 2;
		int[] indices = new int[indices_count];

		int vert = 0;
		int idx = 0;
		for (int si = 0; si < segments_count; ++si) {
			indices [idx++] = vert + 1;
			indices [idx++] = vert;
			indices [idx++] = vert + 2;

			indices [idx++] = vert + 1;
			indices [idx++] = vert + 2;
			indices [idx++] = vert + 3;

			vert += 2;
		}
		#endregion

		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.RecalculateBounds ();
		mesh.Optimize ();
	}

	private void UpdateDiameters ()
	{
		// Update Inner Diameters
		reticleDistanceInMeters =
      Mathf.Clamp (reticleDistanceInMeters, kReticleDistanceMin, kReticleDistanceMax);

		if (reticleInnerAngle < kReticleMinInnerAngle) {
			reticleInnerAngle = kReticleMinInnerAngle;
		}

		if (reticleOuterAngle < kReticleMinOuterAngle) {
			reticleOuterAngle = kReticleMinOuterAngle;
		}

		float inner_half_angle_radians = Mathf.Deg2Rad * reticleInnerAngle * 0.5f;
		float outer_half_angle_radians = Mathf.Deg2Rad * reticleOuterAngle * 0.5f;

		float inner_diameter = 2.0f * Mathf.Tan (inner_half_angle_radians);
		float outer_diameter = 2.0f * Mathf.Tan (outer_half_angle_radians);

		reticleInnerDiameter =
        Mathf.Lerp (reticleInnerDiameter, inner_diameter, Time.deltaTime * reticleGrowthSpeed);
		reticleOuterDiameter =
        Mathf.Lerp (reticleOuterDiameter, outer_diameter, Time.deltaTime * reticleGrowthSpeed);

		materialComp.SetFloat ("_InnerDiameter", reticleInnerDiameter * reticleDistanceInMeters);
		materialComp.SetFloat ("_OuterDiameter", reticleOuterDiameter * reticleDistanceInMeters);
		materialComp.SetFloat ("_DistanceInMeters", reticleDistanceInMeters);
		materialComp.SetFloat ("_Angle", Mathf.PI * 2f);

		// Update Outer Diameters

		float process = Mathf.Clamp01(( Time.time - startViewTime - outParameter.focusTime) / outParameter.confirmTime );

		if ( outParameter.confirmCurve != null )
			process = outParameter.confirmCurve.Evaluate( process);
		outAngle = process * 2f * Mathf.PI;

		Debug.Log("Start time " + startViewTime  + " process " + process + " angle " + outAngle);

		materialOut.SetFloat ("_InnerDiameter", 2.0f * Mathf.Tan (Mathf.Deg2Rad * 0.5f * outParameter.outInnerDiameterAngle) * reticleDistanceInMeters);
		materialOut.SetFloat ("_OuterDiameter", 2.0f * Mathf.Tan (Mathf.Deg2Rad * 0.5f * outParameter.outOuterDiameterAngle) * reticleDistanceInMeters);
		materialOut.SetFloat ("_DistanceInMeters", reticleDistanceInMeters);
		materialOut.SetFloat ("_Angle", outAngle);
	}

	private void SetGazeTarget (Vector3 target, GameObject obj)
	{
		Vector3 targetLocalPosition = transform.parent.InverseTransformPoint (target);

		reticleDistanceInMeters =
        Mathf.Clamp (targetLocalPosition.z, kReticleDistanceMin, kReticleDistanceMax);
		reticleInnerAngle = kReticleMinInnerAngle + kReticleGrowthAngle;
		reticleOuterAngle = kReticleMinOuterAngle + kReticleGrowthAngle;

		TargetObj = obj;

	}
}