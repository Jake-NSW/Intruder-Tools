using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu( menuName = "Intruder/Skin" )]
public class CustomSkinAsset : ScriptableObject
{

}

[CustomEditor( typeof( CustomSkinAsset ) )]
public class CustomSkinAssetEditor : Editor
{
	public override bool HasPreviewGUI()
	{
		return true;
	}

	public override void OnPreviewGUI( Rect r, GUIStyle background )
	{
		GUI.Label( r, target.name + " is being previewed" );
	}
}
