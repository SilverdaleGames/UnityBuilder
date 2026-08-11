# Repository Setup Checklist

Complete this after creating a repository from the template, then delete this file.

- [ ] Replace the repository name and description in `README.md`.
- [ ] Fill in the repository map, verified commands, project rules, and exceptions in `AGENTS.md`.
- [ ] Fill in `docs/AI_CONTEXT.md` and record any known decisions or lessons in `docs/DECISIONS.md`.
- [ ] Choose and document a versioning profile from `REPO-STANDARDS.md`.
- [ ] Create `development`, make it the default branch, and protect both `development` and `main`.
- [ ] Adapt `.github/workflows/ci.yml` to run real lint, test, and build commands.
- [ ] Keep `tag.yml` only for the SemVer profile; adapt or remove it for other profiles.
- [ ] Configure deployment and enable its automatic trigger only when it is safe, or remove `deploy.yml` and document the replacement.
- [ ] Add the repository package ecosystem to `.github/dependabot.yml`.
- [ ] Document required environment-variable names in `.env.example` without values.
- [ ] Configure squash merges for working PRs into `development`, commit-preserving promotion to `main`, required checks, and automatic working-branch deletion.
- [ ] Confirm the GitHub remote uses SSH.
- [ ] Document branch-to-environment deployment mappings and destructive/scheduled operations, when applicable.
- [ ] Delete this checklist in the repository-setup PR.
