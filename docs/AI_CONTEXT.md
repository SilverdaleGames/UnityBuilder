# Unity Builder: AI Context

## Purpose

Unity Builder is a reusable editor-only build orchestration package. Consuming projects define concrete configurations while this package supplies discovery, lifecycle orchestration, platform helpers, and CI adapters.

The package deliberately does not own a game's environments, bundle identifiers, store settings, credentials, upload destinations, version offsets, or content layout.

## Main flow

`Silverdale.UnityBuilder.Builder.Build()` is the batch-mode entry point. It reads `-config`, discovers exactly one `BuilderConfig` type with the matching `[ConfigName]`, invokes its preprocessing lifecycle, calls `BuildPipeline.BuildPlayer`, and invokes postprocessing after a successful build.

`BuilderConfig` owns `BuildPlayerOptions` and an ordered plugin collection. Its lifecycle is:

1. Establish target, scenes, build options, and output path.
2. Let the concrete project configuration customize state.
3. Run each enabled plugin's `PreProcess` method.
4. Run each enabled plugin's `PreBuild` method.
5. Invoke static methods marked with `[PreProcess]`.
6. Build the Unity player.
7. Let the project configuration perform post-build customization.
8. Run each enabled plugin's `PostBuild` method.

Plugin ordering is registration ordering by default. `RunsBefore` and `RunsAfter` attributes add dependency edges, and a stable topological sort resolves them before each lifecycle stage. Missing optional dependencies are ignored; cycles fail the build. `Get<T>()` returns the first registered plugin of that type or creates and appends one. `RemovePlugin<T>()` removes the first matching plugin.

## Extension boundaries

- `Plugin` is the primary behavior extension point.
- `BuilderConfig` is the project policy extension point.
- `[ConfigName]` allows project configurations to be discovered across loaded editor assemblies.
- `BuildServices` owns replaceable manifest, build-block, and build-tag providers. Local and no-op implementations are the safe defaults.
- `ICloudBuildManifestProvider`, `ICloudBuildBlockProvider`, and `ICloudBuildTagProvider` describe CI integration boundaries.
- Platform helpers modify Unity settings or exported projects; they must remain target-gated.

## Dependencies

All source is editor-only. The core assembly has no Addressables or CI reference. Platform and integration assemblies reference the core; the package declares Addressables because its Addressables integration ships in the same package.

## Release model

The package uses SemVer tags. The release workflow scans Conventional Commit messages since the latest `vMAJOR.MINOR.PATCH` tag, updates `package.json`, commits the version change when needed, and tags that commit. Consumers should pin a release tag.

The first release is `v0.1.0`. A normal feature before `1.0.0` increments the minor version according to the repository-wide SemVer profile; breaking changes use the standard major-bump rule.

## Validation limitations

Compilation requires a licensed Unity editor and the relevant platform modules. Repository CI performs deterministic package-structure validation without requiring Unity credentials. A consuming project or dedicated Unity CI should perform full editor compilation before adopting a release.
