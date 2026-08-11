using System;
using System.Collections.Generic;
using System.Linq;

namespace Silverdale.UnityBuilder
{
	internal static class PluginOrder
	{
		public static IReadOnlyList<Plugin> Resolve(IReadOnlyList<Plugin> plugins)
		{
			var enabledPlugins = plugins.Where(plugin => plugin.IsEnabled).ToList();
			var dependencies = enabledPlugins.ToDictionary(
				plugin => plugin,
				plugin => new HashSet<Plugin>());

			foreach (var plugin in enabledPlugins)
			{
				var pluginType = plugin.GetType();
				foreach (RunsAfterAttribute attribute in pluginType.GetCustomAttributes(typeof(RunsAfterAttribute), true))
				{
					AddMatches(dependencies[plugin], enabledPlugins, attribute.PluginType);
					dependencies[plugin].Remove(plugin);
				}

				foreach (RunsBeforeAttribute attribute in pluginType.GetCustomAttributes(typeof(RunsBeforeAttribute), true))
				{
					foreach (var dependency in FindMatches(enabledPlugins, attribute.PluginType))
					{
						if (dependency != plugin)
						{
							dependencies[dependency].Add(plugin);
						}
					}
				}
			}

			var ordered = new List<Plugin>(enabledPlugins.Count);
			while (ordered.Count < enabledPlugins.Count)
			{
				var next = enabledPlugins.FirstOrDefault(plugin =>
					!ordered.Contains(plugin) && dependencies[plugin].All(ordered.Contains));

				if (next == null)
				{
					var cycle = enabledPlugins
						.Where(plugin => !ordered.Contains(plugin))
						.Select(plugin => plugin.GetType().Name);
					throw new InvalidOperationException($"Plugin ordering contains a cycle: {string.Join(", ", cycle)}");
				}

				ordered.Add(next);
			}

			return ordered;
		}

		private static void AddMatches(HashSet<Plugin> dependencies, IReadOnlyList<Plugin> plugins, Type type)
		{
			foreach (var plugin in FindMatches(plugins, type))
			{
				dependencies.Add(plugin);
			}
		}

		private static IEnumerable<Plugin> FindMatches(IEnumerable<Plugin> plugins, Type type)
		{
			return plugins.Where(plugin => type.IsAssignableFrom(plugin.GetType()));
		}
	}
}
