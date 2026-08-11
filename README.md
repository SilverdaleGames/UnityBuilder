# Unity Builder

An extensible, editor-only Unity build pipeline from Silverdale Games. It discovers project-owned build configurations and composes preprocessing, player-build, and postprocessing behavior from reusable plugins.

The package keeps build orchestration reusable while leaving product-specific environments, credentials, version rules, uploads, and asset configuration in each consuming project.

## Requirements

- Unity 6000.0 or newer
- Addressables 3.1.0 or newer
- A project editor assembly for concrete build configurations

## Install

Add a tagged release to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.silverdale.unity-builder": "https://github.com/SilverdaleGames/UnityBuilder.git#v0.1.0"
  }
}
```

Use a release tag rather than `main` so every project resolves a reproducible package version.

## Define a build configuration

Create an editor assembly that references `Silverdale.UnityBuilder.Editor`, then define a named configuration:

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

Run the configuration from Unity batch mode:

```sh
Unity \
  -batchmode \
  -quit \
  -projectPath /path/to/project \
  -executeMethod Silverdale.UnityBuilder.Builder.Build \
  -config development
```

`BuilderConfig` executes enabled plugins in registration order:

1. `PreProcess` configures Unity and the project.
2. `PreBuild` prepares build-time content such as Addressables.
3. Unity runs `BuildPipeline.BuildPlayer`.
4. `PostBuild` modifies exported platform projects or artifacts.

Included building blocks cover Android SDK and signing configuration, Apple project capabilities, Xcode plist editing, scripting defines, Addressables, Unity Package Manager changes, TeamCity service messages, and local/CI build manifests.

## Register build services

The core uses local no-op services by default and does not assume a CI provider. Register integrations from project-owned editor initialization or configuration code:

```csharp
BuildServices.Register<ICloudBuildManifestProvider>(
    new TeamCityBuildManifestProvider());
BuildServices.Register<ICloudBuildBlockProvider>(
    new TeamCityCloudBuildBlockProvider());
BuildServices.Register<ICloudBuildTagProvider>(
    new TeamCityCloudBuildTagProvider());
```

Custom CI systems implement the same interfaces. `BuildServices.Reset()` restores the local defaults, which is useful for tests and local tooling.

The included TeamCity integration registers itself in batch mode. The Unity Build Automation integration registers its manifest provider when `UNITY_CLOUD_BUILD` is defined. Explicit project registration can override either selection.

## Extending the package

Derive from `Plugin` and override only the lifecycle stages you need:

```csharp
public sealed class BuildMetadataPlugin : Plugin
{
    public override void PreProcess(BuilderConfig config)
    {
        // Configure project state before the player build.
    }

    public override void PostBuild(BuilderConfig config, string exportPath)
    {
        // Process the exported artifact.
    }
}
```

Register it from a project configuration with `Get<BuildMetadataPlugin>()` or `AddPlugin(...)`.

Plugins run in registration order unless they declare dependencies:

```csharp
[RunsAfter(typeof(ScriptingDefines))]
[RunsBefore(typeof(AddressableBuilder))]
public sealed class BuildMetadataPlugin : Plugin
{
}
```

Missing dependency types are ignored, so optional integrations remain optional. Dependency cycles stop the build with an explicit error.

## Assemblies

- `Silverdale.UnityBuilder.Editor` contains the core pipeline and scripting defines.
- `Silverdale.UnityBuilder.Platforms.Editor` contains Android and Apple helpers.
- `Silverdale.UnityBuilder.Addressables.Editor` isolates the Addressables integration.
- `Silverdale.UnityBuilder.TeamCity.Editor` contains TeamCity providers.
- `Silverdale.UnityBuilder.UnityCloudBuild.Editor` is enabled for Unity Build Automation.
- `Silverdale.UnityBuilder.PackageManager.Editor` contains package mutation support.

## Security

Signing credentials must come from CI secrets, environment variables, or command-line arguments. Never commit passwords, tokens, keys, provisioning profiles, or service-account files.

## Versioning and releases

The repository follows [Semantic Versioning](https://semver.org/) and Conventional Commits without scopes:

- `feat:` produces a minor release.
- `fix:` and `perf:` produce a patch release.
- `feat!:` or a `BREAKING CHANGE:` footer produces a major release.
- Other allowed commit types do not release a new version.

After changes merge to `main`, GitHub Actions updates `package.json` and creates the matching `vMAJOR.MINOR.PATCH` tag. Unity projects should consume those tags.

See [REPO-STANDARDS.md](REPO-STANDARDS.md) for the complete repository policy and [AGENTS.md](AGENTS.md) for contributor and coding-agent guidance.

## License

[MIT](LICENSE)

---

Bootstrapped from [aixaCode/repo-template](https://github.com/aixaCode/repo-template).
