# Repository Agent Guide

This is the vendor-neutral entry point for coding agents. Keep it concise and repository-specific. Shared engineering policy lives in `REPO-STANDARDS.md`; human setup and usage belong in `README.md`; deeper architecture and lessons belong under `docs/`.

## Start here

- Read `README.md`, `REPO-STANDARDS.md`, `docs/AI_CONTEXT.md`, and relevant decisions before changing the repository.
- Inspect `git status` and preserve unrelated or user-owned changes.
- Replace every placeholder in this file when creating a repository from the template.

## Repository map

<!-- Describe the important directories, entry points, generated files, and ownership boundaries. -->

## Commands

<!-- Provide exact, verified commands. Delete commands that do not apply. -->

```text
install: <command>
lint:    <command>
test:    <command>
build:   <command>
run:     <command>
```

Do not claim a check passed unless it was run successfully. If a command requires credentials, external services, or unsupported tooling, state that limitation.

## Project rules

<!-- Add architecture rules, conventions, fragile areas, and files agents must not edit. -->

- Follow nearby code and test patterns.
- Keep changes focused; do not combine unrelated cleanup or dependency upgrades.
- Add or update tests when behavior changes.
- Update durable documentation when a change invalidates it.
- Start work from `development` on a typed working branch. Merge working PRs
  into `development` first; promote the tested `development` history to `main`.

## Context documentation

- Keep architecture, data flow, integration boundaries, and verified operational context in `docs/AI_CONTEXT.md`.
- Record non-obvious decisions and regression-prevention lessons in `docs/DECISIONS.md`.
- Keep instructions verifiable and current; mark unknowns instead of inventing commands or guarantees.

## Safety

- Never print, store, or commit credentials, private keys, tokens, passwords, service-account files, or personal data.
- Treat deployments, releases, destructive migrations, and writes to production or shared environments as external side effects requiring explicit authorization.
- Use SSH for Git fetch and push. Keep GitHub remotes in SSH form, such as `git@github.com:OWNER/REPOSITORY.git`.
- GitHub CLI authentication is required only for GitHub API operations such as creating or editing pull requests, issues, or releases—not for ordinary SSH Git transport.

## Repository-specific exceptions

<!-- Record justified exceptions to REPO-STANDARDS.md, including default branch, versioning profile, deployment model, or release process. Write "None" when there are no exceptions. -->

- None.
