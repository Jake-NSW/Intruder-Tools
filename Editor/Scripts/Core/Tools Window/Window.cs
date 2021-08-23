//-------------------------------------------------------------//
// Intruder Tools - Created by Superboss Games.
// Copyright Superboss Games 2021.
//-------------------------------------------------------------//

using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Intruder.Tools
{
	public class Window : EditorWindow
	{
		public static Window current;
		public static readonly GUIContent WindowTitle = new GUIContent( "Intruder Tools" );

		//-------------------------------------------------------------//
		// Entry Point
		//-------------------------------------------------------------//
		[MenuItem( "Intruder Tools/Tools Window" )]
		public static void OpenWindow()
		{
			var window = EditorWindow.GetWindow<Window>();
			window.titleContent = WindowTitle;
			window.minSize = new Vector2( 465, 350 );
			window.Show();
			current = window;
		}

		//-------------------------------------------------------------//
		// Window States
		//-------------------------------------------------------------//
		[InitializeOnLoadMethod]
		private static void Initialize()
		{
			Icons.Precache();
			GrabTools();
		}

		//-------------------------------------------------------------//
		// Tools Grabber / Creator
		//-------------------------------------------------------------//
		public static Tool activeTool;
		private static Preferences preferencesTool = new Preferences();
		private static List<Tool> cachedTools = new List<Tool>();

		private static void GrabTools()
		{
			cachedTools.Clear();

			// Get types that have CustomTool and Subclass of Tool
			var uninitializedTools = AppDomain.CurrentDomain.GetAssemblies()
												.SelectMany( assembly => assembly.GetTypes() )
												.Where( type => type.IsDefined( typeof( CustomToolAttribute ) ) && type.IsSubclassOf( typeof( Tool ) ) );

			// Go through each uninitializedTool and create it
			foreach ( var item in uninitializedTools )
			{
				// Get the attribute + create instance of tool
				var customToolAttribute = item.GetCustomAttribute<CustomToolAttribute>();
				var newTool = Activator.CreateInstance( item ) as Tool;

				newTool.Name = customToolAttribute.Name;
				newTool.Priority = customToolAttribute.Priority;
				newTool.Title = String.IsNullOrEmpty( customToolAttribute.Title ) ? customToolAttribute.Name : customToolAttribute.Title;
				newTool.Description = customToolAttribute.Description;
				newTool.HasOptions = customToolAttribute.HasOptions;

				// Load icon if it is there - Use dark or light icon if attribute property for it is assigned
				if ( !string.IsNullOrEmpty( customToolAttribute.Icon ) && (string.IsNullOrEmpty( customToolAttribute.LightThemeIcon ) && string.IsNullOrEmpty( customToolAttribute.DarkThemeIcon )) )
					newTool.Icon = AssetDatabase.LoadAssetAtPath<Texture2D>( customToolAttribute.Icon );
				else
					newTool.Icon = AssetDatabase.LoadAssetAtPath<Texture2D>( EditorGUIUtility.isProSkin ? customToolAttribute.DarkThemeIcon : customToolAttribute.LightThemeIcon );

				newTool.Tooltip = customToolAttribute.Tooltip;

				// Add to the list of tools 
				cachedTools.Add( newTool );
			}

			cachedTools = cachedTools.OrderBy( e => e.Priority ).ToList();
			activeTool = cachedTools[0];
		}

		//-------------------------------------------------------------//
		// Options Creator
		//-------------------------------------------------------------//


		//-------------------------------------------------------------//
		// GUI
		//-------------------------------------------------------------//
		public static class Icons
		{
			public static void Precache()
			{
				IntruderBanner = AssetDatabase.LoadAssetAtPath<Texture2D>( AssetDatabase.GUIDToAssetPath( "35dc69cfb37103c45b283015925ed9bd" ) );
				DefaultThumbnail = AssetDatabase.LoadAssetAtPath<Texture2D>( AssetDatabase.GUIDToAssetPath( "37dc8cac11054e642b8bebb42ec7a17c" ) );
			}

			public static Texture2D IntruderBanner;
			public static Texture2D DefaultThumbnail;
		}

		private void OnGUI()
		{
			UserCardGUI();
			BodyGUI();
		}

		private void UserCardGUI()
		{
			using ( new GUILayout.HorizontalScope( Styles.Panel, GUILayout.Height( 64 ) ) )
			{
				GUILayout.Label( Icons.IntruderBanner, Styles.Image, GUILayout.ExpandHeight( true ), GUILayout.Height( 64 ), GUILayout.Width( 220 ) );
				GUILayout.FlexibleSpace();
				GUILayout.Label( $"<size=14><b>{Local.Username}</b></size>\nFollowers: {Local.Followers}", Styles.Text, GUILayout.ExpandHeight( true ) );
				if ( GUILayout.Button( Local.Avatar, GUILayout.Height( 64 ), GUILayout.Width( 64 ) ) )
				{
					Application.OpenURL( $"steam://openurl/https://steamcommunity.com/profiles/{Local.SteamId}/myworkshopfiles/?appid=518150" );
				}
			}
		}

		private Vector2 toolsScrollPos;
		private void BodyGUI()
		{
			using ( new GUILayout.HorizontalScope( GUILayout.ExpandHeight( true ) ) )
			{
				// Tools Panel
				using ( new GUILayout.VerticalScope( new GUIStyle( Styles.Panel ) { padding = new RectOffset( 0, 0, 8, 8 ) }, GUILayout.Width( 64 ), GUILayout.ExpandHeight( true ) ) )
				{
					// Go through each tool and create button
					toolsScrollPos = GUILayout.BeginScrollView( toolsScrollPos, GUIStyle.none, GUILayout.ExpandWidth( true ) );
					{
						foreach ( var tool in cachedTools )
						{
							Toolbutton( tool );
						}
					}
					GUILayout.EndScrollView();

					// Bottom
					GUILayout.FlexibleSpace();

					// On options button, set active tool to options... Feels hacky?
					/* if ( GUILayout.Button( "Options", Styles.ToolsButton, GUILayout.MinWidth( 64 ) ) )
						activeTool = preferencesTool; */

					Toolbutton( preferencesTool, 32 );

					ToolGUI.PanelName( "Tools" );
				}

				// Inspector + Help Box
				using ( new GUILayout.VerticalScope( GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) ) )
				{
					// Get active tool and make inspector
					using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) ) )
					{
						activeTool.InternalInspectorGUI();
						ToolGUI.PanelName( "Inspector" );
					}

					// If tool has no help, don't draw
					if ( !string.IsNullOrEmpty( activeTool.Description ) )
					{
						// Help Box
						using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandWidth( true ), GUILayout.Height( 64 ) ) )
						{
							GUILayout.Label( activeTool.Description, EditorStyles.wordWrappedLabel );
							GUILayout.FlexibleSpace();
							ToolGUI.PanelName( "Help" );
						}
					}
				}
			}
		}

		private void Toolbutton( Tool tool, float height = 64 )
		{
			var isActiveTool = tool == activeTool;

			if ( !isActiveTool )
				GUI.color = new Color( 1f, 1f, 1f, 0.5f );

			if ( GUILayout.Button( new GUIContent( tool.Name, tool.Icon, tool.Tooltip ), tool.Icon ? Styles.ToolsImageButton : Styles.ToolsButton, GUILayout.Height( height ), GUILayout.MinWidth( 64 ) ) )
			{
				activeTool = tool;
				tool.OnSelect();
			}

			if ( !isActiveTool )
				GUI.color = new Color( 1, 1, 1, 1 );

		}

		//-------------------------------------------------------------//
		// Preferences / Options
		// - Created the preferences in this,
		// - feels hacky but doing it so it can be a hardcoded tool
		//-------------------------------------------------------------//
		private class Preferences : Tool
		{
			public Preferences()
			{
				Name = "Options";
				Title = "Tool Options";
				Description = "Edit tool preferences";
			}

			public override void InspectorGUI()
			{
				foreach ( var item in cachedTools )
				{
					item.InternalOptionsGUI();
				}
			}
		}
	}
}

