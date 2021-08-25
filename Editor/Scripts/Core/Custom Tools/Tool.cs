//-------------------------------------------------------------//
// Intruder Tools - Created by Superboss Games.
// Copyright Superboss Games 2021.
//-------------------------------------------------------------//

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Intruder.Tools
{
	public abstract class Tool
	{
		//-------------------------------------------------------------//
		// Tool Meta Data
		//-------------------------------------------------------------//
		public int Priority { get; set; }
		public string Name { get; set; }
		public Texture2D Icon { get; set; }
		public string Tooltip { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public bool HasOptions { get; set; }

		//-------------------------------------------------------------//
		// Tool GUI
		//-------------------------------------------------------------//
		private Vector2 scrollPos;
		internal void InternalInspectorGUI()
		{
			GUILayout.Label( Title, Styles.Title );
			UtilityGUI.UnderlineGUI( 1 );

			scrollPos = GUILayout.BeginScrollView( scrollPos, GUIStyle.none );
			{
				InspectorGUI();
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndScrollView();
		}

		private bool optionsFoldout;
		internal void InternalOptionsGUI()
		{
			if ( !HasOptions )
				return;

			using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
			{// Foldout group
				optionsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup( optionsFoldout, $" {Title}", Styles.FoldoutSubTitle );
				EditorGUILayout.EndFoldoutHeaderGroup();

				if ( optionsFoldout )
					OptionsGUI();
			}
		}

		public virtual void OnSelect() { }
		public virtual void InspectorGUI() { }
		public virtual void OptionsGUI() { }
	}
}
