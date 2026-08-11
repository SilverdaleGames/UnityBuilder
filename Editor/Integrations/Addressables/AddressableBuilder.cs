using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build;
using UnityEngine;

namespace Silverdale.UnityBuilder
{
	public class AddressableBuilder : Plugin
	{
		public override void PreBuild(BuilderConfig config)
		{
			base.PreBuild(config);

			var blockCreator = BuildServices.Resolve<ICloudBuildBlockProvider>();

			blockCreator.BlockOpen("Building Addressables", "Building Addressables");
			try
			{
				AddressableAssetSettings.CleanPlayerContent();
				Utility.Log("Addressables - Clean Player Content");
				AddressableAssetSettings.BuildPlayerContent(out var buildResult);
				Utility.Log("Addressables - Build Player Content");
				if (!string.IsNullOrEmpty(buildResult.Error))
				{
					throw new BuildFailedException(buildResult.Error);
				}
			}
			finally
			{
				blockCreator.BlockClose("Building Addressables");
			}
		}
	}
}
