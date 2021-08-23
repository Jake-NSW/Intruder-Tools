namespace Intruder.Tools
{
	[System.AttributeUsage( System.AttributeTargets.Class, Inherited = false, AllowMultiple = false )]
	public class CustomToolAttribute : System.Attribute
	{
		readonly string toolName;

		public CustomToolAttribute( string toolName )
		{
			this.toolName = toolName;
		}

		public string Name => toolName;
		public string Title { get; set; }
		public string Description { get; set; }
		public string HelpLink { get; set; }

		public int Priority { get; set; }
		public bool HasOptions { get; set; }

		public string Icon { get; set; }
		public string Tooltip { get; set; }

		public string LightThemeIcon { get; set; }
		public string DarkThemeIcon { get; set; }
	}
}
