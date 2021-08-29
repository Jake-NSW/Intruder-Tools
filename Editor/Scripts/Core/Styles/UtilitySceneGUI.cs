using UnityEngine;
using UnityEditor;

namespace Intruder.Tools.SceneGUI
{
	public static class UtilitySceneGUI
	{
		public static void DrawActivatorLine( Activator activator, Vector3 to, Color color = default( Color ), float width = 4 )
		{
			Handles.color = color;

			var activatorPos = activator.transform.position;
			var direction = (activatorPos - to).normalized;

			Handles.DrawLine( activatorPos, to, width );

			for ( int i = 3; i < Vector3.Distance( to, activatorPos ); i += 2 )
			{
				Handles.ArrowHandleCap( 0, to + (direction * i), Quaternion.LookRotation( -direction, direction ), 1, EventType.Repaint );
			}

			ThickCircle( to, SceneView.lastActiveSceneView.camera.transform.rotation, color, width, 3 );

			Handles.color = Color.white;
		}

		public static void ThickCircle( Vector3 pos, Quaternion rotation, Color color, float gap, float thickness = 2 )
		{
			for ( int i = 1; i < (thickness * 10); i++ )
			{
				Handles.CircleHandleCap( i, pos, rotation, (gap / 16) + (i * 0.0005f), EventType.Repaint );
			}
		}
	}
}
