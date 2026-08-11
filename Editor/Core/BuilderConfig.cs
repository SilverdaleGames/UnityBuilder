using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Silverdale.UnityBuilder
{
	public class BuilderConfig
	{
		private readonly List<Plugin> plugins = new List<Plugin>();
		private BuildPlayerOptions unity = new BuildPlayerOptions();
		private string outputName;

		public BuildPlayerOptions Unity => unity;
		public string OutputName => outputName;
		public ICloudBuildManifest Manifest => BuildServices.Resolve<ICloudBuildManifestProvider>().GetBuildManifest();
		protected virtual string BuildRoot => Path.Combine(Path.GetDirectoryName(Application.dataPath) ?? ".", "Builds");
		public string GetName() => (GetType().GetCustomAttributes(typeof(ConfigName), true)[0] as ConfigName).Name;

		public void OnPreprocessBuild()
		{
			// By default use sanitized product name as the build filename
			if(string.IsNullOrEmpty(outputName))
			{
				outputName = Utility.SanitizeFileName($"{Application.productName}-{Application.version}-{GetName()}");
			}

			unity.target = EditorUserBuildSettings.activeBuildTarget;

			unity.targetGroup = BuildPipeline.GetBuildTargetGroup(Unity.target);

			//Disabling this option when building iOS, as this crashes the build on macOS
			if (unity.target != BuildTarget.iOS)
			{
				unity.options |= BuildOptions.DetailedBuildReport;
			}

			if (Environment.GetEnvironmentVariable("DEVELOPMENT_BUILD") == "true")
			{
				unity.options |= BuildOptions.AllowDebugging;
				unity.options |= BuildOptions.ConnectWithProfiler;
				unity.options |= BuildOptions.Development;
			}
			else
			{
				unity.options |= BuildOptions.CompressWithLz4HC;
			}

			if (Environment.GetEnvironmentVariable("DEEP_PROFILING") == "true")
			{
				var tagProvider = BuildServices.Resolve<ICloudBuildTagProvider>();
				tagProvider.AddBuildTag($"Deep Profile");
				unity.options |= BuildOptions.EnableDeepProfilingSupport;
			}

			EditorUserBuildSettings.buildAppBundle = Environment.GetEnvironmentVariable("BUILD_TYPE") == "aab";

			// By default build all active scenes which defined in project
			unity.scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();

			Debug.Log($"Builder :: OnPreprocessBuild :: PreProcess for build target: {unity.target}");

			OnPreprocessBuildConfigure();
			NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(Unity.targetGroup);
			Debug.Log($"Scripting defines for build {PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget)}");

			string path = Path.Combine(BuildRoot,
				$"{unity.target}-{PlayerSettings.GetScriptingBackend(namedBuildTarget)}",
				Utility.GetPlatformOutputName(outputName, unity.target));

			// Ensure that the directory exists
			if (!Directory.Exists(Path.GetDirectoryName(path)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
			}

			Debug.Log($"Setting path for build - {path}");
			unity.locationPathName = path;

			foreach(var plugin in PluginOrder.Resolve(plugins))
			{
				if (plugin.IsEnabled)
				{
					plugin.PreProcess(this);
				}
			}

			foreach(var plugin in PluginOrder.Resolve(plugins))
			{
				if (plugin.IsEnabled)
				{
					plugin.PreBuild(this);
				}
			}

			Utility.InvokePreprocessMethods();
		}

		public void OnPostprocessBuild(string exportPath)
		{
			unity.target = EditorUserBuildSettings.activeBuildTarget;
			unity.targetGroup = BuildPipeline.GetBuildTargetGroup(Unity.target);

			Debug.Log($"Builder :: OnPostprocessBuild :: PostProcess for build target: {unity.target}");

			OnPostprocessBuildConfigure(exportPath);

			foreach(var plugin in PluginOrder.Resolve(plugins))
			{
				if (plugin.IsEnabled)
				{
					plugin.PostBuild(this, exportPath);
				}
			}
		}

		protected virtual void OnPreprocessBuildConfigure()
		{

		}

		protected virtual void OnPostprocessBuildConfigure(string exportPath)
		{

		}

		[MenuItem("Silverdale/Builder/PreProcess Methods")]
		public static void Test()
		{
			var config = Utility.FindConfig("development");
		}

		public T Get<T>() where T : Plugin, new()
		{
			return plugins.FirstOrDefault(e => e is T) as T ?? AddPlugin<T>();
		}

		public bool HasPlugin<T>() where T : Plugin
		{
			return plugins.Exists(e => e is T);
		}

		public T AddPlugin<T>() where T : Plugin, new()
		{
			return AddPlugin(new T());
		}

		public T AddPlugin<T>(T plugin) where T : Plugin
		{
			plugins.Add(plugin);
			return plugin;
		}

		public void RemovePlugin<T>() where T : Plugin
		{
			var plugin = plugins.FirstOrDefault(e => e is T) as T;
			if (plugin != null)
			{
				plugins.Remove(plugin);
			}
		}
	}
}
