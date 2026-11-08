# L2 effect tooling

## Human Mage VFX batch

1. Manifest (gap → alias / generate / skip):

```bash
python3 tools/l2-effect/human_mage_vfx_manifest.py
```

2. Decode `lineageeffect.u` once (if needed) with l2encdec into
   `.work/umodel-prominence/lineageeffect.dec.u`.

3. Extract `.uc` for generate entries:

```bash
dotnet build tools/l2-effect/UeDecompile -c Release
python3 tools/l2-effect/extract_human_mage_ucs.py
```

4. Unity batch generate + GlobalEffect bind:

```bash
/Applications/Unity/Hub/Editor/6000.3.27f1/Unity.app/Contents/MacOS/Unity \
  -batchmode -nographics -quit \
  -projectPath l2-unity \
  -executeMethod L2EffectGeneratorWindow.GenerateHumanMageBatchCli \
  -logFile - 
```
