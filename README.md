# Unity Builder

Unity Builder gives a Unity project one batch-mode entry point for named build configurations. Each project owns its release rules; the package supplies the build lifecycle and reusable plugins for common Unity, platform, Addressables, and CI work.

## Requirements

- Unity 6000.0 or newer
- Addressables 3.1.0 or newer
- An editor-only assembly for your project's build configurations

## Install

Add the latest tagged release to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.silverdale.unity-builder": "https://github.com/SilverdaleGames/UnityBuilder.git#v0.2.0"
  }
}
```

Pin a tag instead of `main` so the same project revision always resolves the same package version.

## Create your first configuration

Put build configurations in an editor folder, for example `Assets/Build/Editor`. Add an assembly definition there that references the core package assembly:

```json
{
  "name": "MyGame.Build.Editor",
  "references": [
    "Silverdale.UnityBuilder.Editor"
  ],
  "includePlatforms": [
    "Editor"
  ],
  "autoReferenced": false
}
```

Then add a named configuration:

```csharp
using Silverdale.UnityBuilder;

[ConfigName("development")]
public sealed class DevelopmentBuildConfig : BuilderConfig
{
	protected override void OnPreprocessBuildConfigure()
	{
		Get<ScriptingDefines>().Defines.Add("DEVELOPMENT_BUILD");
		Get<AddressableBuilder>();
	}
}
```

Run it with the Unity executable for your installed editor version:

```sh
/path/to/Unity \
  -batchmode \
  -quit \
  -projectPath /path/to/project \
  -executeMethod Silverdale.UnityBuilder.Builder.Build \
  -config development
```

Unity Builder reports an error when `-config` is missing, unknown, or matches more than one configuration. The selected configuration derives its target and output path from Unity and the supplied command-line options.

## Build lifecycle

For each enabled plugin, Unity Builder runs:

1. `PreProcess` to configure Unity and the project.
2. `PreBuild` to prepare content such as Addressables.
3. `BuildPipeline.BuildPlayer` to create the player.
4. `PostBuild` to modify or inspect the exported artifact.

Plugins run in registration order unless ordering attributes add a dependency.

## Add a project plugin

Derive from `Plugin` and implement only the stages you need:

```csharp
public sealed class BuildMetadataPlugin : Plugin
{
	public override void PreProcess(BuilderConfig config)
	{
		// Configure state before the player build.
	}

	public override void PostBuild(BuilderConfig config, string exportPath)
	{
		// Process the completed artifact.
	}
}
```

Register it with `Get<BuildMetadataPlugin>()` or `AddPlugin(...)` from a configuration. Declare ordering only when one plugin actually depends on another:

```csharp
[RunsAfter(typeof(ScriptingDefines))]
[RunsBefore(typeof(AddressableBuilder))]
public sealed class BuildMetadataPlugin : Plugin
{
}
```

A dependency attribute applies when the referenced plugin is present. Missing optional plugins are ignored; dependency cycles fail with a clear error.

## Included assemblies

Reference only the assemblies your project uses:

- `Silverdale.UnityBuilder.Editor`: build orchestration and scripting defines
- `Silverdale.UnityBuilder.Platforms.Editor`: Android and Apple build helpers
- `Silverdale.UnityBuilder.Addressables.Editor`: Addressables builds
- `Silverdale.UnityBuilder.PackageManager.Editor`: package manifest changes
- `Silverdale.UnityBuilder.TeamCity.Editor`: TeamCity service messages and manifests
- `Silverdale.UnityBuilder.UnityCloudBuild.Editor`: Unity Build Automation manifests

The TeamCity providers register automatically in batch mode, except in Unity Build Automation. Unity Build Automation registers its manifest provider when `UNITY_CLOUD_BUILD` is defined. A project can replace a provider through `BuildServices.Register<T>()`; `BuildServices.Reset()` restores local defaults.

## Credentials

Supply signing passwords, tokens, keys, provisioning profiles, and service-account files through CI secrets, environment variables, or command-line arguments. Do not store them in a configuration class or commit them to the project.

## Contributing and testing

Package tests are under `Tests/Editor`. In Unity, open **Window > General > Test Runner**, select **EditMode**, and run `Silverdale.UnityBuilder.Editor.Tests`.

Before opening a pull request, also import the package into a small Unity project and compile every optional assembly you changed. Platform export and CI-provider changes should be checked in their real environment because EditMode tests do not exercise Xcode, Android signing, TeamCity, or Unity Build Automation end to end.

The repository uses Conventional Commits and Semantic Versioning. After a releasable change merges to `main`, GitHub Actions updates `package.json` and creates the corresponding `vMAJOR.MINOR.PATCH` tag. See [REPO-STANDARDS.md](REPO-STANDARDS.md) for the exact rules.

## License

[MIT](LICENSE)
