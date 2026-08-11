#if UNITY_CLOUD_BUILD
using UnityEditor;
using UnityEngine.CloudBuild;
using UnityCloud.API;

namespace Silverdale.UnityBuilder
{
    [InitializeOnLoad]
    internal static class UnityCloudBuildServicesInitializer
    {
        static UnityCloudBuildServicesInitializer()
        {
            BuildServices.Register<ICloudBuildManifestProvider>(new UnityCloudBuildManifestProvider());
        }
    }

    public class UnityCloudBuildManifestProvider : ICloudBuildManifestProvider
    {
        public ICloudBuildManifest GetBuildManifest()
        {
            var manifest = (BuildManifestObject)AssetDatabase.LoadAssetAtPath(
                "Assets/__UnityCloud__/Resources/UnityCloudBuildManifest.scriptable.asset",
                typeof(BuildManifestObject));
            return new UnityCloudBuildManifest(manifest);
        }
    }

    public class UnityCloudBuildManifest : ICloudBuildManifest
    {
        private BuildManifestObject buildManifestObject;

        public UnityCloudBuildManifest(BuildManifestObject buildManifestObject)
        {
            this.buildManifestObject = buildManifestObject;
        }

        public string ScmCommitId => Get("scmCommitId", null);
        public string ScmBranch => Get("scmBranch", null);
        public string BuildNumber => Get("buildNumber", null);
        public string BuildStartTime => Get("buildStartTime", null);
        public string ProjectId => Get("projectId", null);
        public string BundleId => Get("bundleId", null);
        public string UnityVersion => Get("unityVersion", null);
        public string XCodeVersion => Get("xCodeVersion", null);
        public string CloudBuildTargetName => Get("cloudBuildTargetName", null);

        public string Get(string key, string defaultValue)
        {
            if (buildManifestObject.TryGetValue<string>(key, out var result))
            {
                return result;
            }

            return defaultValue;
        }
    }
}
#endif
