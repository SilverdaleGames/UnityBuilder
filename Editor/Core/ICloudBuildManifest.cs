namespace Silverdale.UnityBuilder
{
	public interface ICloudBuildManifest
	{
		string ScmCommitId { get; }
		string ScmBranch { get; }
		string BuildNumber { get; }
		string BuildStartTime { get; }
		string ProjectId { get; }
		string BundleId { get; }
		string UnityVersion { get; }
		string XCodeVersion { get; }
		string CloudBuildTargetName { get; }

		string Get(string key, string defaultValue);
	}
}