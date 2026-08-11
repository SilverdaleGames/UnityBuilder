namespace Silverdale.UnityBuilder
{
	public class NullCloudBuildBlockProvider : ICloudBuildBlockProvider
	{
		public void BlockOpen(string name, string description) { }
		public void BlockClose(string name) { }
		public void ProgressStart(string name) { }
		public void ProgressFinish(string name) { }
	}
}
