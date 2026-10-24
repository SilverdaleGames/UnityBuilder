using System.IO;
using NUnit.Framework;
using UnityEditor.Build;

namespace Silverdale.UnityBuilder.Tests
{
	public class SingleSizeAppIconTests
	{
		private string exportPath;
		private string iconSetPath;

		[SetUp]
		public void SetUp()
		{
			exportPath = Path.Combine(Path.GetTempPath(), "SingleSizeAppIconTests", Path.GetRandomFileName());
			iconSetPath = Path.Combine(exportPath, SingleSizeAppIcon.AppIconSetPath);
			Directory.CreateDirectory(iconSetPath);
			File.WriteAllText(Path.Combine(iconSetPath, "Contents.json"), "{ \"images\" : [] }");
			File.WriteAllText(Path.Combine(iconSetPath, "Icon-iPhone-120.png"), "png");
			File.WriteAllText(Path.Combine(iconSetPath, "Icon-iPad-152.png"), "png");
		}

		[TearDown]
		public void TearDown()
		{
			if (Directory.Exists(exportPath))
			{
				Directory.Delete(exportPath, true);
			}
		}

		[Test]
		public void ConvertKeepsOnlyTheAppStoreIcon()
		{
			File.WriteAllText(Path.Combine(iconSetPath, SingleSizeAppIcon.MarketingIconFile), "png");

			SingleSizeAppIcon.Convert(exportPath);

			Assert.That(Directory.GetFiles(iconSetPath, "*.png"),
				Is.EqualTo(new[] { Path.Combine(iconSetPath, SingleSizeAppIcon.MarketingIconFile) }));
		}

		[Test]
		public void ConvertWritesSingleSizeContents()
		{
			File.WriteAllText(Path.Combine(iconSetPath, SingleSizeAppIcon.MarketingIconFile), "png");

			SingleSizeAppIcon.Convert(exportPath);

			var contents = File.ReadAllText(Path.Combine(iconSetPath, "Contents.json"));
			StringAssert.Contains("\"filename\" : \"Icon-Store-1024.png\"", contents);
			StringAssert.Contains("\"idiom\" : \"universal\"", contents);
			StringAssert.Contains("\"platform\" : \"ios\"", contents);
			StringAssert.Contains("\"size\" : \"1024x1024\"", contents);
		}

		[Test]
		public void ConvertFailsWithoutTheAppStoreIcon()
		{
			Assert.Throws<BuildFailedException>(() => SingleSizeAppIcon.Convert(exportPath));
			Assert.That(File.Exists(Path.Combine(iconSetPath, "Icon-iPhone-120.png")), Is.True);
		}
	}
}
