#!/usr/bin/env python3
"""Write version.json and CHANGELOG.md for a client release."""

from __future__ import annotations

import argparse
import json
import os
import re
import subprocess
import sys
import tempfile
from pathlib import Path

from playtest_hosts import LANDING_URL, endpoint_defaults

ASSETS = [
    {"id": "windows", "file": "Fineage-Windows-x64.zip"},
    {"id": "macos", "file": "Fineage-macOS.zip"},
    {"id": "linux", "file": "Fineage-Linux-x64.zip"},
]


def sanitize_token(value: str) -> str:
    cleaned = re.sub(r"[^A-Za-z0-9._-]", "-", value).strip("-")
    return cleaned or "0.0.0"


def unity_bundle_version(version: str) -> str:
    match = re.match(r"^(\d+)\.(\d+)\.(\d+)", version)
    if not match:
        return "0.0.0"
    return ".".join(match.groups())


def parse_release_semver(tag: str) -> tuple[int, int, int] | None:
    """Semver from stable vX.Y.Z or playtest-X.Y.Z tags; ignores legacy non-semver tags."""
    match = re.fullmatch(r"v(\d+)\.(\d+)\.(\d+)", tag)
    if match:
        return tuple(int(part) for part in match.groups())  # type: ignore[return-value]
    match = re.fullmatch(r"playtest-(\d+)\.(\d+)\.(\d+)", tag)
    if match:
        return tuple(int(part) for part in match.groups())  # type: ignore[return-value]
    return None


def highest_release_semver(repo: Path) -> tuple[int, int, int] | None:
    tags = git_output(repo, ["tag", "-l"], check=False)
    found: list[tuple[int, int, int]] = []
    for line in tags.splitlines():
        parsed = parse_release_semver(line.strip())
        if parsed:
            found.append(parsed)
    return max(found) if found else None


def playtest_next_version(repo: Path) -> str:
    """Next patch after the highest vX.Y.Z or playtest-X.Y.Z tag (0.0.1 if none)."""
    highest = highest_release_semver(repo)
    if highest is None:
        return "0.0.1"
    major, minor, patch = highest
    return f"{major}.{minor}.{patch + 1}"


def version_from_ref_name(ref_name: str) -> str:
    name = ref_name
    if name.startswith("v"):
        name = name[1:]
    elif name.startswith("playtest-"):
        name = name[len("playtest-") :]
    return sanitize_token(name)


def resolve_version(
    ref_type: str,
    ref_name: str,
    describe: str,
    sha: str,
    *,
    repo: Path | None = None,
) -> str:
    # Untagged main pushes: bump patch from the highest vX.Y.Z / playtest-X.Y.Z tag (else 0.0.1).
    del describe, sha
    if ref_type == "tag" and ref_name:
        return version_from_ref_name(ref_name)
    if repo is None:
        raise ValueError("repo is required to resolve an untagged playtest version")
    return sanitize_token(playtest_next_version(repo))


def release_tag(ref_type: str, ref_name: str, version: str) -> tuple[str, bool]:
    if ref_type == "tag" and ref_name:
        return ref_name, False
    return f"playtest-{version}", True


def git_output(repo: Path, args: list[str], *, check: bool = True) -> str:
    result = subprocess.run(
        ["git", *args],
        cwd=repo,
        check=check,
        capture_output=True,
        text=True,
        encoding="utf-8",
        errors="replace",
    )
    if result.returncode != 0:
        return ""
    return result.stdout.strip()


def changelog_range_start(repo: Path) -> str:
    exact = git_output(repo, ["describe", "--tags", "--exact-match", "HEAD"], check=False)
    if exact:
        return git_output(repo, ["describe", "--tags", "--abbrev=0", "HEAD^"], check=False)
    return git_output(repo, ["describe", "--tags", "--abbrev=0"], check=False)


def changelog_markdown(repo: Path, version: str, endpoint: dict[str, object]) -> str:
    previous = changelog_range_start(repo)

    log_args = ["log", "--pretty=format:- %s (%h)", "-n", "100"]
    header_note = ""
    if previous:
        log_args.append(f"{previous}..HEAD")
    else:
        header_note = "No previous tag; showing the latest commits.\n\n"

    log = git_output(repo, log_args, check=False)
    if not log:
        log = "- No commits in range."
    elif log.count("\n") >= 99:
        log += "\n- … older commits omitted"

    host = str(endpoint["host"])
    login_host = str(endpoint["loginHost"])
    game_host = str(endpoint["gameHost"])
    login_port = endpoint["loginPort"]
    game_port = endpoint["gamePort"]

    return (
        f"# Fineage {version}\n\n"
        "Shared playtest client for the one public dev server.\n\n"
        f"- Landing: `{endpoint['landingUrl']}`\n"
        f"- Public host: `{host}`\n"
        f"- Login: `{login_host}:{login_port}`\n"
        f"- Game: `{game_host}:{game_port}`\n\n"
        "## Changes\n\n"
        f"{header_note}{log}\n"
    )


def endpoint_summary(env: dict[str, str]) -> dict[str, object]:
    defaults = endpoint_defaults()
    host = env.get("FINEAGE_PUBLIC_HOST", "").strip() or str(defaults["host"])
    login_host = env.get("FINEAGE_LOGIN_HOST", "").strip() or str(defaults["loginHost"])
    game_host = env.get("FINEAGE_GAME_HOST", "").strip() or str(defaults["gameHost"])
    login_port = port_or_zero(env.get("FINEAGE_LOGIN_PORT", "")) or defaults["loginPort"]
    game_port = port_or_zero(env.get("FINEAGE_GAME_PORT", "")) or defaults["gamePort"]
    return {
        "host": host,
        "loginHost": login_host,
        "gameHost": game_host,
        "loginPort": login_port,
        "gamePort": game_port,
        "landingUrl": LANDING_URL if host == defaults["host"] else "https://" + host,
    }


def port_or_zero(raw: str) -> int:
    raw = raw.strip()
    if not raw:
        return 0
    try:
        value = int(raw)
    except ValueError:
        return 0
    if 1 <= value <= 65535:
        return value
    return 0


def build_notes(repo: Path, env: dict[str, str]) -> dict[str, object]:
    ref_type = env.get("RELEASE_REF_TYPE", "").strip()
    ref_name = env.get("RELEASE_REF_NAME", "").strip()
    sha = git_output(repo, ["rev-parse", "--short=12", "HEAD"])
    full_sha = git_output(repo, ["rev-parse", "HEAD"])
    describe = git_output(repo, ["describe", "--tags", "--always"], check=False)
    version = resolve_version(ref_type, ref_name, describe, sha, repo=repo)
    tag, prerelease = release_tag(ref_type, ref_name, version)
    endpoint = endpoint_summary(env)
    return {
        "version": version,
        "unityVersion": unity_bundle_version(version),
        "commit": full_sha,
        "ref": ref_name,
        "releaseTag": tag,
        "prerelease": prerelease,
        "host": endpoint["host"],
        "landingUrl": endpoint["landingUrl"],
        "loginHost": endpoint["loginHost"],
        "gameHost": endpoint["gameHost"],
        "loginPort": endpoint["loginPort"],
        "gamePort": endpoint["gamePort"],
        "gamePortMeaning": "override" if endpoint["gamePort"] else "server-list",
        "assets": ASSETS,
        "changelog": "CHANGELOG.md",
        "versionFile": "version.json",
        "_markdown": changelog_markdown(repo, version, endpoint),
    }


def write_notes(repo: Path, output: Path, env: dict[str, str]) -> dict[str, object]:
    notes = build_notes(repo, env)
    markdown = str(notes.pop("_markdown"))
    output.mkdir(parents=True, exist_ok=True)
    (output / "CHANGELOG.md").write_text(markdown, encoding="utf-8")
    (output / "version.json").write_text(json.dumps(notes, indent=2) + "\n", encoding="utf-8")
    github_output = os.environ.get("GITHUB_OUTPUT")
    if github_output:
        with open(github_output, "a", encoding="utf-8") as handle:
            handle.write(f"version={notes['version']}\n")
            handle.write(f"unity_version={notes['unityVersion']}\n")
            handle.write(f"release_tag={notes['releaseTag']}\n")
            handle.write(f"prerelease={'true' if notes['prerelease'] else 'false'}\n")
    print(f"version {notes['version']} tag {notes['releaseTag']}")
    return notes


def self_test() -> int:
    assert resolve_version("tag", "v1.2.3", "v1.2.3", "abc") == "1.2.3"
    assert resolve_version("tag", "playtest-0.0.5", "ignored", "abc") == "0.0.5"
    assert parse_release_semver("v1.2.3") == (1, 2, 3)
    assert parse_release_semver("playtest-0.0.2") == (0, 0, 2)
    assert parse_release_semver("playtest-20261028") is None
    assert unity_bundle_version("0.4.1-2-gabcdef") == "0.4.1"
    assert unity_bundle_version("0.0.3") == "0.0.3"
    assert release_tag("tag", "v1.2.3", "1.2.3") == ("v1.2.3", False)
    assert release_tag("branch", "main", "0.0.3") == ("playtest-0.0.3", True)

    with tempfile.TemporaryDirectory() as tmp:
        repo = Path(tmp) / "repo"
        repo.mkdir()
        subprocess.check_call(["git", "init"], cwd=repo)
        subprocess.check_call(["git", "config", "user.email", "test@example.com"], cwd=repo)
        subprocess.check_call(["git", "config", "user.name", "Test"], cwd=repo)
        readme = repo / "README.md"
        readme.write_text("hi\n", encoding="utf-8")
        subprocess.check_call(["git", "add", "README.md"], cwd=repo)
        subprocess.check_call(["git", "commit", "-m", "Initial client"], cwd=repo)

        env = {
            "FINEAGE_PUBLIC_HOST": "play.test",
            "RELEASE_REF_TYPE": "branch",
            "RELEASE_REF_NAME": "main",
        }
        out = Path(tmp) / "out"
        notes = write_notes(repo, out, env)
        assert notes["version"] == "0.0.1"
        assert notes["unityVersion"] == "0.0.1"
        assert notes["releaseTag"] == "playtest-0.0.1"
        assert notes["host"] == "play.test"
        assert notes["loginHost"] == "login.fineage.evpoint.by"
        assert notes["gameHost"] == "game.fineage.evpoint.by"
        assert notes["loginPort"] == 2106
        assert notes["gamePort"] == 7777
        assert notes["landingUrl"] == "https://play.test"
        changelog = (out / "CHANGELOG.md").read_text(encoding="utf-8")
        assert "Initial client" in changelog
        assert "play.test" in changelog
        assert "login.fineage.evpoint.by:2106" in changelog

        subprocess.check_call(["git", "tag", "v0.2.0"], cwd=repo)
        readme.write_text("hi2\n", encoding="utf-8")
        subprocess.check_call(["git", "commit", "-am", "Add login host override"], cwd=repo)
        notes = write_notes(repo, out, env)
        assert notes["version"] == "0.2.1"
        assert notes["unityVersion"] == "0.2.1"
        assert notes["releaseTag"] == "playtest-0.2.1"
        assert "Add login host override" in (out / "CHANGELOG.md").read_text(encoding="utf-8")

        subprocess.check_call(["git", "tag", "playtest-0.2.1"], cwd=repo)
        readme.write_text("hi3\n", encoding="utf-8")
        subprocess.check_call(["git", "commit", "-am", "Third commit"], cwd=repo)
        notes = write_notes(repo, out, env)
        assert notes["version"] == "0.2.2"
        assert notes["releaseTag"] == "playtest-0.2.2"

        env = {
            "FINEAGE_PUBLIC_HOST": "play.test",
            "FINEAGE_GAME_PORT": "4000",
            "RELEASE_REF_TYPE": "tag",
            "RELEASE_REF_NAME": "v0.2.0",
        }
        # Tag points at the parent; version still comes from the ref name.
        notes = write_notes(repo, out, env)
        assert notes["version"] == "0.2.0"
        assert notes["prerelease"] is False
        assert notes["releaseTag"] == "v0.2.0"
        assert notes["gamePort"] == 4000

    print("release-notes self-test OK")
    return 0


def main(argv: list[str]) -> int:
    if "--self-test" in argv:
        return self_test()

    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--repo", type=Path, default=Path(__file__).resolve().parents[1])
    args = parser.parse_args(argv)
    write_notes(args.repo, args.output, dict(os.environ))
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
