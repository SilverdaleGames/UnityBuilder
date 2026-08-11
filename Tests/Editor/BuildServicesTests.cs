using System;
using NUnit.Framework;

namespace Silverdale.UnityBuilder.Tests
{
	public class BuildServicesTests
	{
		[SetUp]
		public void SetUp()
		{
			BuildServices.Reset();
		}

		[TearDown]
		public void TearDown()
		{
			BuildServices.Reset();
		}

		[Test]
		public void ResetRegistersLocalDefaults()
		{
			Assert.That(BuildServices.Resolve<ICloudBuildManifestProvider>(), Is.TypeOf<LocalBuildManifestProvider>());
			Assert.That(BuildServices.Resolve<ICloudBuildBlockProvider>(), Is.TypeOf<NullCloudBuildBlockProvider>());
			Assert.That(BuildServices.Resolve<ICloudBuildTagProvider>(), Is.TypeOf<NullCloudBuildTagProvider>());
		}

		[Test]
		public void RegisterReplacesServiceForInterface()
		{
			var service = new TestService();

			BuildServices.Register<ITestService>(service);

			Assert.That(BuildServices.Resolve<ITestService>(), Is.SameAs(service));
		}

		[Test]
		public void TryResolveReturnsFalseForMissingService()
		{
			var resolved = BuildServices.TryResolve<ITestService>(out var service);

			Assert.That(resolved, Is.False);
			Assert.That(service, Is.Null);
		}

		[Test]
		public void RegisterRejectsNullService()
		{
			Assert.Throws<ArgumentNullException>(() => BuildServices.Register<ITestService>(null));
		}

		[Test]
		public void ResolveRejectsMissingService()
		{
			Assert.Throws<InvalidOperationException>(() => BuildServices.Resolve<ITestService>());
		}

		private interface ITestService { }

		private sealed class TestService : ITestService { }
	}
}
