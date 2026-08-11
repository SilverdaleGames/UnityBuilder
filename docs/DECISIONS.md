# Unity Builder Decisions and Lessons

Record durable, non-obvious knowledge that can prevent regressions. Keep entries concise and date them.

## 2026-08-10 — Keep product configuration outside the package

**Context:** The build pipeline originated inside Animal Park and mixed reusable orchestration with project configuration.

**Decision:** Keep environments, credentials, bundle identifiers, version offsets, uploads, and game assets in consuming projects. The package owns only reusable editor build infrastructure.

**Reason:** Project assumptions make a shared package unsafe to adopt and difficult to version independently.

**When changing this:** Prefer a public extension point or project-owned plugin. Add core behavior only when it applies consistently across consuming projects.

## 2026-08-10 — Preserve plugin lifecycle ordering

**Context:** Configurations compose behavior through ordered `PreProcess`, `PreBuild`, and `PostBuild` hooks.

**Decision:** Registration order remains execution order unless `RunsBefore` or `RunsAfter` explicitly declares a dependency. Ordering cycles fail before plugin execution.

**Reason:** Signing, scripting defines, Addressables, platform export changes, and project hooks can depend on earlier configuration.

**When changing this:** Add tests covering multiple plugins, disabled plugins, removal, and exceptions at every stage.

## 2026-08-11 — Keep integrations out of the core assembly

**Context:** The original assembly referenced Addressables and directly constructed TeamCity providers.

**Decision:** Keep core orchestration provider-neutral. Platform and vendor integrations compile in separate editor assemblies, and CI services are registered through `BuildServices`.

**Reason:** Projects should consume the core without inheriting unused vendor dependencies or CI assumptions.

**When changing this:** Add new external systems as integration assemblies. Avoid adding their package references or concrete types to the core assembly.

## 2026-08-10 — Release immutable package tags

**Context:** Unity Package Manager can install Git dependencies from branches or tags.

**Decision:** Consumers pin automated SemVer tags. Humans do not edit the package version or create release tags manually.

**Reason:** Immutable tags make package resolution reproducible and keep `package.json` aligned with the installed release.

**When changing this:** Update the release workflow, README, and repository rules together, and validate the first release plus major, minor, patch, and no-release paths.
