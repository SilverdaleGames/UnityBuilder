using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Silverdale.UnityBuilder
{
	public class ScriptingDefines : Plugin
	{
		public static bool EnsureDefine(BuildTargetGroup targetGroup, string define)
		{
			NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);
			string current = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
			var defines = current.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

			if (defines.Contains(define))
			{
				return true;
			}

			defines.Add(define);
			PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, string.Join(";", defines));
			AssetDatabase.SaveAssets();
			return false;
		}

		/// <summary>
		/// Custom defines to be added for the script compilation.
		/// </summary>
		public HashSet<string> Defines = new HashSet<string>();

		/// <summary>
		/// Custom defines to be removed from the script compilation.
		/// Undefines are always applied as last, overriding the defines either in either ProjectSettings or in <c>Defines</c>.
		/// </summary>
		public HashSet<string> Undefines = new HashSet<string>();

		public override void PreProcess(BuilderConfig config)
		{
			base.PreProcess(config);

			try
			{
				// Log the target build group
				Debug.Log($"[ScriptingDefines] Target Group: {config.Unity.targetGroup}");
				NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(config.Unity.targetGroup);

				// Fetch current scripting define symbols
				string current = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
				Debug.Log($"[ScriptingDefines] Current Defines: {current}");

				// Convert to list and apply additions/removals
				List<string> list = current.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
				Debug.Log($"[ScriptingDefines] Initial List: {string.Join(", ", list)}");

				// Add new defines
				list.AddRange(Defines.Where(s => !list.Contains(s)));
				Debug.Log($"[ScriptingDefines] After Adding: {string.Join(", ", list)}");

				// Remove unwanted defines
				list.RemoveAll(s => Undefines.Contains(s));
				Debug.Log($"[ScriptingDefines] After Removing: {string.Join(", ", list)}");

				// Join updated defines
				string defines = string.Join(";", list);
				Debug.Log($"[ScriptingDefines] Final Defines: {defines}");

				// Update if necessary
				if (current != defines)
				{
					PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defines);
					AssetDatabase.SaveAssets();
					Debug.Log($"[ScriptingDefines] Updated scripting defines.");
				}
				else
				{
					Debug.Log($"[ScriptingDefines] No changes to scripting defines.");
				}
			}
			catch (Exception ex)
			{
				// Log exceptions for better diagnosis
				Debug.LogError($"[ScriptingDefines] Error: {ex.Message}\n{ex.StackTrace}");
			}
		}
	}
}
