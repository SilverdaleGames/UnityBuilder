using System;
using UnityEditor;
using UnityEngine;

namespace Silverdale.UnityBuilder
{
	[InitializeOnLoad]
	internal static class TeamCityBuildServicesInitializer
	{
		static TeamCityBuildServicesInitializer()
		{
#if UNITY_CLOUD_BUILD
			return;
#else
			if (!Application.isBatchMode)
			{
				return;
			}

			BuildServices.Register<ICloudBuildManifestProvider>(new TeamCityBuildManifestProvider());
			BuildServices.Register<ICloudBuildBlockProvider>(new TeamCityCloudBuildBlockProvider());
			BuildServices.Register<ICloudBuildTagProvider>(new TeamCityCloudBuildTagProvider());
#endif
		}
	}

	public class TeamCityBuildManifestProvider : ICloudBuildManifestProvider
	{
		public ICloudBuildManifest GetBuildManifest()
		{
			return new TeamCityBuildManifest();
		}
	}

	public class TeamCityBuildManifest : ICloudBuildManifest
	{
		public string ScmCommitId => Environment.GetEnvironmentVariable("BUILD_VCS_NUMBER");
		public string ScmBranch => Environment.GetEnvironmentVariable("TEAMCITY_BUILD_BRANCH");
		public string BuildNumber => Environment.GetEnvironmentVariable("BUILD_NUMBER");
		public string BuildStartTime => Environment.GetEnvironmentVariable("teamcity.build.startDate");
		public string ProjectId => Environment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME");
		public string BundleId => Environment.GetEnvironmentVariable("bundle.id"); // This needs to be configured in TeamCity as a parameter
		public string UnityVersion => Environment.GetEnvironmentVariable("unity.version"); // This needs to be configured in TeamCity as a parameter
		public string XCodeVersion => Environment.GetEnvironmentVariable("xcode.version"); // This needs to be configured in TeamCity as a parameter
		public string CloudBuildTargetName => Environment.GetEnvironmentVariable("cloudBuildTargetName"); // This needs to be configured in TeamCity as a parameter

		public string Get(string key, string defaultValue)
		{
			string value = Environment.GetEnvironmentVariable(key);
			return string.IsNullOrEmpty(value) ? defaultValue : value;
		}
	}
}
