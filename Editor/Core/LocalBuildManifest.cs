using UnityEngine;

namespace Silverdale.UnityBuilder
{
	public class LocalBuildManifest : ICloudBuildManifest
	{
		// We assume some mock data here as local Unity builds typically don't have some of these details
		public string ScmCommitId => "Local Build";
		public string ScmBranch => "Local Build";
		public string BuildNumber => "1";
		public string BuildStartTime => System.DateTime.Now.ToString();
		public string ProjectId => Application.productName; // Assuming project name as ProjectId
		public string BundleId => Application.identifier;
		public string UnityVersion => Application.unityVersion;
		public string XCodeVersion => "N/A";
		public string CloudBuildTargetName => "Local Build";

		// Local build won't have extra metadata, return default value
		public string Get(string key, string defaultValue)
		{
			return defaultValue;
		}
	}
}