using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Steamworks;

namespace Intruder.Tools
{
	[CustomTool( "Content Tester", Title = "Content Testing", Description = "Test maps, skins and launch Intruder", Priority = 0 )]
	public class ContentTest : Tool
	{
		public override void InspectorGUI()
		{
			GUILayout.Label( "Install Path:" );
			EditorGUI.BeginDisabledGroup( true );
			{
				EditorGUILayout.TextField( SteamApps.AppInstallDir() );
			}
			EditorGUI.EndDisabledGroup();
		}
	}
}
