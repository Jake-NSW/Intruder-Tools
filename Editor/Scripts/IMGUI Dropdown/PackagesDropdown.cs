using System.IO;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Intruder.Tools.IMGUI
{
	public sealed class PackagesDropdown : AdvancedDropdown
	{
		private IPackageDirectory package;

		public PackagesDropdown( AdvancedDropdownState state, IPackageDirectory package ) : base( state )
		{
			this.package = package;
			this.minimumSize = new Vector2( 0, 200 );
		}

		protected override void ItemSelected( AdvancedDropdownItem item )
		{
			var projectPath = Path.GetFullPath( Application.dataPath + "/../" );

			switch ( item.id )
			{
				case 0:
					package.OnNothingSelected();
					break;
				// Item is map
				case 1:
					package.OnMapSelected( Path.GetFullPath( projectPath + "/Exports/Maps/" + item.name ) );
					break;

				// Item is Skin
				case 2:
					package.OnSkinSelected( Path.GetFullPath( projectPath + "/Exports/Skins/" + item.name ) );
					break;
			}
		}

		protected override AdvancedDropdownItem BuildRoot()
		{
			var root = new AdvancedDropdownItem( $"Select a Package" );
			var projectPath = Path.GetFullPath( Application.dataPath + "/../" );

			root.AddChild( new AdvancedDropdownItem( "None" ) { id = 0 } );
			root.AddSeparator();

			if ( Directory.Exists( projectPath + "/Exports/Maps" ) )
			{
				var maps = new AdvancedDropdownItem( "Map Packages" );

				foreach ( var item in Directory.GetDirectories( projectPath + $"/Exports/Maps" ) )
					maps.AddChild( new AdvancedDropdownItem( Path.GetFileName( item ) ) { id = 1 } );

				root.AddChild( maps );
			}

			if ( Directory.Exists( projectPath + "/Exports/Skins" ) )
			{
				var skins = new AdvancedDropdownItem( "Skin Packages" );

				foreach ( var item in Directory.GetDirectories( projectPath + $"/Exports/Skins" ) )
					skins.AddChild( new AdvancedDropdownItem( Path.GetFileName( item ) ) { id = 2 } );

				root.AddChild( skins );
			}

			return root;
		}
	}
}
