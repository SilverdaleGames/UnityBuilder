using System;
using System.Collections.Generic;
using System.Linq;

namespace Silverdale.UnityBuilder
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ConfigName : Attribute
	{
		/// <summary>
		/// Name of the configuration.
		/// </summary>
		public string Name { get; private set; }

		/// <param name="name">Name of the configuration</param>
		public ConfigName(string name) { Name = name; }

		public static Type[] Find(string name)
		{
			return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
				from type in assembly.GetTypes()
				where typeof(BuilderConfig).IsAssignableFrom(type)
				let attributes = type.GetCustomAttributes(typeof(ConfigName), false)
				where attributes != null && attributes.Length > 0
				from attr in attributes.Cast<ConfigName>()
				where attr.Name == name
				select type).ToArray();
		}

		public static string[] GetAllNames()
		{
			// Get all types which are inherited from BuildConfig
			var configs = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
				from type in assembly.GetTypes()
				where typeof(BuilderConfig).IsAssignableFrom(type)
				select type).ToArray();

			List<string> names = new List<string>();
			foreach(var c in configs)
			{
				foreach(ConfigName cn in c.GetCustomAttributes(typeof(ConfigName), false))
					names.Add(cn.Name);
			}

			return names.ToArray();
		}
	}
}