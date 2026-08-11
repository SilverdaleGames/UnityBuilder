using UnityEngine;

namespace Silverdale.UnityBuilder
{
	public class TeamCityCloudBuildTagProvider : ICloudBuildTagProvider
	{
		public void AddBuildTag(string tag)
		{
			Debug.Log($"##teamcity[addBuildTag '{tag}']");
		}
	}
}