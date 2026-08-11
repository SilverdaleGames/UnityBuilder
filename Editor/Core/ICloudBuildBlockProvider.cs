namespace Silverdale.UnityBuilder
{
	public interface ICloudBuildBlockProvider
	{
		void BlockOpen(string name, string description);
		void BlockClose(string name);
		void ProgressStart(string name);
		void ProgressFinish(string name);
	}
}