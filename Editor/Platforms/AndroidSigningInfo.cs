using System;

namespace Silverdale.UnityBuilder
{
	public class AndroidSigningInfo
	{
		public string KeystorePass;
		public string KeyaliasName;
		public string KeyaliasPass;

		public static AndroidSigningInfo GetAndroidSignInfo()
		{
			string keystorePass = Utility.GetCommandLineArg("-keystorePass") ?? Environment.GetEnvironmentVariable("ANDROID_KEYSTORE_PASSWORD");
			string keyaliasName = Utility.GetCommandLineArg("-keyaliasName") ?? Environment.GetEnvironmentVariable("ANDROID_KEYALIAS_NAME");
			string keyaliasPass = Utility.GetCommandLineArg("-keyaliasPass") ?? Environment.GetEnvironmentVariable("ANDROID_KEYALIAS_PASSWORD");
			return new AndroidSigningInfo()
			{
				KeyaliasName = keyaliasName,
				KeyaliasPass = keyaliasPass,
				KeystorePass = keystorePass
			};
		}
	}
}