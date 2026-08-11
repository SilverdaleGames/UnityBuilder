using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS || UNITY_TVOS
using UnityEngine;
using UnityEditor.iOS.Xcode;
#endif

namespace Silverdale.UnityBuilder
{
	public class ApplePlatform : Plugin
	{
		public enum PushNotificationsCapability
		{
			None,
			Development,
			Release
		};

		public bool EnableBitcode { get; set; } = false;
		public bool EnableSignInWithApple { get; set; } = true;
		public PushNotificationsCapability PushNotification { get; set; } = PushNotificationsCapability.None;

		public List<string> AddAssociatedDomains { get; set; } = new List<string>();

		public List<(string, string)> AddFileToProject { get; set; } = new List<(string, string)>();

		public ApplePlatform()
		{
			var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

			IsEnabled = activeBuildTarget == BuildTarget.iOS ||
			            activeBuildTarget == BuildTarget.tvOS;
		}

#if UNITY_IOS || UNITY_TVOS
		public override void PreProcess(BuilderConfig config)
		{
			base.PreProcess(config);

			if (config.Unity.target == BuildTarget.iOS)
			{

			}
		}

		public override void PostBuild(BuilderConfig config, string exportPath)
		{
			base.PostBuild(config, exportPath);

			if (config.Unity.target == BuildTarget.iOS)
			{
				Utility.Log("OnPostProcessBuildConfigure :: Set bitcode");

				var path = PBXProject.GetPBXProjectPath(exportPath);
				var project = new PBXProject();
				project.ReadFromFile(path);

				var targetGuid = project.GetUnityMainTargetGuid();
				project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", EnableBitcode ? "YES" : "NO");

				// Unity Tests
				var targetUnityTests = project.TargetGuidByName(PBXProject.GetUnityTestTargetName());
				project.SetBuildProperty(targetUnityTests, "ENABLE_BITCODE", EnableBitcode ? "YES" : "NO");

				// Unity Framework
				var targetUnityFramework = project.GetUnityFrameworkTargetGuid();
				project.SetBuildProperty(targetUnityFramework, "ENABLE_BITCODE", EnableBitcode ? "YES" : "NO");

				project.SetBuildProperty(targetUnityFramework, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");

				if (AddFileToProject.Count > 0)
				{
					for (int i = 0; i < AddFileToProject.Count; i++)
					{
						project.AddFileToBuild(targetGuid, project.AddFile(AddFileToProject[i].Item1, AddFileToProject[i].Item2));
					}
				}

				project.WriteToFile(path);

				bool capabilityAdded = false;

				var entitlementsFileName = project.GetBuildPropertyForAnyConfig(targetGuid, "CODE_SIGN_ENTITLEMENTS");
				if (entitlementsFileName == null)
				{
					entitlementsFileName = $"{Application.productName}.entitlements";
				}
				var capManager = new ProjectCapabilityManager(path, entitlementsFileName, "Unity-iPhone");

				if (EnableSignInWithApple)
				{
					capabilityAdded = true;
					capManager.AddSignInWithApple();
				}

				// Update the entitlements file.
				if(PushNotification != PushNotificationsCapability.None)
				{
					capabilityAdded = true;
					bool devEnv = (PushNotification == PushNotificationsCapability.Development);
					capManager.AddPushNotifications(devEnv);
				}

				if (AddAssociatedDomains.Count > 0)
				{
					capabilityAdded = true;
					var domainsArray = new string[AddAssociatedDomains.Count];
					for (int i = 0; i < AddAssociatedDomains.Count; i++)
					{
						domainsArray[i] = AddAssociatedDomains[i];
					}

					capManager.AddAssociatedDomains(domainsArray);

				}

				if(capabilityAdded)
				{
					capManager.WriteToFile();
				}
			}
		}
#endif
	}
}
