using UnityEditor;
using UnityEngine;

namespace Silverdale.UnityBuilder
{
	public class AndroidPlatform : Plugin
	{
		public AndroidSigningInfo Signing { get; set; }
		public string JDKPath { get; set; } = null;
		public string GradlePath { get; set; } = null;

		public string SDKPath { get; set; } = null;

		public AndroidPlatform()
		{
			var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

			IsEnabled = activeBuildTarget == BuildTarget.Android;
		}
		public override void PreProcess(BuilderConfig config)
		{
			base.PreProcess(config);

			if (config.Unity.target == BuildTarget.Android)
			{
				if (!string.IsNullOrEmpty(JDKPath))
				{
					EditorPrefs.SetString("Jdk11Path", JDKPath);
					EditorPrefs.SetString("JdkPath", JDKPath);
					EditorPrefs.SetBool("JdkUseEmbedded", false);
				}
				else
				{
					EditorPrefs.SetBool("JdkUseEmbedded", true);
				}

				if (!string.IsNullOrEmpty(GradlePath))
				{
					EditorPrefs.SetString("GradlePath", GradlePath);
					EditorPrefs.SetBool("GradleUseEmbedded", false);
				}
				else
				{
					EditorPrefs.SetBool("GradleUseEmbedded", true);
				}

				if (!string.IsNullOrEmpty(SDKPath))
				{
					EditorPrefs.SetString("AndroidSdkRoot", SDKPath);
					EditorPrefs.SetBool("SdkUseEmbedded", false);
				}
				else
				{
					EditorPrefs.SetBool("SdkUseEmbedded", true);
				}

				Utility.Log($"Gradle Path - {EditorPrefs.GetString("GradlePath")}");
				Utility.Log($"Gradle Use Embedded - {EditorPrefs.GetBool("GradleUseEmbedded")}");

				Utility.Log($"JDK Path - {EditorPrefs.GetString("JdkPath")}");
				Utility.Log($"JDK11 Path - {EditorPrefs.GetString("Jdk11Path")}");
				Utility.Log($"JDK Use Embedded - {EditorPrefs.GetBool("JdkUseEmbedded")}");
				Utility.Log($"SDK path - {EditorPrefs.GetString("AndroidSdkRoot")}");

				PlayerSettings.Android.keystorePass = Signing.KeystorePass;
				PlayerSettings.Android.keyaliasName = Signing.KeyaliasName;
				PlayerSettings.Android.keyaliasPass = Signing.KeyaliasPass;

				Utility.Log($"Android certificate password set successfully. {Signing.KeyaliasName}");
			}
		}
	}
}