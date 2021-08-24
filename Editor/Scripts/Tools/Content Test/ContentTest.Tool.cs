using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Steamworks;

namespace Intruder.Tools.Testing
{
	[CustomTool( "Content Tester", Title = "Content Testing", Description = "Test maps, skins and launch Intruder", Priority = 0 )]
	public class ContentTestTool : Tool
	{
		private bool quickCommandsFoldout = true;
		private bool mapTestingFoldout = false;

		public override void InspectorGUI()
		{
			GUILayout.Label( "Install Path:" );
			EditorGUI.BeginDisabledGroup( true );
			{
				EditorGUILayout.TextField( SteamApps.AppInstallDir() );
			}
			EditorGUI.EndDisabledGroup();

			// Quick Commands
			using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
			{
				// Foldout group
				quickCommandsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup( quickCommandsFoldout, " Quick Commands", Styles.FoldoutSubTitle );
				EditorGUILayout.EndFoldoutHeaderGroup();

				if ( quickCommandsFoldout )
					QuickCommandsGUI();
			}

			// Map Testing
			using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
			{
				// Foldout group
				mapTestingFoldout = EditorGUILayout.BeginFoldoutHeaderGroup( mapTestingFoldout, " Map Testing", Styles.FoldoutSubTitle );
				EditorGUILayout.EndFoldoutHeaderGroup();

				if ( mapTestingFoldout )
					MapTestingGUI();
			}
		}

		//-------------------------------------------------------------//
		// Quick Commands GUI
		//-------------------------------------------------------------//
		private void QuickCommandsGUI()
		{
			if ( GUILayout.Button( "Quick Launch Intruder", GUILayout.Height( 24 ) ) )
			{
				ContentTest.LaunchIntruder();
			}
		}

		//-------------------------------------------------------------//
		// Map Testing GUI
		//-------------------------------------------------------------//
		private void MapTestingGUI()
		{
			GUILayout.Label( "Awesome" );
		}
	}
}
