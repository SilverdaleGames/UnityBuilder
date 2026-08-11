using System;

namespace Silverdale.UnityBuilder
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class RunsAfterAttribute : Attribute
	{
		public Type PluginType { get; }

		public RunsAfterAttribute(Type pluginType)
		{
			PluginType = pluginType ?? throw new ArgumentNullException(nameof(pluginType));
		}
	}
}
