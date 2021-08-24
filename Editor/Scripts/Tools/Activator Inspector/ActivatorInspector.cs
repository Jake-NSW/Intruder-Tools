using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Intruder.Tools
{
	[CustomTool( "Activator Inspector", Description = "See what your activators are doing!", Priority = 100 )]
	public class ActivatorInspector : Tool
	{
		public override void InspectorGUI()
		{
			GUILayout.Label( "Coming Soon" );
		}
	}
}
