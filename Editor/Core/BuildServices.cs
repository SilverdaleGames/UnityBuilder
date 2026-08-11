using System;
using System.Collections.Generic;

namespace Silverdale.UnityBuilder
{
	public static class BuildServices
	{
		private static readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();

		static BuildServices()
		{
			Reset();
		}

		public static void Register<T>(T service) where T : class
		{
			if (service == null)
			{
				throw new ArgumentNullException(nameof(service));
			}

			Services[typeof(T)] = service;
		}

		public static T Resolve<T>() where T : class
		{
			if (Services.TryGetValue(typeof(T), out var service))
			{
				return service as T;
			}

			throw new InvalidOperationException($"Build service is not registered: {typeof(T).FullName}");
		}

		public static bool TryResolve<T>(out T service) where T : class
		{
			if (Services.TryGetValue(typeof(T), out var value))
			{
				service = value as T;
				return service != null;
			}

			service = null;
			return false;
		}

		public static void Reset()
		{
			Services.Clear();
			Register<ICloudBuildManifestProvider>(new LocalBuildManifestProvider());
			Register<ICloudBuildBlockProvider>(new NullCloudBuildBlockProvider());
			Register<ICloudBuildTagProvider>(new NullCloudBuildTagProvider());
		}
	}
}
