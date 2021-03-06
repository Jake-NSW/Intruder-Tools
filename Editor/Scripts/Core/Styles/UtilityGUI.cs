using UnityEngine;
using UnityEditor;

namespace Intruder.Tools.IMGUI
{
	public static class UtilityGUI
	{
		public static void GhostedTextField( ref string text, bool allowSpaces = true, string ghostedText = "GameObject", float fontSize = 12, params GUILayoutOption[] options )
		{
			text = GhostedTextField( text, allowSpaces, ghostedText, fontSize, options );
		}

		public static string GhostedTextField( string text, bool allowSpaces = true, string ghostedText = "GameObject", float fontSize = 12, params GUILayoutOption[] options )
		{
			EditorGUI.BeginChangeCheck();
			var textProcess = EditorGUILayout.TextField( text, Styles.TextField, options );

			// Remove Spaces from text name
			if ( EditorGUI.EndChangeCheck() && !allowSpaces )
				textProcess = textProcess.RemoveSpaces();

			if ( string.IsNullOrEmpty( text ) )
				GUI.Label( GUILayoutUtility.GetLastRect(), $"<color=grey>{ghostedText}</color>", Styles.TextFieldGhost );

			return textProcess;
		}

		public static void GhostedTextArea( ref string text, bool allowSpaces = true, string ghostedText = "GameObject", float fontSize = 12, params GUILayoutOption[] options )
		{
			text = GhostedTextArea( text, allowSpaces, ghostedText, fontSize, options );
		}

		public static void FoldoutPanel( ref bool foldoutValue, in string panelName = default( string ), params System.Action[] onGUI )
		{
			using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
			{
				foldoutValue = EditorGUILayout.Foldout( foldoutValue, $" {panelName}", true, Styles.FoldoutSubTitle );

				if ( foldoutValue && onGUI != null )
				{
					foreach ( var item in onGUI )
					{
						Panel( item );
					}
				}
			}
		}

		public static void Panel( System.Action onGUI = null, params GUILayoutOption[] options )
		{
			using ( new GUILayout.VerticalScope( Styles.Panel, options ) )
			{
				onGUI?.Invoke();
			}
		}

		public static string GhostedTextArea( string text, bool allowSpaces = true, string ghostedText = "GameObject", float fontSize = 12, params GUILayoutOption[] options )
		{
			EditorGUI.BeginChangeCheck();
			var textProcess = EditorGUILayout.TextArea( text, Styles.TextArea, options );

			// Remove Spaces from text name
			if ( EditorGUI.EndChangeCheck() && !allowSpaces )
				textProcess = textProcess.RemoveSpaces();

			if ( string.IsNullOrEmpty( text ) )
				GUI.Label( GUILayoutUtility.GetLastRect(), $"<color=grey>{ghostedText}</color>", Styles.TextAreaGhost );

			return textProcess;
		}

		public static void UnderlineGUI( float opacity = 0.4f, params GUILayoutOption[] options )
		{
			GUI.color = new Color( 1f, 1f, 1f, opacity );
			GUILayout.Label( "", Styles.Underline, options );
			GUI.color = Color.white;
		}

		public static void PanelName( string name )
		{
			UnderlineGUI( 0.2f );
			GUILayout.Label( name, EditorStyles.centeredGreyMiniLabel );
		}
	}
}
