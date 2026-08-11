using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Silverdale.UnityBuilder
{
	/// <summary>
	/// Plugin for adding and removing Unity Package Manager packages.
	/// Additions are done before removals.
	/// Additions and removals are undone in the post-build step.
	/// </summary>
	public class UnityPackageHelper : Plugin
	{
		/// <summary>
		/// List of names of Package Manager packages to be added.
		/// The latest version of the package is used.
		/// The appropriate registry must already be in <c>manifest.json</c>.
		/// Example of a package name: <c>com.unity.ide.visualstudio</c>.
		/// </summary>
		public List<string> Additions = new List<string>();

		/// <summary>
		/// List of names of Package Manager packages to be removed.
		/// Example of a package name: <c>com.unity.ide.visualstudio</c>.
		/// </summary>
		public List<string> Removals = new List<string>();

		/// <summary>
		/// Keeps track of packages added during the build process.
		/// </summary>
		private List<string> addedPackages = new List<string>();

		/// <summary>
		/// Keeps track of packages removed during the build process.
		/// </summary>
		private List<string> removedPackages = new List<string>();

		public override void PreProcess(BuilderConfig config)
		{
			base.PreProcess(config);
			addedPackages = AddPackages(Additions);
			removedPackages = RemovePackages(Removals);
		}

		public override void PostBuild(BuilderConfig config, string exportPath)
		{
			base.PostBuild(config, exportPath);
			// Undo changes made by PreProcess().
			AddPackages(removedPackages);
			RemovePackages(addedPackages);
		}

		/// <summary>
		/// Adds packages.
		/// </summary>
		private static List<string> AddPackages(List<string> additions)
		{
			var added = new List<string>();

#if UNITY_2019_4_OR_NEWER
			// A request needs exclusive access to the project,
			// so only one of them can be in flight at any time.
			foreach(var packageName in additions)
			{
				AddRequest request = UnityEditor.PackageManager.Client.Add(packageName);

				while(!request.IsCompleted)
				{
					System.Threading.Thread.Sleep(30);
				}

				if(request.Status == StatusCode.Success)
				{
					Utility.Log($"Unity package '{request.Result.packageId}' added");
					added.Add(packageName);
				}
				else
				{
					throw new Exception($"(UnityPackageHelper) {request.Error.message}");
				}
			}
#else
			if(additions.Count > 0)
			{
				throw new Exception($"(UnityPackageHelper) Unity 2019.4 or newer is required to add packages");
			}
#endif

			return added;
		}

		/// <summary>
		/// Removes packages.
		/// </summary>
		private static List<string> RemovePackages(List<string> removals)
		{
			var removed = new List<string>();

#if UNITY_2019_4_OR_NEWER
			// A request needs exclusive access to the project,
			// so only one of them can be in flight at any time.
			foreach(var packageName in removals)
			{
				RemoveRequest request = UnityEditor.PackageManager.Client.Remove(packageName);

				while(!request.IsCompleted)
				{
					System.Threading.Thread.Sleep(100);
				}

				if(request.Status == StatusCode.Success)
				{
					Utility.Log($"Unity package '{request.PackageIdOrName}' removed");
					removed.Add(packageName);
				}
				else
				{
					throw new Exception($"(UnityPackageHelper) {request.Error.message}");
				}
			}
#else
			if(removals.Count > 0)
			{
				throw new Exception($"(UnityPackageHelper) Unity 2019.4 or newer is required to remove packages");
			}
#endif

			return removed;
		}
	}
}
