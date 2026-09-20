using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Silverdale.UnityBuilder
{
	public class Builder
	{
		private static Exception activeException;
		private static readonly List<Action> cleanupActions = new List<Action>();

		public static void RegisterCleanupAction(Action cleanupAction)
		{
			if (cleanupAction == null)
			{
				throw new ArgumentNullException(nameof(cleanupAction));
			}

			cleanupActions.Add(cleanupAction);
		}

		/// <summary>
		/// Main entry point for Buildtool when invoked from command line.
		/// </summary>
		public static void Build()
		{
			DoBuild(Utility.GetCommandLineArg("-config"));
		}

		private static void DoBuild(string configName)
		{
			activeException = null;
			BuildReport activeReport = null;
			try
			{
				var blockCreator = BuildServices.Resolve<ICloudBuildBlockProvider>();
				// Make sure our own the pre-conditions are set correctly when we start building
				blockCreator.BlockOpen("Configuring Build", "Setting build configuration");
				Initialize();

				Utility.DisplayProgressBar("Building", "Configuring build", 1);
				blockCreator.BlockClose("Configuring Build");

				// Find active configuration
				var config = Utility.FindConfig(configName);

				blockCreator.BlockOpen("OnPreprocessBuild", "OnPreprocessBuild");
				config.OnPreprocessBuild();
				blockCreator.BlockClose("OnPreprocessBuild");

				blockCreator.BlockOpen("Building Unity", "Unity Build");
				activeReport = BuildPipeline.BuildPlayer(config.Unity);

				Utility.Log($"Build completed {activeReport.summary.outputPath} {activeReport.summary.result}");
				blockCreator.BlockClose("Building Unity");

				if (activeReport != null && activeReport.summary.result == BuildResult.Succeeded)
				{
					blockCreator.BlockOpen("OnPostProcessBuild", "OnPostProcessBuild");
					config.OnPostprocessBuild(activeReport.summary.outputPath);
					blockCreator.BlockClose("OnPostProcessBuild");
				}
			}
			catch (Exception e)
			{
				activeException = e;
			}

			FinalizeBuild(activeReport);
		}

		private static void Initialize()
		{
		}

		private static void FinalizeBuild(BuildReport buildReport)
		{
			if(activeException != null)
			{
				Debug.LogException(activeException);
				Completed(ExitCode.BuildFailed, buildReport, Utility.FormatExceptionMessage(activeException));
			}
			else if(buildReport != null)
			{
				if (buildReport.summary.result == BuildResult.Failed)
				{
					Completed(ExitCode.BuildFailed, buildReport, null);
				}
				else if (buildReport.summary.result == BuildResult.Cancelled)
				{
					Completed(ExitCode.BuildFailed, buildReport);
				}
				else
				{
					Completed(ExitCode.BuildSucceeded, buildReport);
				}
			}
			else if(buildReport == null)
			{
				Completed(ExitCode.BuildFailed, buildReport);
			}
			else
			{
				Completed(ExitCode.BuildSucceeded, buildReport);
			}
		}

		private static void Completed(ExitCode code, BuildReport buildReport, string errorMessage = null)
		{
			Cleanup();

			if(Application.isBatchMode)
			{
				Utility.Log($"Build completed with exit code {(int)code} - {code.ToString()}");
				if (!string.IsNullOrEmpty(errorMessage))
				{
					Utility.LogError($"Error captured {errorMessage}");
				}

				if (buildReport != null)
				{
					Utility.Log($"Build summary:\n" +
					            $"\t result - {buildReport.summary.result} \n" +
					            $"\t total time - {buildReport.summary.totalTime} \n" +
					            $"\t total errors - {buildReport.summary.totalErrors} \n" +
					            $"\t total warnings - {buildReport.summary.totalWarnings} \n" +
					            $"\t total size - {buildReport.summary.totalSize} \n");
				}

				EditorApplication.Exit((int)code);
			}
			else if(!string.IsNullOrEmpty(errorMessage))
			{
				EditorUtility.DisplayDialog(code.ToString(), errorMessage, "OK");
			}
		}

		internal static void Cleanup()
		{
			activeException = null;

			for (var index = cleanupActions.Count - 1; index >= 0; index--)
			{
				try
				{
					cleanupActions[index]();
				}
				catch (Exception exception)
				{
					Utility.LogError($"Build cleanup failed: {exception.Message}");
				}
			}

			cleanupActions.Clear();
		}
	}
}
