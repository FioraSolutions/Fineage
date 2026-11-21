# anim-keyreduce

Offline keyframe reduction for Unity YAML `.anim` clips. No Unity install needed.

The imported Interlude clips are text YAML with `m_Compressed: 0` and a key every 1/24 s on
every bone channel, including channels that never move. The tool drops the keys that the clip's
own Hermite interpolation reproduces from the remaining keys:

- Greedy pass per transform property (rotation / position / scale of one bone, or one float
  curve). Runtime curves (`m_RotationCurves`, ...) and editor curves (`m_EditorCurves`,
  `m_EulerEditorCurves`) of the same property share one keep-mask when their key times match,
  so both representations stay in sync.
- A key is dropped only if the curve without it stays within tolerance at **every original key
  time and every original segment midpoint**.
- Constant curves collapse to their first and last key.
- Curves with weighted tangents, stepped (infinite) tangents, or non-increasing times are kept
  as they are.
- Only whole key blocks are removed. Every other line (GUIDs, fileIDs, bindings, clip settings,
  events) is written back byte-for-byte; the `.meta` files are not touched.

## Validation

Every clip is validated before it is written:

1. the reduced curve is evaluated (float64 Hermite, same formula as `AnimationCurve.Evaluate`
   for non-weighted keys) and compared with the original at all original key times and
   segment midpoints; max error must be <= tolerance;
2. the output is re-parsed; its non-key skeleton must equal the input's and the key count must
   match.

On a parse error or a tolerance breach the tool prints `ERROR <path>: <reason> -- clip left
unchanged`, skips that clip, and exits with code 1.

`--probe-out` writes the ORIGINAL values of the most animated editor curves per clip to a JSON
fixture. `Assets/Tests/EditMode/AnimKeyReducePilotEditModeTests.cs` loads the reduced clips in
Unity and checks `AnimationCurve.Evaluate` against that fixture, so CI proves Unity reads the
reduced YAML and plays the same pose.

## Usage

```bash
python3 tools/anim-keyreduce/anim_keyreduce.py --dry-run <folder-or-.anim>...   # report only
python3 tools/anim-keyreduce/anim_keyreduce.py --report out.csv \
  --probe-out l2-unity/Assets/Tests/EditMode/Fixtures/<name>.json <folder>
```

Default tolerances: quaternion component `--rot 1e-5` (about 0.001 degrees), `--pos 1e-5`
(Unity units), `--scale 1e-5`, `--float 1e-5`, `--euler 1e-3` degrees. Requires Python 3.9+ and
numpy.

Speed is about 3.5 MB of YAML per second, so the full 9.5 GB set needs a few hours on one core;
run it pack by pack.
