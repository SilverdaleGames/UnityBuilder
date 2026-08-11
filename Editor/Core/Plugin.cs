namespace Silverdale.UnityBuilder
{
	public class Plugin
	{
		/// <summary>
		/// Can be used to disable the plugin so that none of the build steps are run.
		/// </summary>
		public bool IsEnabled = true;

		public virtual void PreProcess(BuilderConfig config) { }
		public virtual void PreBuild(BuilderConfig config) { }
		public virtual void PostBuild(BuilderConfig config, string exportPath) { }
	}
}