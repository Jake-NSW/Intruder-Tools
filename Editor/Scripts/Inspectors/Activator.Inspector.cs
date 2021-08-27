using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Intruder.Tools.IMGUI;

namespace Intruder.Tools.Inspectors
{
	[System.Flags]
	public enum ActivatorTriggers
	{
		None = 0,
		OnUse = 1 << 0,
		OnShot = 2 << 1,
		OnEnter = 4 << 3,
		OnRagdollEnter = 5 << 4,
		OnExplosion = 6 << 5,
	}

	[CustomEditor( typeof( Activator ) )]
	public class ActivatorEditor : Editor
	{
		public Activator Activator => target as Activator;
		public Transform Transform => Activator.transform;
		private int currentIndex = 0;

		private ActivatorTriggers triggers { get; set; }

		public override bool UseDefaultMargins() => false;

		public override void OnInspectorGUI()
		{
			GUILayout.BeginVertical( new GUIStyle() { margin = new RectOffset( 8, 8, 8, 8 ) } );
			{
				ToolbarGUI();
				UtilityGUI.Panel( ActivatorGUI );
			}
			GUILayout.EndVertical();
		}

		private void OnEnable()
		{
			triggers = ParseTriggers();
		}

		private ActivatorTriggers ParseTriggers()
		{
			var builder = new System.Text.StringBuilder();

			if ( Activator.triggerByUse )
				builder.Append( ", OnUse " );
			if ( Activator.triggerByShoot )
				builder.Append( ", OnShot " );
			if ( Activator.triggerByEnter )
				builder.Append( ", OnEnter " );
			if ( Activator.triggerByRagdollEnter )
				builder.Append( ", OnRagdollEnter " );
			if ( Activator.triggerByExplosion )
				builder.Append( ", OnExplosion " );


			return (ActivatorTriggers)System.Enum.Parse( typeof( ActivatorTriggers ), builder.Remove( 0, 1 ).ToString());
		}

		private void OnSceneGUI()
		{
			ActivatorSceneGUI();
		}

		private void ToolbarGUI()
		{
			UtilityGUI.Panel( () => { currentIndex = GUILayout.Toolbar( currentIndex, new string[] { "Triggers", "Actions" }, GUILayout.Height( 24 ) ); } );
		}

		public void ActivatorGUI()
		{
			switch ( currentIndex )
			{
				case 0:
					TriggerGUI();
					break;
				case 1:
					ActionsGUI();
					break;
			}
		}

		private void TriggerGUI()
		{
			EditorGUI.BeginChangeCheck();
			{
				triggers = (ActivatorTriggers)EditorGUILayout.EnumFlagsField( triggers );
			}
			if ( EditorGUI.EndChangeCheck() )
			{
				// All this feels terrible
				if ( triggers.HasFlag( ActivatorTriggers.OnUse ) )
					serializedObject.FindProperty( "triggerByUse" ).boolValue = true;
				else
					serializedObject.FindProperty( "triggerByUse" ).boolValue = false;

				if ( triggers.HasFlag( ActivatorTriggers.OnShot ) )
					serializedObject.FindProperty( "triggerByShoot" ).boolValue = true;
				else
					serializedObject.FindProperty( "triggerByShoot" ).boolValue = false;

				if ( triggers.HasFlag( ActivatorTriggers.OnEnter ) )
					serializedObject.FindProperty( "triggerByEnter" ).boolValue = true;
				else
					serializedObject.FindProperty( "triggerByEnter" ).boolValue = false;

				if ( triggers.HasFlag( ActivatorTriggers.OnRagdollEnter ) )
					serializedObject.FindProperty( "triggerByRagdollEnter" ).boolValue = true;
				else
					serializedObject.FindProperty( "triggerByRagdollEnter" ).boolValue = false;

				if ( triggers.HasFlag( ActivatorTriggers.OnExplosion ) )
					serializedObject.FindProperty( "triggerByExplosion" ).boolValue = true;
				else
					serializedObject.FindProperty( "triggerByExplosion" ).boolValue = false;

				serializedObject.ApplyModifiedProperties();
			}
		}

		private void ActionsGUI()
		{
			GUILayout.Label( "Actions" );
		}

		public void ActivatorSceneGUI()
		{

		}
	}

}
