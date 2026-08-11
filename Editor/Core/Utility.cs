using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Silverdale.UnityBuilder
{
	public static class Utility
	{
		private const string PREFIX = "SILVERDALE BUILDER";

		public static void Log(string message)
		{
			Debug.Log($"[{PREFIX}] {message}");
		}

		public static void LogError(string message)
		{
			Debug.LogError($"[{PREFIX}] {message}");
		}

		public static string GetCommandLineArg(string argument)
		{
			var args = GetArguments();

			for(int i = 0; i < args.Length - 1; ++i)
			{
				if(args[i] == argument)
					return args[i + 1];
			}
			return null;
		}

		internal static bool HasCommandLineArg(string argument)
		{
			return GetArguments().Contains(argument);
		}

		private static string[] GetArguments()
		{
			return Environment.GetCommandLineArgs();
		}

		/// <summary>
		/// Removes spaces and characters that are not allowed in Windows, Linux or macOS file names
		/// </summary>
		internal static string SanitizeFileName(string fileName)
		{
			return Regex.Replace(fileName, @"[\x00-\x1F<>:""/|\\?* ]", "");
		}

		internal static string GetPlatformOutputName(string outputName, BuildTarget target)
		{
			switch(target)
			{
				case BuildTarget.Android when EditorUserBuildSettings.buildAppBundle:
					return $"{outputName}.aab";

				case BuildTarget.Android when !EditorUserBuildSettings.buildAppBundle:
					return $"{outputName}.apk";

				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
					return Path.Combine(SanitizeFileName(Application.productName), outputName + ".exe");

				case BuildTarget.StandaloneLinux64:
					return Path.Combine(SanitizeFileName(Application.productName), outputName);

				case BuildTarget.StandaloneOSX:
					return outputName;
				default:
					return "";
			}
		}

		internal static string FormatExceptionMessage(Exception e)
		{
			if(e is AggregateException a)
				return string.Join("\n", a.InnerExceptions.Select(i => i.Message));
			return e.Message;
		}

		internal static void DisplayProgressBar(string title, string info, float progress)
		{
			if(!Application.isBatchMode)
				EditorUtility.DisplayProgressBar(title, info, progress);
		}

		internal static BuilderConfig FindConfig(string name)
		{
			// Find all config classes with matching ConfigName attribute
			var configs = ConfigName.Find(name);

			// Make sure only one configuration matches the given name, and if do, return instance of it
			switch(configs.Length)
			{
				case 0:
					throw new Exception($"No configurations found with the name: {name}");

				case 1:
					return  (BuilderConfig)Activator.CreateInstance(configs[0]);

				default:
					throw new Exception($"Found {configs.Length} configurations with the name {name}");
			}
		}

		internal static void InvokePreprocessMethods()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];

				var methods = assembly.GetTypes()
					.SelectMany(t => t.GetMethods())
					.Where(m => m.GetCustomAttributes(typeof(PreProcessAttribute), false).Length > 0)
					.ToArray();

				if (methods.Length > 0)
				{
					for (int j = 0; j < methods.Length; j++)
					{
						var m = methods[j];
						m.Invoke(null, null);
					}
				}
			}
		}
	}
}
