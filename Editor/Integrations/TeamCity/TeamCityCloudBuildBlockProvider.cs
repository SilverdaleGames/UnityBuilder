using UnityEngine;

namespace Silverdale.UnityBuilder
{
	public class TeamCityCloudBuildBlockProvider : ICloudBuildBlockProvider
	{
		public void BlockOpen(string name, string description)
		{
			Debug.Log($"##teamcity[blockOpened name='{name}' description='{description}']");
		}

		public void BlockClose(string name)
		{
			Debug.Log($"##teamcity[blockClosed name='{name}']");
		}

		public void ProgressStart(string name)
		{
			Debug.Log($"##teamcity[progressStart '{name}']");
		}

		public void ProgressFinish(string name)
		{
			Debug.Log($"##teamcity[progressFinish '{name}']");
		}

		public void SetEnvironmentalVariable(string environmentalVariableName, string value)
		{
			Debug.Log($"##teamcity[setParameter name='{environmentalVariableName}' value='{value}']");
		}
	}
}