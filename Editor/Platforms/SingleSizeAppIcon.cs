using System.IO;
using UnityEditor;
using UnityEditor.Build;

namespace Silverdale.UnityBuilder
{
	/// <summary>
	/// Rewrites the exported iOS AppIcon set to Xcode's single-size format, using Unity's 1024x1024 App Store icon.
	/// Xcode then generates every icon size the app needs.
	/// </summary>
	public class SingleSizeAppIcon : Plugin
	{
		public const string AppIconSetPath = "Unity-iPhone/Images.xcassets/AppIcon.appiconset";
		public const string MarketingIconFile = "Icon-Store-1024.png";

		private const string SingleSizeContents =
@"{
  ""images"" : [
    {
      ""filename"" : """ + MarketingIconFile + @""",
      ""idiom"" : ""universal"",
      ""platform"" : ""ios"",
      ""size"" : ""1024x1024""
    }
  ],
  ""info"" : {
    ""author"" : ""xcode"",
    ""version"" : 1
  }
}
";

		public SingleSizeAppIcon()
		{
			IsEnabled = EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;
		}

		public override void PostBuild(BuilderConfig config, string exportPath)
		{
			base.PostBuild(config, exportPath);

			if (config.Unity.target == BuildTarget.iOS)
			{
				Convert(exportPath);
			}
		}

		/// <summary>
		/// Converts the AppIcon set in an exported Unity iOS project. Fails the build if the App Store icon is missing.
		/// </summary>
		public static void Convert(string exportPath)
		{
			var iconSetPath = Path.Combine(exportPath, AppIconSetPath);
			var marketingIconPath = Path.Combine(iconSetPath, MarketingIconFile);
			if (!File.Exists(marketingIconPath))
			{
				throw new BuildFailedException(
					$"iOS App Store icon not found at {marketingIconPath}. Set the 1024x1024 icon in the iOS Player Settings.");
			}

			// Only the 1024x1024 image is referenced; Xcode warns about unreferenced files in the set.
			foreach (var file in Directory.GetFiles(iconSetPath, "*.png"))
			{
				if (Path.GetFileName(file) != MarketingIconFile)
				{
					File.Delete(file);
				}
			}

			File.WriteAllText(Path.Combine(iconSetPath, "Contents.json"), SingleSizeContents);
			Utility.Log($"Converted the iOS AppIcon set to the single 1024x1024 format at {iconSetPath}");
		}
	}
}
