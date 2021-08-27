using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Intruder.Tools
{
	public sealed class Initializer
	{
		[InitializeOnLoadMethod]
		private static void InstallCheck()
		{
			HasAllUnityBuildPlatforms();
		}

		public static bool HasAllUnityBuildPlatforms()
		{
			var moduleManager = System.Type.GetType( "UnityEditor.Modules.ModuleManager,UnityEditor.dll" );
			var isPlatformSupportLoaded = moduleManager.GetMethod( "IsPlatformSupportLoaded", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic );
			var getTargetStringFromBuildTarget = moduleManager.GetMethod( "GetTargetStringFromBuildTarget", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic );

			bool windowsLoaded = (bool)isPlatformSupportLoaded.Invoke( null, new object[]
			{
				(string) getTargetStringFromBuildTarget.Invoke(null, new object[] { BuildTarget.StandaloneWindows64 })
			} );

			bool osxLoaded = (bool)isPlatformSupportLoaded.Invoke( null, new object[]
			{
				(string) getTargetStringFromBuildTarget.Invoke(null, new object[] { BuildTarget.StandaloneOSX })
			} );

			if ( !(windowsLoaded && osxLoaded) )
			{
				string platformInstallError = "Error: Cannot build. Make sure both Windows and OSX build platforms are installed. Run Unity installer again to fix.";
				Debug.LogError( platformInstallError );
				EditorUtility.DisplayDialog( "Build error", platformInstallError, "Ok" );
			}

			return windowsLoaded && osxLoaded;
		}
	}
}
