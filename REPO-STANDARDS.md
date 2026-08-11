# Repo Standards

Rules for every new repo. This file ships inside the template repo
(`aixaCode/repo-template`) so each bootstrapped repo carries its own contract.
Existing repos adopt it opportunistically — don't retrofit history.

Standards version: **1.2.0**. Repository-specific exceptions must be recorded
in `AGENTS.md` rather than silently diverging from this document.

## 1. Commits — Conventional Commits, no scopes

Format: `<type>: <imperative summary>` — lowercase type, no scope parentheses.

| Type | Use for | Version effect |
|---|---|---|
| `feat:` | new user-visible capability | minor |
| `fix:` | bug fix | patch |
| `perf:` | performance improvement | patch |
| `refactor:` | behavior-preserving restructure | none |
| `chore:` | tooling, deps, config | none |
| `docs:` | documentation only | none |
| `test:` | tests only | none |
| `ci:` | workflow/pipeline changes | none |

- **No scopes.** `feat: add export` — never `feat(api): add export`. Scopes add
  ceremony without payoff at our repo sizes; the diff says where.
- Breaking change: `feat!:` or a `BREAKING CHANGE:` footer → major bump.
- Summary is imperative, ≤ 72 chars, no trailing period.

## 2. Versioning — choose one profile

Every repository must select one profile in `AGENTS.md` and describe how a
release is identified. Do not run multiple competing version/tag schemes.

### SemVer profile

- Tags are `vMAJOR.MINOR.PATCH` (e.g. `v1.4.2`). Nothing else.
- **CI creates tags, never humans.** On every merge to `main`, a workflow scans
  commits since the last tag and bumps: breaking → major, `feat:` → minor,
  `fix:`/`perf:` → patch, nothing release-worthy → no tag. First tag is `v0.1.0`.
- Hand-made tags are how `1.10` ends up sorting before `1.0.9`. Don't.
- Release notes are generated from commits since the previous tag; no manual
  CHANGELOG file.

Use this profile for libraries, APIs, and independently versioned applications.
Keep `.github/workflows/tag.yml` enabled for this profile.

### Store-release profile

Use this profile for mobile or desktop applications whose marketing version,
build number, platform, environment, or store review process determines the
release identity. Document the source of each version number, the tag format,
and which automation creates tags. Adapt or remove `tag.yml`; do not apply the
template SemVer workflow without confirming compatibility.

### Continuous-deployment profile

Use this profile for services that deploy revisions without public release
versions. Identify releases by an immutable commit SHA or deployment identifier.
Remove `tag.yml` unless tags serve a separately documented operational purpose.

## 3. Branches and merging

- Use a lightweight GitFlow with two long-lived protected branches:
  `development` integrates completed work, and `main` contains promoted,
  production-ready history.
- Create working branches from the latest `development`. Merge them back into
  `development` through pull requests using squash merge. The PR title becomes
  the Conventional Commit, so WIP commit messages do not need enforcement.
- Promote `development` to `main` through a dedicated promotion PR after CI and
  release checks pass. Do not squash or rebase the promotion: `main` must receive
  the exact tested commits from `development`.
- Do not develop directly on `main` or merge a working branch straight to
  `main`.
- For an urgent production hotfix, branch from `main`, merge the fix to `main`,
  then immediately merge `main` back into `development` before other work.
- Protect both `development` and `main`; require appropriate CI checks and PRs.
- Branch names use `<type>/<short-kebab-description>`. Preferred types are
  `feat`, `fix`, `perf`, `refactor`, `chore`, `docs`, `test`, `ci`, and
  `release`; for example, `fix/android-startup-failure`. Branch-name
  enforcement is optional; the PR title remains the required check.
- Use `development` as the default branch unless a documented repository
  constraint requires another flow.

## 4. CI — required PR checks

Every repo has a `ci.yml` that runs on PRs and pushes to `development` and `main`:

1. install → lint/typecheck → test → build (whichever apply to the stack)
2. a **PR-title check** that enforces `^(feat|fix|perf|refactor|chore|docs|test|ci)(!)?: .+`
   (note: no scope group — scoped titles fail)

Both are required status checks on `development` and `main`. A repo with no
tests still runs lint + build; "no CI" is not a valid state.

## 5. Deployment — automatic only after configuration

- The template deploy workflow is manual and intentionally fails until replaced.
- After a real deployment is configured, prefer merge-to-main deployment with
  CI gating when the product and release process support it.
- Keep a `workflow_dispatch` trigger on the deploy workflow as the escape
  hatch (redeploy without a new commit, or roll forward by tag).
- If a repo genuinely can't auto-deploy (app-store review, on-prem), the deploy
  workflow still builds and publishes the artifact; only the final push is manual.

## 6. Secrets

- Secrets live in GitHub Actions secrets / environments — never in the repo,
  not even in a private one. No service-account JSON files in the tree.
- Local dev reads from `.env` (gitignored); `.env.example` documents the keys.

## 7. Repo hygiene

- **README.md** — what it is, how to run it, how to deploy it. Kept current.
- **AGENTS.md** — concise, vendor-neutral repository map, verified commands,
  safety constraints, and project-specific exceptions.
- **CLAUDE.md** — a thin Claude Code wrapper importing `AGENTS.md`; do not
  duplicate shared instructions.
- **docs/AI_CONTEXT.md** — architecture, data flow, integration boundaries,
  verified commands, and operational context that would make `AGENTS.md` too large.
- **docs/DECISIONS.md** — dated, non-obvious decisions and lessons that prevent
  future regressions.
- **Dependabot** enabled for the package ecosystem and `github-actions`.
- `.gitignore` and `.editorconfig` from the template; extend, don't replace.
- Delete dead branches on merge (repo setting).

## 8. Git transport

- Use SSH for fetch and push. GitHub remotes use
  `git@github.com:OWNER/REPOSITORY.git`.
- Do not require GitHub CLI authentication for ordinary Git operations.
- Authenticate `gh` only when a task needs GitHub API operations such as pull
  requests, issues, or releases.

## Bootstrapping a new repo

1. GitHub → **Use this template** → `aixaCode/repo-template`.
2. Complete `TEMPLATE-CHECKLIST.md`, including the repository's versioning profile.
3. Adapt `ci.yml` steps to the stack; keep the PR-title check unchanged.
4. Keep `tag.yml` only for the SemVer profile; adapt or remove it otherwise.
5. Point `deploy.yml` at the real target and enable the appropriate trigger, or
   delete it and document the replacement.
6. Create `development`, make it the default branch, and protect both
   `development` and `main` with CI and PR requirements. Squash working PRs
   into `development`; preserve commits when promoting `development` to `main`.
7. Fill in `README.md` and `AGENTS.md` before the first feature PR, then delete
   `TEMPLATE-CHECKLIST.md`.

## Template repo contents

```
repo-template/
├── AGENTS.md                 # vendor-neutral agent entry point
├── CLAUDE.md                 # imports AGENTS.md for Claude Code
├── REPO-STANDARDS.md          # this file
├── README.md                  # skeleton
├── TEMPLATE-CHECKLIST.md      # delete after repository setup
├── docs/
│   ├── AI_CONTEXT.md          # architecture and operational context
│   └── DECISIONS.md           # durable decisions and lessons
├── .editorconfig
├── .gitignore
├── .env.example
└── .github/
    ├── PULL_REQUEST_TEMPLATE.md
    ├── dependabot.yml
    └── workflows/
        ├── ci.yml             # lint + test + build, on PR and long-lived branches
        ├── pr-title.yml       # conventional-commit title gate (no scopes)
        ├── tag.yml            # SemVer tag from commits since last tag
        └── deploy.yml         # manual placeholder until safely configured
```

The `tag.yml` reference implementation is the one running in
`personal-assistant` (SemVer bump computed from commit messages in ~40 lines of
shell, no third-party action) — copy it, tighten its regexes to drop the
optional scope group.
