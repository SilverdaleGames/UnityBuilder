namespace Silverdale.UnityBuilder
{
	public class LocalBuildManifestProvider : ICloudBuildManifestProvider
	{
		public ICloudBuildManifest GetBuildManifest()
		{
			return new LocalBuildManifest();
		}
	}
}
