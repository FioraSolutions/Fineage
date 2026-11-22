#!/usr/bin/env python3
"""Build umodel (UE Viewer) command lines.

umodel's positional form is `<package> [<object> [<class>]]`. More than three
positionals is "too many arguments": the second object is read as `class`
(UmodelTool/Main.cpp). `-obj=` can name further objects, but the class applies
only to the first one, so a class from the package export table has to be its
own invocation.

FindExport compares the leaf ObjectName, not Group.Name. The group is recorded
by the caller and is not an argument. Passing `Group.Name` looks up an export
that is not in the package.

Whole-package export (step 9) passes the package only. One mesh (step 7) passes
package and object, no class. A material (step 6) passes package, leaf object,
and class — one invocation per object.
"""
from __future__ import annotations

import argparse
import shlex
import sys

# libpng + SDL are what the linux/amd64 umodel binary needs on ubuntu:22.04.
APT_PREFIX = (
    "apt-get update -qq && apt-get install -y -qq libpng16-16 libsdl2-2.0-0 >/dev/null"
)


def _flags(out: str) -> list[str]:
    return ["/umodel", "-path=/client", "-game=l2", "-export", f"-out={out}"]


def _token(label: str, value: str) -> None:
    if not value or any(ch.isspace() for ch in value) or value.startswith("-"):
        raise ValueError(f"umodel {label} must be one token, got {value!r}")


def _positionals(argv: list[str]) -> list[str]:
    return [arg for arg in argv if arg != "/umodel" and not arg.startswith("-")]


def package_argv(package: str, *, out: str = "/export") -> list[str]:
    """Export every object in the package. No object slot, no class slot."""
    _token("package", package)
    argv = _flags(out) + [package]
    if _positionals(argv) != [package]:
        raise ValueError(f"whole-package umodel argv has an object slot: {argv}")
    return argv


def material_argvs(
    package: str,
    objects: list[tuple[str, str, str]],
    *,
    out: str = "/export",
) -> list[list[str]]:
    """One argv per material. Each object is (group, leaf name, ue class).

    The group is not placed in the argv. The class is the third positional of
    that object's own invocation, never the next object's name.
    """
    if not objects:
        raise ValueError(
            "material export needs objects; a whole package uses package_argv"
        )
    _token("package", package)
    argvs: list[list[str]] = []
    for group, name, ue_class in objects:
        _token("object", name)
        _token("class", ue_class)
        if "." in name:
            raise ValueError(
                "umodel object is the export ObjectName, not Group.Name "
                f"(group={group!r} name={name!r} class={ue_class!r})"
            )
        if "." in ue_class:
            raise ValueError(f"umodel class is one token, got {ue_class!r}")
        argv = _flags(out) + [package, name, ue_class]
        got = _positionals(argv)
        # Group is not inserted. A group that happens to equal the class name
        # still belongs only in the class slot, as the export's class.
        if got != [package, name, ue_class]:
            raise ValueError(f"umodel positional slots are package, object, class: {got}")
        argvs.append(argv)
    return argvs


def object_argvs(
    package: str,
    names: list[str],
    *,
    out: str = "/export",
) -> list[list[str]]:
    """One argv per object, no class. Used for a single static mesh name."""
    if not names:
        raise ValueError("object export needs a name; a whole package uses package_argv")
    _token("package", package)
    argvs: list[list[str]] = []
    for name in names:
        _token("object", name)
        if "." in name:
            raise ValueError(
                f"umodel object is the export ObjectName, not Group.Name (name={name!r})"
            )
        argv = _flags(out) + [package, name]
        if _positionals(argv) != [package, name]:
            raise ValueError(f"mesh umodel argv must be package and one object: {argv}")
        argvs.append(argv)
    return argvs


def docker_inner(argvs: list[list[str]]) -> str:
    """One container script: install libs once, then one umodel process per argv."""
    if not argvs:
        raise ValueError("no umodel commands")
    commands = [shlex.join(argv) for argv in argvs]
    return "set -euo pipefail; " + APT_PREFIX + "; " + "; ".join(commands)


def format_commands(argvs: list[list[str]]) -> str:
    return "\n".join(shlex.join(argv) for argv in argvs)


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--package", required=True, help="path relative to the client root")
    parser.add_argument("--out", default="/export")
    parser.add_argument("--object", action="append", default=[], help="leaf export name; repeatable")
    parser.add_argument(
        "--class", dest="ue_class", action="append", default=[],
        help="export class, paired by position with --object",
    )
    parser.add_argument("--apt", action="store_true", help="print the docker bash -lc script")
    args = parser.parse_args(argv)
    if args.ue_class and len(args.ue_class) != len(args.object):
        raise SystemExit(
            f"--class count ({len(args.ue_class)}) != --object count ({len(args.object)})"
        )
    if not args.object:
        argvs = [package_argv(args.package, out=args.out)]
    elif args.ue_class:
        argvs = material_argvs(
            args.package,
            [("", name, ue_class) for name, ue_class in zip(args.object, args.ue_class)],
            out=args.out,
        )
    else:
        argvs = object_argvs(args.package, args.object, out=args.out)
    print(docker_inner(argvs) if args.apt else format_commands(argvs))
    return 0


if __name__ == "__main__":
    sys.exit(main())
