using UnityEngine;
using UnityEditor;

namespace Intruder.Tools.SceneGUI
{
	public static class UtilitySceneGUI
	{
		public static void DrawActivatorLine( Activator activator, Vector3 to, Color color = default( Color ) )
		{
			Handles.color = color;

			var activatorPos = activator.transform.position;
			var activeView = SceneView.lastActiveSceneView;

			var direction = (activatorPos - to).normalized;
			var distance = Vector3.Distance( activeView.camera.transform.position, activatorPos );
			var size = 0.05f / distance / (2.0f * Mathf.Tan( (activeView.camera.fieldOfView / 2.0f) * Mathf.Deg2Rad )) * 1000.0f;

			Handles.DrawAAPolyLine( size, activatorPos, to );

			for ( float i = 0; i < Vector3.Distance( activatorPos, to ); i++ )
			{
				Handles.CircleHandleCap( 0, activatorPos - direction * i, /* activatorPos + (direction * (i - 0.15f)) +  */Quaternion.FromToRotation( activatorPos, to ).normalized, 0.5f, EventType.Repaint );
			}

			Handles.color = Color.white;
		}
	}
}
