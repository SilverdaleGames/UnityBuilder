using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Silverdale.UnityBuilder.Tests
{
	public class PluginOrderTests
	{
		[TearDown]
		public void TearDown()
		{
			Builder.Cleanup();
		}

		[Test]
		public void ResolvePreservesRegistrationOrderWithoutDependencies()
		{
			var first = new FirstPlugin();
			var second = new SecondPlugin();

			var result = PluginOrder.Resolve(new List<Plugin> { first, second });

			Assert.That(result, Is.EqualTo(new Plugin[] { first, second }));
		}

		[Test]
		public void ResolveAppliesRunsAfterDependencies()
		{
			var dependent = new RunsAfterFirstPlugin();
			var first = new FirstPlugin();

			var result = PluginOrder.Resolve(new List<Plugin> { dependent, first });

			Assert.That(result, Is.EqualTo(new Plugin[] { first, dependent }));
		}

		[Test]
		public void ResolveAppliesRunsBeforeDependencies()
		{
			var second = new SecondPlugin();
			var first = new RunsBeforeSecondPlugin();

			var result = PluginOrder.Resolve(new List<Plugin> { second, first });

			Assert.That(result, Is.EqualTo(new Plugin[] { first, second }));
		}

		[Test]
		public void ResolveIgnoresMissingDependencies()
		{
			var dependent = new RunsAfterFirstPlugin();
			var second = new SecondPlugin();

			var result = PluginOrder.Resolve(new List<Plugin> { dependent, second });

			Assert.That(result, Is.EqualTo(new Plugin[] { dependent, second }));
		}

		[Test]
		public void ResolveAppliesDependenciesToDerivedPluginTypes()
		{
			var dependent = new RunsAfterFirstPlugin();
			var derived = new DerivedFirstPlugin();

			var result = PluginOrder.Resolve(new List<Plugin> { dependent, derived });

			Assert.That(result, Is.EqualTo(new Plugin[] { derived, dependent }));
		}

		[Test]
		public void ResolveExcludesDisabledPlugins()
		{
			var disabled = new FirstPlugin { IsEnabled = false };

			var result = PluginOrder.Resolve(new List<Plugin> { disabled, new SecondPlugin() });

			Assert.That(result, Has.Count.EqualTo(1));
			Assert.That(result[0], Is.TypeOf<SecondPlugin>());
		}

		[Test]
		public void ResolveRejectsDependencyCycles()
		{
			var plugins = new List<Plugin> { new CycleAPlugin(), new CycleBPlugin() };

			Assert.Throws<InvalidOperationException>(() => PluginOrder.Resolve(plugins));
		}

		[Test]
		public void CleanupRunsRegisteredActionsInReverseOrder()
		{
			var calls = new List<int>();
			Builder.RegisterCleanupAction(() => calls.Add(1));
			Builder.RegisterCleanupAction(() => calls.Add(2));

			Builder.Cleanup();

			Assert.That(calls, Is.EqualTo(new[] { 2, 1 }));
		}

		[Test]
		public void RegisterCleanupActionRejectsNull()
		{
			Assert.Throws<ArgumentNullException>(() => Builder.RegisterCleanupAction(null));
		}

		private class FirstPlugin : Plugin { }
		private class DerivedFirstPlugin : FirstPlugin { }
		private class SecondPlugin : Plugin { }

		[RunsAfter(typeof(FirstPlugin))]
		private class RunsAfterFirstPlugin : Plugin { }

		[RunsBefore(typeof(SecondPlugin))]
		private class RunsBeforeSecondPlugin : Plugin { }

		[RunsAfter(typeof(CycleBPlugin))]
		private class CycleAPlugin : Plugin { }

		[RunsAfter(typeof(CycleAPlugin))]
		private class CycleBPlugin : Plugin { }
	}
}
