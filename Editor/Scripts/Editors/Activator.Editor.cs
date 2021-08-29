using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Intruder.Tools.IMGUI;

namespace Intruder.Tools.Editors
{
	[CustomEditor( typeof( Activator ) )]
	public class ActivatorEditor : Editor
	{
		private Activator Activator => target as Activator;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}

		private void OnSceneGUI()
		{
			SceneGUI.UtilitySceneGUI.DrawActivatorLine( Activator, Activator.objectsToEnable[0].transform.position, Color.cyan );
		}
	}
}
