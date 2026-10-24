using System.IO;
using UnityEditor;

namespace Silverdale.UnityBuilder
{
	/// <summary>
	/// Rewrites the exported iOS AppIcon set to Xcode's single 1024x1024 format so Xcode generates every size.
	/// </summary>
	public class SingleSizeAppIcon : Plugin
	{
		private const string IconFile = "Icon-Store-1024.png";
		private const string Contents =
			"{\"images\":[{\"filename\":\"" + IconFile + "\",\"idiom\":\"universal\",\"platform\":\"ios\",\"size\":\"1024x1024\"}]," +
			"\"info\":{\"author\":\"xcode\",\"version\":1}}";

		public SingleSizeAppIcon()
		{
			IsEnabled = EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;
		}

		public override void PostBuild(BuilderConfig config, string exportPath)
		{
			base.PostBuild(config, exportPath);
			Convert(exportPath);
		}

		public static void Convert(string exportPath)
		{
			var iconSetPath = Path.Combine(exportPath, "Unity-iPhone/Images.xcassets/AppIcon.appiconset");
			if (!File.Exists(Path.Combine(iconSetPath, IconFile)))
			{
				Utility.Log($"{IconFile} not found in {iconSetPath}; the AppIcon set was not converted");
				return;
			}

			// Xcode warns about images the set no longer references.
			foreach (var file in Directory.GetFiles(iconSetPath, "*.png"))
			{
				if (Path.GetFileName(file) != IconFile)
				{
					File.Delete(file);
				}
			}

			File.WriteAllText(Path.Combine(iconSetPath, "Contents.json"), Contents);
			Utility.Log($"Converted the AppIcon set at {iconSetPath} to the single 1024x1024 format");
		}
	}
}
