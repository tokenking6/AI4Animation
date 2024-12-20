﻿using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class PID {
	public Parameters Gain;

	private float Integral;
	private float Derivative;
	private float LastError;

	public PID(float P, float I, float D) {
		Gain = new Parameters(P, I, D);
	}

	public PID(Parameters gain) {
		Gain = gain;
	}

	public float Update(float error, float deltaTime) {
		Integral += error*deltaTime;
		Derivative = (error-LastError)/deltaTime;
		LastError = error;
		return error*Gain.P + Integral*Gain.I + Derivative*Gain.D;
	}

	[Serializable]
	public class Parameters {
		public float P;
		public float I;
		public float D;
		public Parameters(float p, float i, float d) {
			P = p;
			I = i;
			D = d;
		}
		#if UNITY_EDITOR
		public void Inspector(string name) {
			float width = EditorGUIUtility.currentViewWidth;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(name, GUILayout.Width(0.4f*width));
			EditorGUILayout.LabelField("P", GUILayout.Width(25f));
			P = EditorGUILayout.FloatField(P, GUILayout.Width(0.2f*width-0.6f*75f));
			EditorGUILayout.LabelField("I", GUILayout.Width(25f));
			I = EditorGUILayout.FloatField(I, GUILayout.Width(0.2f*width-0.6f*75f));
			EditorGUILayout.LabelField("D", GUILayout.Width(25f));
			D = EditorGUILayout.FloatField(D, GUILayout.Width(0.2f*width-0.6f*75f));
			EditorGUILayout.EndHorizontal();
		}
		#endif
	}
}

[Serializable]
public class PID_Vector3Controller {
	public PID.Parameters Gain;

	private Vector3 Integral;
	private Vector3 Derivative;
	private Vector3 LastError;

	public PID_Vector3Controller(float P, float I, float D) {
		Gain = new PID.Parameters(P, I, D);
	}

	public PID_Vector3Controller(PID.Parameters gain) {
		Gain = gain;
	}

	public Vector3 Update(Vector3 current, Vector3 target, float deltaTime) {
		Vector3 error = target-current;
		Integral += error*deltaTime;
		Derivative = (error-LastError)/deltaTime;
		LastError = error;
		return current + error*Gain.P + Integral*Gain.I + Derivative*Gain.D;
	}

	// public Vector3 Update(Vector3 error, float deltaTime) {
	// 	Integral += error*deltaTime;
	// 	Derivative = (error-LastError)/deltaTime;
	// 	LastError = error;
	// 	return error*Gain.P + Integral*Gain.I + Derivative*Gain.D;
	// }
}

// [Serializable]
// public class PID_QuaternionController {
// 	public PID.Parameters Gain;

// 	private Vector4 Integral;
// 	private Vector4 Derivative;
// 	private Vector4 LastError;

// 	public PID_QuaternionController(float P, float I, float D) {
// 		Gain = new PID.Parameters(P, I, D);
// 	}

// 	public PID_QuaternionController(PID.Parameters gain) {
// 		Gain = gain;
// 	}

// 	public Quaternion Update(Quaternion current, Quaternion target, float deltaTime) {
// 		Vector4 error = target.GetVector()-current.GetVector();
// 		Integral += error*deltaTime;
// 		Derivative = (error-LastError)/deltaTime;
// 		LastError = error;
// 		Vector4 result = current.GetVector() + error*Gain.P + Integral*Gain.I + Derivative*Gain.D;
// 		return new Quaternion(result.x, result.y, result.z, result.w).normalized;
// 	}
// }

[Serializable]
public class PID_QuaternionController {
	public PID.Parameters Gain;

	private PID_Vector3Controller Y;
	private PID_Vector3Controller Z;

	public PID_QuaternionController(float P, float I, float D) {
		Gain = new PID.Parameters(P, I, D);
	}

	public PID_QuaternionController(PID.Parameters gain) {
		Gain = gain;
	}

	public Quaternion Update(Quaternion current, Quaternion target, float deltaTime) {
		if(Y == null || Z == null) {
			Y = new PID_Vector3Controller(Gain);
			Z = new PID_Vector3Controller(Gain);
		}
		Vector3 y = Y.Update(current.GetUp(), target.GetUp(), deltaTime);
		Vector3 z = Z.Update(current.GetForward(), target.GetForward(), deltaTime);
		return Quaternion.LookRotation(z, y);
	}
}