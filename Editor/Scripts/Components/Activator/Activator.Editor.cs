using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Intruder.Tools.Inspectors
{
	[CustomEditor( typeof( Activator ) ), CanEditMultipleObjects]
	public class ActivatorEditor : Editor
	{
		public Activator Activator => target as Activator;
		public Transform Transform => Activator.transform;
		public Vector3 Position => Transform.position;

		private void OnSceneGUI()
		{
			var activeView = SceneView.lastActiveSceneView;

			var distance = Vector3.Distance( activeView.camera.transform.position, Position );
			var size = 0.05f / distance / (2.0f * Mathf.Tan( (activeView.camera.fieldOfView / 2.0f) * Mathf.Deg2Rad )) * 1500.0f;
			Handles.DrawAAPolyLine( size, Position, Activator.objectsToEnable[0].transform.position );

			var text = new GUIStyle( EditorStyles.label );
			text.fontSize = (int)size;

			Handles.Label( Position, "Awesome", text );
		}
	}
}
