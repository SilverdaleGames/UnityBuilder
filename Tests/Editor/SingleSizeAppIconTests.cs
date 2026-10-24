using System.IO;
using NUnit.Framework;

namespace Silverdale.UnityBuilder.Tests
{
	public class SingleSizeAppIconTests
	{
		private string exportPath;
		private string iconSetPath;

		[SetUp]
		public void SetUp()
		{
			exportPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			iconSetPath = Path.Combine(exportPath, "Unity-iPhone/Images.xcassets/AppIcon.appiconset");
			Directory.CreateDirectory(iconSetPath);
			File.WriteAllText(Path.Combine(iconSetPath, "Contents.json"), "{}");
			File.WriteAllText(Path.Combine(iconSetPath, "Icon-iPhone-120.png"), "");
		}

		[TearDown]
		public void TearDown()
		{
			Directory.Delete(exportPath, true);
		}

		[Test]
		public void ConvertKeepsOnlyTheStoreIcon()
		{
			File.WriteAllText(Path.Combine(iconSetPath, "Icon-Store-1024.png"), "");

			SingleSizeAppIcon.Convert(exportPath);

			Assert.That(Directory.GetFiles(iconSetPath, "*.png"),
				Is.EqualTo(new[] { Path.Combine(iconSetPath, "Icon-Store-1024.png") }));
			StringAssert.Contains("\"size\":\"1024x1024\"",
				File.ReadAllText(Path.Combine(iconSetPath, "Contents.json")));
		}

		[Test]
		public void ConvertSkipsSetWithoutStoreIcon()
		{
			SingleSizeAppIcon.Convert(exportPath);

			Assert.That(File.Exists(Path.Combine(iconSetPath, "Icon-iPhone-120.png")), Is.True);
			Assert.That(File.ReadAllText(Path.Combine(iconSetPath, "Contents.json")), Is.EqualTo("{}"));
		}
	}
}
