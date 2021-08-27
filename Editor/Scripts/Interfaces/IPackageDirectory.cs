// Explicitly for IMGUI package dropdown control users 
public interface IPackageDirectory
{
	void OnNothingSelected();
	void OnSkinSelected( string path );
	void OnMapSelected( string path );
}
