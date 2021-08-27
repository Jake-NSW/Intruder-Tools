namespace Intruder.Tools
{
	public sealed class Preferences : Tool
	{
		public Preferences()
		{
			Name = "Options";
			Title = "Tool Options";
			Description = "Edit tool preferences";			
		}

		public override void InspectorGUI()
		{
			foreach ( var item in Window.cachedTools)
			{
				item.InternalOptionsGUI();
			}
		}
	}
}
