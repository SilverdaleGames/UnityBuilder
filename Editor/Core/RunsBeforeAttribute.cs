using System;

namespace Silverdale.UnityBuilder
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class RunsBeforeAttribute : Attribute
	{
		public Type PluginType { get; }

		public RunsBeforeAttribute(Type pluginType)
		{
			PluginType = pluginType ?? throw new ArgumentNullException(nameof(pluginType));
		}
	}
}
