using System.Collections.Generic;
using UnityEditor;

#if UNITY_IOS || UNITY_TVOS
using UnityEditor.iOS.Xcode;
using System.IO;
using UnityEngine;
using System;
using System.Collections;
#endif

namespace Silverdale.UnityBuilder
{
	public class XcodePList : Plugin
	{
		/// <summary>
		/// List of key/value pairs to be added to Info.plist.
		/// Supports: string, bool, List<string>, and nested IList/IDictionary (e.g., List<Dictionary<string, object>>).
		/// </summary>
		public List<(string key, object value)> Additions = new List<(string, object)>();

		/// <summary>
		/// List of keys of elements to be removed from Info.plist.
		/// </summary>
		public List<string> Removals = new List<string>();

		public XcodePList()
		{
			var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
			IsEnabled = activeBuildTarget == BuildTarget.iOS || activeBuildTarget == BuildTarget.tvOS;
		}

#if UNITY_IOS || UNITY_TVOS
		public override void PostBuild(BuilderConfig config, string exportPath)
		{
			base.PostBuild(config, exportPath);
			ModifyMainPList(config, exportPath);
		}

		private void ModifyMainPList(BuilderConfig config, string exportPath)
		{
			const string plistName = "Info.plist";
			string plistPath = Path.Combine(exportPath, plistName);

			var plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));

			// Add items
			foreach (var (key, value) in Additions)
			{
				switch (value)
				{
					case bool b:
						Utility.Log($"Adding '{key}' = '{b}' to {plistName}");
						plist.root.SetBoolean(key, b);
						break;

					case string s:
						Utility.Log($"Adding '{key}' = '{s}' to {plistName}");
						plist.root.SetString(key, s);
						break;

					case List<string> l:
						Utility.Log($"Adding '{key}' = '{string.Join(", ", l)}' to {plistName}");
						var array = plist.root.CreateArray(key);
						foreach (var item in l)
							array.AddString(item);
						break;

					case IList list:
						Utility.Log($"Adding '{key}' as array (IList, count={list.Count}) to {plistName}");
						var arr = plist.root.CreateArray(key);
						AddArrayValues(arr, list);
						break;

					case IDictionary dict:
						Utility.Log($"Adding '{key}' as dict (IDictionary) to {plistName}");
						var d = plist.root.CreateDict(key);
						foreach (DictionaryEntry entry in dict)
							SetDictValue(d, entry.Key.ToString(), entry.Value);
						break;

					default:
						throw new NotImplementedException(
							$"(XcodePList) Adding an element of type '{value.GetType()}' (with value '{value}') not supported.");
				}
			}

			// Remove items
			foreach (var key in Removals)
			{
				if (plist.root.values.ContainsKey(key))
				{
					Utility.Log($"Removing '{key}' from {plistName}");
					plist.root.values.Remove(key);
				}
			}

			plist.WriteToFile(plistPath);
		}

		private static void SetDictValue(PlistElementDict dict, string key, object value)
		{
			switch (value)
			{
				case null:
					dict.SetString(key, string.Empty);
					return;
				case string s:
					dict.SetString(key, s);
					return;
				case bool b:
					dict.SetBoolean(key, b);
					return;
			}

			if (value is IDictionary idict)
			{
				var d = dict.CreateDict(key);
				foreach (DictionaryEntry e in idict)
					SetDictValue(d, e.Key.ToString(), e.Value);
				return;
			}

			if (value is IList ilist)
			{
				var a = dict.CreateArray(key);
				AddArrayValues(a, ilist);
				return;
			}

			dict.SetString(key, value.ToString());
		}

		private static void AddArrayValues(PlistElementArray array, IList list)
		{
			foreach (var item in list)
			{
				switch (item)
				{
					case null:
						array.AddString(string.Empty);
						continue;
					case string s:
						array.AddString(s);
						continue;
					case bool b:
						array.AddBoolean(b);
						continue;
				}

				if (item is IDictionary idict)
				{
					var d = array.AddDict();
					foreach (DictionaryEntry e in idict)
						SetDictValue(d, e.Key.ToString(), e.Value);
					continue;
				}

				if (item is IList ilist)
				{
					var a = array.AddArray();
					AddArrayValues(a, ilist);
					continue;
				}

				array.AddString(item.ToString());
			}
		}
#endif // UNITY_IOS || UNITY_TVOS
	}
}
