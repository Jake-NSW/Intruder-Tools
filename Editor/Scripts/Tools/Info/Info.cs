using UnityEngine;
using UnityEditor;

namespace Intruder.Tools
{
	[CustomTool( "Tools Info", Title = "Intruder Tools", Icon = "Assets/Intruder-Tools/Editor/Materials/Editor/Tools/help_icon.png", Priority = -500 )]
	public sealed class Info : Tool
	{
		private static bool headerInfoFoldout = true;
		private static bool helpFoldout = true;
		private static bool linksFoldout = true;

		public override void InspectorGUI()
		{
			InfoGUI();
		}

		private void InfoGUI()
		{
			// Workshop Statistics
			using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
			{
				// Foldout group
				headerInfoFoldout = EditorGUILayout.BeginFoldoutHeaderGroup( headerInfoFoldout, " What are the Intruder Tools?", Styles.FoldoutSubTitle );
				EditorGUILayout.EndFoldoutHeaderGroup();

				if ( headerInfoFoldout )
				{
					GUILayout.BeginVertical( Styles.Panel );
					{
						GUILayout.Label( "Here you will find all your tools for creating content for Intruder\n- If you need any help be sure to click the buttons below\n- Wanna show off cool stuff you've made? Come join the Discord server", new GUIStyle( "Label" ) { wordWrap = true }, GUILayout.ExpandHeight( true ), GUILayout.ExpandWidth( true ) );
					}
					GUILayout.EndVertical();
				}
			}

			using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
			{
				// Foldout group
				helpFoldout = EditorGUILayout.BeginFoldoutHeaderGroup( helpFoldout, " Get Help", Styles.FoldoutSubTitle );
				EditorGUILayout.EndFoldoutHeaderGroup();

				if ( helpFoldout )
				{
					GUILayout.BeginVertical( Styles.Panel );
					{
						if ( GUILayout.Button( "Intruder Website", GUILayout.Height( 32 ) ) )
						{
							Application.OpenURL( "https://intruderfps.com" );
						}

						if ( GUILayout.Button( "Intruder Community Discord", GUILayout.Height( 32 ) ) )
						{
							Application.OpenURL( "https://discord.gg/superbossgames" );
						}

						if ( GUILayout.Button( "Intruder Workshop Tools Discord", GUILayout.Height( 32 ) ) )
						{
							Application.OpenURL( "https://discord.gg/BV4mMC3mMu" );
						}

						if ( GUILayout.Button( "Steam Guides", GUILayout.Height( 32 ) ) )
						{
							Application.OpenURL( "https://steamcommunity.com/app/518150/guides/?searchText=&browsefilter=trend&browsesort=creationorder&requiredtags%5B%5D=Modding+or+Configuration&requiredtags%5B%5D=English#scrollTop=0" );
						}
					}
					GUILayout.EndVertical();
				}
			}

			using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
			{
				// Foldout group
				linksFoldout = EditorGUILayout.BeginFoldoutHeaderGroup( linksFoldout, " Steam Links", Styles.FoldoutSubTitle );
				EditorGUILayout.EndFoldoutHeaderGroup();

				if ( linksFoldout )
				{
					GUILayout.BeginVertical( Styles.Panel );
					{
						if ( GUILayout.Button( "Community Hub", GUILayout.Height( 32 ) ) )
						{
							Application.OpenURL( "https://steamcommunity.com/app/518150/" );
						}

						if ( GUILayout.Button( "Workshop", GUILayout.Height( 32 ) ) )
						{
							Application.OpenURL( "https://steamcommunity.com/app/518150/workshop/" );
						}
					}
					GUILayout.EndVertical();
				}
			}
		}
	}
}
