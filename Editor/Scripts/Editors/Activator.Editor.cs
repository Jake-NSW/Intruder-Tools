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
		public override bool UseDefaultMargins() => false;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}
	}
}
