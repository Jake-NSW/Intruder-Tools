using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Steamworks;
using Intruder.Tools.IMGUI;
using UnityEditor.IMGUI.Controls;
using Intruder.Tools.IMGUI.Controls;

namespace Intruder.Tools.Testing
{
	[CustomTool( "Content Tester", Title = "Content Testing", Description = "Test maps, skins and launch Intruder", Priority = 0 )]
	public sealed class ContentTestTool : Tool, IPackageDirectory
	{
		private bool quickCommandsFoldout = true;
		private bool packageTestingFoldout = false;
		private string cachedDirectory;

		public override void InspectorGUI()
		{
			// If steam is not connected.. Don't draw GUI
			if ( !SteamClient.IsValid )
			{
				GUILayout.Label( "Steam not connected!", Styles.SubTitle );
				return;
			}

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
				packageTestingFoldout = EditorGUILayout.BeginFoldoutHeaderGroup( packageTestingFoldout, " Package Testing", Styles.FoldoutSubTitle );
				EditorGUILayout.EndFoldoutHeaderGroup();

				if ( packageTestingFoldout )
					PackageTestingGUI();
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
		// Packing Testing GUI
		//-------------------------------------------------------------//
		private void PackageTestingGUI()
		{
			using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
			{
				var content = (string.IsNullOrEmpty( cachedDirectory )) ? new GUIContent( "No Package Selected" ) : new GUIContent( Path.GetFileName( cachedDirectory ) );
				var style = new GUIStyle( EditorStyles.popup ) { fixedHeight = 0 };

				var rect = GUILayoutUtility.GetRect( content, style, GUILayout.Height( 20 ) );
				if ( GUI.Button( rect, content, style ) )
				{
					var dropdown = new PackagesDropdown( new AdvancedDropdownState(), this );
					dropdown.Show( rect );
				}

				if ( string.IsNullOrEmpty( cachedDirectory ) )
					EditorGUILayout.HelpBox( "Need to have a selected package in the field above, in order to playtest an item", MessageType.Error );

				EditorGUI.BeginDisabledGroup( string.IsNullOrEmpty( cachedDirectory ) );
				{
					if ( GUILayout.Button( "Test Package", GUILayout.Height( 24 ) ) )
					{
						// For now just do map laod crap
						ContentTest.LaunchIntruder( ContentTest.LoadLevelArgs( cachedDirectory ) );
					}
				}
				EditorGUI.EndDisabledGroup();
			}
		}

		void IPackageDirectory.OnNothingSelected()
		{
			cachedDirectory = null;
		}

		void IPackageDirectory.OnSkinSelected( string path )
		{
			cachedDirectory = path;
		}

		void IPackageDirectory.OnMapSelected( string path )
		{
			cachedDirectory = path;
		}
	}
}
