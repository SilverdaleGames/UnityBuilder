using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Silverdale.UnityBuilder
{
	/// <summary>
	/// Raises deployment targets declared by pods to the project's minimum OS version.
	/// </summary>
	public class PatchPodDeploymentTargets : Plugin
	{
		public static PatchPodDeploymentTargets Instance { get; private set; }

		public PatchPodDeploymentTargets()
		{
			IsEnabled = EditorUserBuildSettings.activeBuildTarget is
				BuildTarget.iOS or BuildTarget.tvOS or BuildTarget.VisionOS;

			if (IsEnabled)
			{
				Instance = this;
				Builder.RegisterCleanupAction(() => Instance = null);
			}
		}

		// EDM4U generates the Podfile at order 40 and invokes pod install at order 50.
		[PostProcessBuild(45)]
		private static void OnPostProcessBuild(BuildTarget target, string buildPath)
		{
			if (Instance == null || !Instance.IsEnabled)
			{
				return;
			}

			var (settingKey, minimumVersion) = target switch
			{
				BuildTarget.iOS =>
					("IPHONEOS_DEPLOYMENT_TARGET", PlayerSettings.iOS.targetOSVersionString),
				BuildTarget.tvOS =>
					("TVOS_DEPLOYMENT_TARGET", PlayerSettings.tvOS.targetOSVersionString),
				BuildTarget.VisionOS =>
					("XROS_DEPLOYMENT_TARGET", PlayerSettings.VisionOS.targetOSVersionString),
				_ => (null, null)
			};

			if (string.IsNullOrEmpty(settingKey) || string.IsNullOrEmpty(minimumVersion))
			{
				return;
			}

			var podfilePath = Path.Combine(buildPath, "Podfile");
			if (!File.Exists(podfilePath))
			{
				Utility.Log($"Podfile not found at {podfilePath}; deployment targets were not patched");
				return;
			}

			Utility.Log(
				$"Raising any pod's {settingKey} below {minimumVersion} to match the project's minimum");

			using var writer = File.AppendText(podfilePath);
			writer.WriteLine();
			writer.WriteLine("post_install do |installer|");
			writer.WriteLine("  installer.pods_project.targets.each do |target|");
			writer.WriteLine("    target.build_configurations.each do |config|");
			writer.WriteLine($"      current = config.build_settings['{settingKey}']");
			writer.WriteLine(
				$"      if current.nil? || Gem::Version.new(current.to_s) < Gem::Version.new('{minimumVersion}')");
			writer.WriteLine($"        config.build_settings['{settingKey}'] = '{minimumVersion}'");
			writer.WriteLine("      end");
			writer.WriteLine("    end");
			writer.WriteLine("  end");
			writer.WriteLine("end");
		}
	}
}
