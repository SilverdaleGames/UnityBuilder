# Unity Builder Agent Guide

This repository contains an editor-only Unity Package Manager package. Keep this file short and based on verified behavior; architecture context lives in `docs/AI_CONTEXT.md`, and durable decisions live in `docs/DECISIONS.md`.

## Repository layout

- `package.json` is the Unity package manifest and canonical package version.
- `Editor/Core/` contains the provider-neutral `Silverdale.UnityBuilder.Editor` assembly.
- `Editor/Platforms/` contains Android and Apple build helpers.
- `Editor/Integrations/` contains optional Addressables, TeamCity, Unity Build Automation, and Package Manager assemblies.
- `Tests/Editor/` contains Unity editor tests for core orchestration.
- `.github/workflows/` contains validation, pull-request title enforcement, and automated SemVer releases.
- `REPO-STANDARDS.md` defines shared commit, branch, versioning, security, and repository rules.

## Before changing files

- Read this file, `REPO-STANDARDS.md`, `docs/AI_CONTEXT.md`, and relevant entries in `docs/DECISIONS.md`.
- Inspect `git status` and preserve unrelated work.
- Trace plugin lifecycle order and consuming call sites before changing shared APIs.
- Treat `.cs` files and their Unity `.meta` files as coupled assets; preserve GUIDs when moving files.

## Architecture rules

- Keep the package editor-only. Runtime game code must not depend on this assembly.
- Keep product-specific build configurations, identifiers, credentials, upload destinations, and assets in consuming projects.
- Extend behavior through `Plugin` lifecycle hooks instead of adding project conditionals to the core pipeline.
- Preserve `[ConfigName]` discovery so configurations can live in separate project assemblies.
- Preserve stable registration order when no `RunsBefore` or `RunsAfter` constraint applies.
- Keep `BuildServices` defaults safe for local use and register CI-specific providers explicitly.
- Keep CI providers and platform adapters isolated from the core orchestration where practical.
- Avoid adding mandatory dependencies unless the core package requires them. Declare every package dependency in `package.json`.

## Security and external effects

- Never commit credentials, signing material, service-account files, tokens, passwords, keystores, or private keys.
- Building a local player is validation; uploading content, publishing packages, distributing builds, and changing cloud environments are external actions and require explicit authorization.
- Release tags are created only by `.github/workflows/release.yml` after a merge to `main`.

## Validation

- Validate JSON and assembly definition syntax.
- Confirm every package asset that requires Unity identity has its matching `.meta` file.
- Compile `Silverdale.UnityBuilder.Editor` with the minimum supported Unity version when available.
- Exercise configuration discovery and plugin ordering when changing orchestration.
- Validate platform-specific code for the relevant build target and installed Unity platform module.
- State exactly what was and was not run when Unity or platform tooling is unavailable.

## Commits and releases

- Use Conventional Commits without scopes: `<type>: <imperative summary>`.
- Allowed types: `feat`, `fix`, `perf`, `refactor`, `chore`, `docs`, `test`, and `ci`.
- Use `feat!:` or a `BREAKING CHANGE:` footer for a breaking change.
- Keep summaries imperative, at most 72 characters, and without a trailing period.
- Use the SemVer profile from `REPO-STANDARDS.md`. Do not edit versions or create tags manually.
- Use SSH for Git transport.

## Documentation

- Keep setup and public usage in `README.md`.
- Put stable architecture and workflow explanations in `docs/AI_CONTEXT.md`.
- Record non-obvious bug-prevention decisions in `docs/DECISIONS.md`.
