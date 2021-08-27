//-------------------------------------------------------------//
// Intruder Tools - Created by Superboss Games.
// Copyright Superboss Games 2021.
//-------------------------------------------------------------//

using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using Intruder.Tools.Steamworks;
using Intruder.Tools.Testing;

namespace Intruder.Tools.Compiling
{
	[CustomTool( "Map Compiler", Title = "Map Compiler", Description = "This tool allows you to export maps, quickly make new maps and more.", Tooltip = "Make Custom Maps" )]
	public class MapCompiler : Compiler
	{
		//-------------------------------------------------------------//
		// Compiler Inspector GUI
		//-------------------------------------------------------------//
		private bool activeSceneCompileGroup = true;
		private bool advancedCompileGroup = false;

		private static SceneAsset cachedMap;

		private static string[] buildTargets = new string[] { "Windows", "OSX", "Both" };
		private static int buildTargetIndex;
		private static bool cachedBeep;
		private static bool cachedSanityCheck = true;
		private static bool cachedLoadInGame;

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
				advancedCompileGroup = EditorGUILayout.BeginFoldoutHeaderGroup( advancedCompileGroup, " Compiler", Styles.FoldoutSubTitle );
				EditorGUILayout.EndFoldoutHeaderGroup();

				if ( advancedCompileGroup )
					CompilerGUI();
			}
		}

		private void QuickCompilerGUI()
		{
			using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
			{
				if ( GUILayout.Button( "Compile and Test Active Scene", GUILayout.Height( 24 ) ) )
					CompileAndLaunchOpenScene();

				if ( GUILayout.Button( "Compile Active Scene", GUILayout.Height( 24 ) ) )
					CompileOpenScene();
			}
		}

		private void CompilerGUI()
		{
			GUILayout.BeginVertical( Styles.Panel );
			{
				GUILayout.Label( "Map To Export", Styles.SubTitle );
				GUILayout.Space( 4 );
				GUILayout.BeginHorizontal();
				{
					cachedMap = EditorGUILayout.ObjectField( cachedMap, typeof( SceneAsset ), false, GUILayout.Height( 32 ) ) as SceneAsset;
					if ( GUILayout.Button( "Get Open Scene", GUILayout.Width( 128 ), GUILayout.Height( 32 ) ) )
					{
						cachedMap = AssetDatabase.LoadAssetAtPath<SceneAsset>( UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path );
					}
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();

			GUILayout.BeginVertical( Styles.Panel );
			{
				GUILayout.Label( "Map Options", Styles.SubTitle );
				GUILayout.Space( 4 );
				EditorGUILayout.Toggle( new GUIContent( "Export Post Effects", "Disable this if compile times are slow" ), true );
				cachedSanityCheck = EditorGUILayout.Toggle( "Do Sanity Check", cachedSanityCheck );

				EditorGUILayout.Space();

				buildTargetIndex = EditorGUILayout.Popup( "Build Target", buildTargetIndex, buildTargets );
			}
			GUILayout.EndVertical();

			GUILayout.BeginVertical( Styles.Panel );
			{
				GUILayout.Label( "On Build Complete", Styles.SubTitle );
				GUILayout.Space( 4 );
				cachedBeep = EditorGUILayout.Toggle( "Beep on Finish", cachedBeep );
				cachedLoadInGame = EditorGUILayout.Toggle( "Launch Map In Game", cachedLoadInGame );
			}
			GUILayout.EndVertical();

			GUILayout.BeginVertical( Styles.Panel );
			{
				if ( cachedMap == null )
					EditorGUILayout.HelpBox( "Need to add a map to compile!", MessageType.Error );

				EditorGUI.BeginDisabledGroup( cachedMap == null );
				{
					if ( GUILayout.Button( new GUIContent( "Compile Map", EditorGUIUtility.IconContent( "d_Settings" ).image ), GUILayout.Height( 32 ) ) )
					{
						Action<string> postBuild = ( path ) => { };

						if ( cachedBeep )
							postBuild += ( path ) => EditorApplication.Beep();
						if ( cachedLoadInGame )
							postBuild += ( path ) => ContentTest.LaunchIntruder( ContentTest.LoadLevelArgs( path ) );

						GetBuildTarget( buildTargets[buildTargetIndex], out var target );
						CompileLevel( UnityEngine.SceneManagement.SceneManager.GetSceneByPath( AssetDatabase.GetAssetPath( cachedMap ) ), target, cachedSanityCheck, postBuild );
					}
				}
				EditorGUI.EndDisabledGroup();
			}
			GUILayout.EndVertical();
		}

		//-------------------------------------------------------------//
		// Menu Items
		//-------------------------------------------------------------//
		[MenuItem( "Intruder Tools/Maps/Compile and Launch Map _%i", priority = -5 )]
		public static void CompileAndLaunchOpenScene()
		{
			GetBuildTarget( SystemInfo.operatingSystemFamily.ToString(), out var buildTarget );
			CompileLevel( EditorSceneManager.GetActiveScene(), buildTarget, true, ( path ) => ContentTest.LaunchIntruder( ContentTest.LoadLevelArgs( path ) ) );
		}

		[MenuItem( "Intruder Tools/Maps/Compile Map", priority = 5 )]
		public static void CompileOpenScene()
		{
			GetBuildTarget( SystemInfo.operatingSystemFamily.ToString(), out var buildTarget );
			CompileLevel( EditorSceneManager.GetActiveScene(), buildTarget );
		}

		//-------------------------------------------------------------//
		// Compiler
		//-------------------------------------------------------------//
		public static bool CompileLevel( Scene scene, BuildTarget[] buildTargets, bool sanityCheck = true, Action<string> postCompile = null )
		{
			var backupScenePath = scene.path;

			// Ask the user if they want to save the scene, if not don't export!
			if ( !EditorSceneManager.SaveModifiedScenesIfUserWantsTo( new Scene[] { scene } ) )
				return false;

			if ( sanityCheck )
				DoSanityCheck( scene );

			// Copies the scene to Level01 cause of intruder stupid shit
			EditorSceneManager.SaveScene( scene, "Assets/Level1.unity", true );

			// Give the level01 asset
			var level = AssetImporter.GetAtPath( "Assets/Level1.unity" );

			// Check if the dir is there and export level
			FileUtility.DirectoryCheck( $"Exports/Maps/{scene.name}/" );

			// For each target build, build
			foreach ( var target in buildTargets )
			{
				// Clear asset bundles so it doesnt build with map
				// IntruderUtility.ClearAllAssetBundleNames();

				level.assetBundleName = "map.ilf" + (target == BuildTarget.StandaloneWindows64 ? "w" : "m");
				var bundle = BuildPipeline.BuildAssetBundles( $"Exports/Maps/{scene.name}/", BuildAssetBundleOptions.ChunkBasedCompression, target );

				if ( bundle == null )
				{
					EditorUtility.DisplayDialog( "ERROR", $"Map asset bundle compile failed. {target.ToString()}", "Okay" );
					Debug.LogError( "Compile Failed" );
				}

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
			postCompile?.Invoke( $"Exports/Maps/{scene.name}/" );

			EditorUtility.DisplayDialog( "Compile Finish", $"{scene.name} has finished compiling.", "Okay" );

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

		public static void DoSanityCheck( Scene scene )
		{
			var gameObjects = scene.GetRootGameObjects();

			foreach ( var gameObject in gameObjects )
			{
				if ( gameObject.TryGetComponent<Camera>( out var camera ) && gameObject.GetComponent<ObserveCamProxy>() == null )
				{
					if ( camera.targetTexture != null )
					{
						Debug.LogWarning( $"Disabling Camera - {camera.name}" );
						camera.enabled = false;
					}
				}

				if ( gameObject.TryGetComponent<AudioListener>( out var listener ) )
				{
					Debug.LogWarning( $"Disabling AudioListener - {listener.name}" );
					listener.enabled = false;
				}
			}
		}
	}
}
