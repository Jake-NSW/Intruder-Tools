//-------------------------------------------------------------//
// Intruder Tools - Created by Superboss Games.
// Copyright Superboss Games 2021.
//-------------------------------------------------------------//

using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Intruder.Tools
{
	[CustomTool( "Map Compiler", Title = "Map Compiler", Description = "This tool allows you to export maps, quickly make new maps and more.", Tooltip = "Make Custom Maps", HasOptions = true )]
	public class MapCompiler : Compiler
	{
		//-------------------------------------------------------------//
		// Compiler Inspector GUI
		//-------------------------------------------------------------//
		private bool activeSceneCompileGroup = true;
		private bool advancedCompileGroup = false;

		public override void InspectorGUI()
		{
			// Quick Compile
			using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
			{
				// Foldout group
				activeSceneCompileGroup = EditorGUILayout.BeginFoldoutHeaderGroup( activeSceneCompileGroup, " Quick Compiler", Styles.FoldoutSubTitle );
				EditorGUILayout.EndFoldoutHeaderGroup();

				if ( activeSceneCompileGroup )
					QuickCompilerGUI();
			}

			// Advanced Compile
			using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
			{
				// Foldout group
				advancedCompileGroup = EditorGUILayout.BeginFoldoutHeaderGroup( advancedCompileGroup, " Advanced Compiler", Styles.FoldoutSubTitle );
				EditorGUILayout.EndFoldoutHeaderGroup();

				if ( advancedCompileGroup )
					GUILayout.Label( "soon" );
			}
		}

		private void QuickCompilerGUI()
		{
			using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
			{
				if ( GUILayout.Button( "Compile Active Scene" ) )
					CompileOpenScene();
			}
		}

		//-------------------------------------------------------------//
		// Options GUI
		//-------------------------------------------------------------//
		public override void OptionsGUI()
		{
			base.OptionsGUI();

			GUILayout.Label( "Awesome" );
		}

		//-------------------------------------------------------------//
		// Menu Items
		//-------------------------------------------------------------//
		[MenuItem( "Intruder Tools/Compile Open Scene _%i" )]
		public static void CompileOpenScene()
		{
			GetBuildTarget( SystemInfo.operatingSystemFamily.ToString(), out var buildTarget );
			CompileLevel( EditorSceneManager.GetActiveScene(), buildTarget );
		}

		//-------------------------------------------------------------//
		// Compiler
		//-------------------------------------------------------------//
		public static bool CompileLevel( Scene scene, BuildTarget[] buildTargets, Action<string> postCompile = null )
		{
			// Ask the user if they want to save the scene, if not don't export!
			if ( !EditorSceneManager.SaveModifiedScenesIfUserWantsTo( new Scene[] { scene } ) )
				return false;

			// Copies the scene to Level01 cause of intruder stupid shit
			EditorSceneManager.SaveScene( scene, "Assets/Level1.unity", true );

			// Give the level01 asset
			var level = AssetImporter.GetAtPath( "Assets/Level1.unity" );

			// Check if the dir is there and export level
			FileUtility.DirectoryCheck( $"Exports/Maps/{scene.name}/" );

			// For each target build, build
			foreach ( var item in buildTargets )
			{
				// Clear asset bundles so it doesnt build with map
				// IntruderUtility.ClearAllAssetBundleNames();

				level.assetBundleName = "map.ilf" + (item == BuildTarget.StandaloneWindows64 ? "w" : "m");
				BuildPipeline.BuildAssetBundles( $"Exports/Maps/{scene.name}/", BuildAssetBundleOptions.ChunkBasedCompression, item );

				// Clear asset bundles again so they cant stuff up next export somehow
				// IntruderUtility.ClearAllAssetBundleNames();
			}

			// Delete crap files
			FileUtility.DeleteAllFilesWithExtensionAtPath( $"Exports/Maps/{scene.name}/", "manifest" );
			FileUtility.DeleteAllFilesWithExtensionAtPath( $"Exports/Maps/{scene.name}/", "" );

			File.Delete( Path.GetFullPath( "Assets/Level1.unity" ) );
			AssetDatabase.Refresh();

			// I have to do this for some dumbass reason?
			Steam.GetAndCacheAvatar();
			postCompile?.Invoke( $"Exports/Maps/{scene.name}/map.ilf" );

			return true;
		}

		public static void GetBuildTarget( string target, out BuildTarget[] targets )
		{
			// Tf why I gotta do this?!
			targets = new BuildTarget[] { BuildTarget.NoTarget };

			// I would switch this to a dictionary but no point, this is easier for now
			switch ( target )
			{
				case ("OSX"):
				case ("MacOSX"):
					targets = new BuildTarget[] { BuildTarget.StandaloneOSX };
					break;

				case ("Windows"):
					targets = new BuildTarget[] { BuildTarget.StandaloneWindows64 };
					break;

				case ("Both"):
					targets = new BuildTarget[] { BuildTarget.StandaloneWindows64, BuildTarget.StandaloneOSX };
					break;

				default:
					target = null;
					break;
			}
		}
	}
}
