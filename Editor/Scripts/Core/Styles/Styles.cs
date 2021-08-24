using UnityEngine;
using UnityEditor;

namespace Intruder.Tools
{
	public static class Styles
	{
		// I didn't realise how good changing one style would make the window look... use GUI.skin.window!
		public static readonly GUIStyle Panel = new GUIStyle( /* EditorStyles.helpBox  */ GUI.skin.window )
		{
			margin = new RectOffset( 8, 8, 8, 8 ),
			padding = new RectOffset( 8, 8, 8, 8 )
		};

		public static readonly GUIStyle Title = new GUIStyle( "Label" )
		{
			fontSize = 24,
			richText = true
		};

		public static readonly GUIStyle SubTitle = new GUIStyle( "Label" )
		{
			fontSize = 18,
			richText = true
		};

		public static readonly GUIStyle FoldoutSubTitle = new GUIStyle( EditorStyles.foldout )
		{
			fontSize = 18,
			richText = true
		};

		public static readonly GUIStyle Text = new GUIStyle( EditorStyles.label )
		{
			richText = true,
			alignment = TextAnchor.MiddleRight
		};

		public static readonly GUIStyle Image = new GUIStyle( EditorStyles.label )
		{
			margin = new RectOffset( 8, 8, 8, 8 ),
			padding = new RectOffset( 8, 8, 8, 8 )
		};


		public static readonly GUIStyle ToolsButton = new GUIStyle( /* EditorGUIUtility.isProSkin ? EditorStyles.whiteLabel : */ EditorStyles.label )
		{
			imagePosition = ImagePosition.ImageAbove,
			margin = new RectOffset( 4, 4, 4, 4 ),
			padding = new RectOffset( 4, 4, 8, 8 ),
			fontStyle = FontStyle.Bold,
			fixedHeight = 0,
			fixedWidth = 0,
			wordWrap = true,
			alignment = TextAnchor.MiddleCenter
		};

		public static readonly GUIStyle ToolsImageButton = new GUIStyle( ToolsButton )
		{
			fontSize = 12,
			imagePosition = ImagePosition.ImageOnly,
			padding = new RectOffset( 12, 12, 12, 12 ),
		};

		public static readonly GUIStyle Underline = new GUIStyle( GUI.skin.horizontalSlider )
		{
			margin = new RectOffset( 8, 8, 0, 16 ),
		};

		public static readonly GUIStyle TextField = new GUIStyle( EditorStyles.textField )
		{
			richText = true,
			alignment = TextAnchor.MiddleLeft,
			fontSize = 12,
			fontStyle = FontStyle.Bold,
			padding = new RectOffset( 8, 0, 0, 0 )
		};

		public static readonly GUIStyle TextFieldGhost = new GUIStyle( "Label" )
		{
			richText = true,
			alignment = TextAnchor.MiddleLeft,
			fontSize = 12,
			fontStyle = FontStyle.Bold,
			padding = new RectOffset( 8, 0, 0, 0 )
		};

		public static readonly GUIStyle TextArea = new GUIStyle( EditorStyles.textField )
		{
			richText = true,
			alignment = TextAnchor.UpperLeft,
			fontSize = 12,
			wordWrap = true,
			fontStyle = FontStyle.Bold,
			padding = new RectOffset( 8, 8, 8, 8 )
		};

		public static readonly GUIStyle TextAreaGhost = new GUIStyle( "Label" )
		{
			richText = true,
			alignment = TextAnchor.UpperLeft,
			fontSize = 12,
			wordWrap = true,
			fontStyle = FontStyle.Bold,
			padding = new RectOffset( 8, 8, 8, 8 )
		};
	}
}
