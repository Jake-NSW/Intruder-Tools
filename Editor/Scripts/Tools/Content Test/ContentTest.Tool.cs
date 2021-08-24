using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
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
			using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
			{
				GUILayout.Label( "Install Path:" );
				EditorGUI.BeginDisabledGroup( true );
				{
					EditorGUILayout.TextField( SteamApps.AppInstallDir() );
				}
				EditorGUI.EndDisabledGroup();
			}

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
			using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
			{
				if ( GUILayout.Button( "Quick Launch Intruder", GUILayout.Height( 24 ) ) )
				{
					ContentTest.LaunchIntruder();
				}

				if ( GUILayout.Button( "Goto Intruder Install Dir", GUILayout.Height( 24 ) ) )
				{
					ContentTest.GoToInstallPath();
				}
			}

			using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
			{
				var newScene = string.IsNullOrEmpty( EditorSceneManager.GetActiveScene().name );
				EditorGUI.BeginDisabledGroup( newScene );
				{
					if ( GUILayout.Button( new GUIContent( "Open Scene In Game", newScene ? "Must save the scene" : "" ), GUILayout.Height( 24 ) ) )
					{
						if ( Directory.Exists( Path.GetFullPath( $"Exports/Maps/{EditorSceneManager.GetActiveScene().name}/" ) ) )
							ContentTest.LaunchIntruder( ContentTest.LoadLevelArgs( $"Exports/Maps/{EditorSceneManager.GetActiveScene().name}/" ) );
						else
							EditorUtility.DisplayDialog( "Load Level Error", "This scene hasn't been compiled yet", "Okay" );
					}
				}
				EditorGUI.EndDisabledGroup();
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
