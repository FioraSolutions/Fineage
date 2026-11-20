# Client performance & multi-platform audit (l2-unity, URP)

**Scope:** Unity client `l2-unity/` (Unity `6000.3.27f1`, URP `17.3.0`). Audit only — no code or asset changes.
**Date:** 2026-11-20. **Targets:** Windows, macOS (Apple Silicon), Linux, Android, iOS. **Load target:** ~12k-player MMO, dense towns and sieges (hundreds of pawns in view).

**Method:** grep and metadata sweeps over `ProjectSettings/`, `Assets/Rendering/URP/`, 3,786 scripts, 22,560 texture `.meta`, 4,297 FBX `.meta`, 24,712 materials, prefabs and scenes. Representative files were read in full. No Profiler capture was taken.

**Evidence tags.** Every finding carries one:

- **[read]** — seen directly in code, settings or import metadata. The fact is certain. The cost may still be unknown.
- **[profile]** — the cost or gain has to be measured on a device (Unity Profiler, Frame Debugger, Xcode/RenderDoc/AGI/Snapdragon Profiler) before anyone claims a number.

**Project rules applied to every recommendation:**

1. One shared mechanism plus config/profile classes. No `if (platform)`, `if (race)` or `if (class)` branches scattered through gameplay or UI code. A platform or capability is resolved once at boot into a profile; consumers read profile fields.
2. No silent fallbacks. When data, an asset or a capability is missing, log the concrete missing item (file, id, window, platform) through the project logger, and keep the failure visible.
3. Interlude parity. Options, HUD and draw paths follow `ref/l2-interlude-client/` (`interface.xdat` OptionWnd and others). **`ref/` was not present in this checkout**, so the Interlude option names quoted below come from a web search ([Interlude.Wiki graphics options thread](https://interlude.wiki/retail/t-156541-graphics-options.html)) and must be checked against `interface.xdat` before any options UI is built.

Related audit: [`docs/audit/special-cases-and-fallbacks.md`](../audit/special-cases-and-fallbacks.md).

---

## 0. TL;DR

| # | Finding | Tag | Severity |
|---|---------|-----|----------|
| 1 | All six quality tiers render through the same `URP.asset`: 8× MSAA configured, SSAO on, opaque and depth copies always on. `URP_UltraLow.asset` exists but no tier uses it. The default quality for Standalone and Android is index 0, "Very Low", which halves texture resolution (`globalTextureMipmapLimit: 1`). Nothing in code calls `SetQualityLevel`. | [read] | High (look and perf on every platform) |
| 2 | `Assets/Resources/` holds **18 GB**: animations 13 GB, maps 3.4 GB, SysTextures 1.2 GB. All of it ships in every build and loads through Resources. Map tiles load synchronously. Mobile store size limits make this a blocker. | [read] | Blocker for mobile |
| 3 | 4,287 standalone `.anim` clips (4.6 GB of text YAML) are uncompressed (`m_Compressed: 0`), with a key on every bone every frame. FBX animation compression does not apply to them. | [read] | High (memory, load) |
| 4 | Network receive: packets are decoded on the main thread, every packet allocates (header buffer, body, envelope, closure, message class), and backpressure drops the *oldest normal* packet. `EntitySpawn` counts as normal and can be dropped. The drop-warning path reads `Time.unscaledTime` on the socket thread. | [read] | High in sieges |
| 5 | Per-entity `Update`/`FixedUpdate`/`LateUpdate` MonoBehaviours run on every pawn (movement receive, gravity, yaw, gear). With `maximumDeltaTime 0.2` and `fixedDeltaTime 0.02`, a slow frame can run 10 physics steps for every pawn. | [read], cost [profile] | High in crowds |
| 6 | There is no crowd strategy yet: no animation LOD, no merged character meshes, no impostors, 4 `LODGroup`s in the whole project, no occlusion data, and 18,853 deco `MeshRenderer`s in one tile prefab. | [read], cost [profile] | High in towns and sieges |
| 7 | 17,169 of 22,560 textures have Read/Write enabled, which keeps a CPU copy of each. Mobile has no ASTC overrides. Texture streaming is off. | [read] | Medium–High (memory) |
| 8 | Platform readiness: Windows is locked to DX11. Android is missing the FMOD native libraries. Code reads StreamingAssets with `File.*`, which fails inside an Android APK. There is no touch input, no safe-area handling and no `link.xml` for IL2CPP stripping. A Windows-only cursor API silently does nothing on other platforms. | [read] | Blocker for mobile |
| 9 | There are no `ProfilerMarker`s anywhere (0 hits). Measurement has to come first. | [read] | Enabler |

---

## 1. CPU

### 1.1 Main-thread hot spots

| Where | What | Tag |
|-------|------|-----|
| `Networking/Net/Contract/Envelope/NetFraming.cs:85,93` | `new byte[4]` header buffer and `new byte[total]` body for **every** received frame. `PacketEnvelope` is a class. | [read] |
| `Networking/Net/NetSession.Ops.Core.cs` `OnZonePacket` | Allocates one closure per packet (`() => ApplyZoneIfCurrent(epoch, envelope)`). `HandleZone` then decodes on the main thread. Message types are `sealed class` (`EntitySpawnMsg`, `StatusUpdateNetMsg`, `ValidatePositionMsg`), so each decode allocates again. | [read] |
| `Fineage.Networking/NetMainThreadPump.cs:14,17` | Normal packets are capped at 96 per frame and 512 pending. When full it **drops the oldest**, whatever the entity or opcode. Only despawn, inventory, die, drop, revive and teleport are critical (`NetSession.Ops.Helpers.cs:9-16`). `EntitySpawn` and position or status updates are droppable, so a siege burst can leave entities invisible or stale. | [read] |
| `NetMainThreadPump.cs:96` (`MaybeWarnDrops`) | Runs inside `Post()` on the **socket thread** and reads `Time.unscaledTime`, a main-thread-only API. The resulting exception aborts `Post` before `_normal.Enqueue`, so the incoming packet is lost too. `NetTransport` catches and logs it as a "packet handler" error. Confirm with a test that pushes more than 512 normal packets. | [read], confirm [profile] |
| `Networking/EventProcessor.Helpers.cs` `Update` | A second queue (`PacketApplyQueue` → `EventProcessor`) with **no** per-frame cap: it drains the whole batch every frame. | [read] |
| `UI/Game/Nameplate/NameplatesManager.Layout.Helpers.cs:30,44,46` | Each frame: `Physics.OverlapSphere` allocates an array, two lambdas are allocated (`ForEachUser`/`ForEachNpc`), and `UpsertEntry` runs for **every** registered user and NPC regardless of distance. That is O(world entities) per frame. The text, cast-bar and bubble drawing is already batched in a custom drawer, which is the right design. | [read] |
| `Fineage.Game/Rendering/L2ActorPointLights.ApplyCore.*.cs` | Every 1 s (`RefreshSec`) it calls `FindObjectsByType<Animator>()`, `FindObjectsByType<L2ActorLocalLight>()` and `GetComponentsInChildren<SkinnedMeshRenderer>`, so the hitch grows with total pawn count. It also calls `SetPropertyBlock` per SMR, which takes those renderers off the SRP Batcher path (see §2). | [read], hitch size [profile] |
| `Combat/Gear/Gear.cs:64-90` `LateUpdate` | Runs per pawn, every frame. It scans the shield bone's children with `child.name.StartsWith(...)`, and `Object.name` allocates a managed string on each call. | [read] |
| `Game/Manager/SceneLoader.StreamingUnload.*.cs:15,40` | Synchronous `Resources.Load<GameObject>` of tile prefabs (up to ~200 MB per tile folder) during streaming. `CanLoadTile` loads the entire tile prefab only to check that it exists. | [read], hitch [profile] |
| `Rendering/L2DoorPassage.cs:39` | `FindObjectsByType<MeshCollider>` up to 9 times after each scene load. Bounded, but it is a load hitch. | [read] |
| `Fineage.Game/Spawn/SpawnFrameBudget.cs:6` | 6 spawns per frame (count-based, not time-based). 500 pawns arriving in a siege take about 1.4 s at 60 fps; at 20 fps on mobile it takes 4 s. | [read] |

### 1.2 Per-entity MonoBehaviour callbacks

Of 126 `Update`/`LateUpdate`/`FixedUpdate` methods, these run **once per pawn** (code reading). The cost at N=300–1,000 needs profiling:

- `Networking/Unity/NetworkTransformReceive.Core.cs` — `FixedUpdate`
- `Game/Manager/MonsterManager/GravityMonster.cs`, `Game/Manager/NpcManager/GravityNpc.Core.cs` — `FixedUpdate`, each with a `CharacterController` check and move
- `Game/Manager/NpcManager/MoveNpc.Lifecycle.Helpers.cs`, `Game/Manager/MonsterManager/DeadMonster.Core.cs`
- `Fineage.Controller/EntityAction/L2PawnYaw.cs` — `LateUpdate`
- `Combat/Gear/Gear.cs` — `LateUpdate`
- `Animation/BaseAnimationController.Lifecycle.Core.cs` — `Update`. It returns early, but Unity still pays the native-to-managed call.
- `Rendering/Effects/EntityStunStars.cs`, `EntityAbnormalVisuals.Apply.Core.cs`, `Game/Character/SelectableCharacterEntity.Core.cs`, `Fineage.Game/Deco/NpcDecoKeepWorldUp.cs`

**Fixed-step spiral.** `CrowdAnimTest.EnsureTimeCap` sets `fixedDeltaTime 0.02` and `maximumDeltaTime 0.2` (`ProjectSettings/TimeManager.asset:11`). After a 200 ms frame, Unity runs up to **10** `FixedUpdate` passes for every pawn plus physics, and that makes the next frame slow too. [read]; the size of the effect needs [profile].

`Game/Manager/MonsterManager/Move/MoveAllCharacters.*` already loops movement centrally over a dictionary and has its own Stopwatch log (`MoveCostLog`). Extend that pattern; do not add more per-pawn scripts.

### 1.3 Allocations, LINQ, Find, logging

| Pattern | Count | Notes | Tag |
|---------|-------|-------|-----|
| `using System.Linq` | 274 files | Only 3 files with update methods use it (`Combat/Gear/Gear.cs`, `Game/ShortCut/PlayerShortcuts.Helpers.cs`, `UI/Login/CharSelectWindow.Ui.Core.cs`), and those calls run on events (equip), not per frame. Low priority. | [read] |
| `FindObject*` / `GameObject.Find` | 47 files | 8 files with update methods use it (`Audio/L2TownAmbience.cs`, `Audio/ThirdPersonListener.cs`, `Rendering/L2CaveTerrainGate.cs`, `L2BrushPortal.cs`, `L2VillageStatuePanner.cs`, `LoginForestPresenter.cs`, `L2DoorPassage.cs`, `PlayerLoopDebugger`). Most are throttled or cached, so audit each with a marker. | [read] |
| `Camera.main` in update methods | 11 files | Unity caches it since 2020.2. Low priority. | [read] |
| `Debug.Log*` | 1,183 calls / 550 files | `ProjectSettings.asset:58` `m_StackTraceTypes` is ScriptOnly for **all** log types, so every log call in a player build captures a managed stack trace. | [read] |
| `ProfilerMarker` / `Profiler.BeginSample` | **0** | Custom Stopwatch logs (`MoveCostLog`, `PacketApplyLog`) do not appear in the Profiler timeline. | [read] |
| Coroutines | 70 `StartCoroutine` | No storm pattern found. Event `+=` subscriptions were not counted. | [read] |

### 1.4 Where Jobs/Burst/threads actually pay

Burst, Collections and Mathematics are already resolved in `Packages/packages-lock.json`. The particle sim already uses them (`Rendering/Effects/ParticleGroup/Jobs/ParticleGroupSimJobs.cs`, `[BurstCompile] IJob/IJobParallelFor`). Good candidates, in order of expected payoff ([profile] before committing):

1. **Packet decode off the main thread** (plain socket thread, no Burst needed). Decode into pooled message objects or structs in `NetTransport.ReadLoopAsync`; the main thread only *applies*. Coalesce per object: latest-wins for position, status and HP, keyed by `(objectId, opcode-class)`. Spawn and despawn are never dropped. Base the budget on milliseconds, not a packet count.
2. **Remote pawn movement and yaw**: `L2PawnWalk.TryStep` is pure math. Run it as `IJobParallelForTransform` over a `TransformAccessArray` owned by `MoveAllCharacters`, and fold in `NetworkTransformReceive`, `L2PawnYaw` and `Gravity*`. Ground snapping becomes batched `RaycastCommand`s.
3. **Visibility and LOD tier assignment**: one Burst job per frame computes distance, frustum test and screen size per pawn, and writes a tier byte (§2.5). Nameplates, animator rate, FX budget and attachments read that same array.
4. **Instanced deco culling**: `L2InstancedPropRenderer` culls cells by distance only, with no frustum test (`Lifecycle.cs`). A Burst frustum and distance job fills the instance lists.
5. **Geodata pathfinding** (`Terrain/Geodata/PathFinderFactory.Helpers.cs` already uses a Task): Burst A* over a `NativeArray` grid if profiling shows it.

**Stays on the main thread:** all `UnityEngine.Object` access (GameObject, Transform outside `TransformAccessArray`, Animator parameters, Renderer, Material), UI Toolkit, `Instantiate`/`Destroy`, single physics queries (batched `*Command` jobs are the exception), applying packets to `World` state, and the FMOD Studio event lifecycle.

**Animator:** Unity already evaluates Animators on worker threads. The win comes from **fewer animators updating less often** (§2.5), not from rewriting animation as jobs.

---

## 2. GPU and rendering

### 2.1 Pipeline configuration (read from assets)

| Setting | Value | Ref | Comment |
|---------|-------|-----|---------|
| Rendering path | Forward+ (`m_RenderingMode: 2`), depth priming Auto | `Assets/Rendering/URP/URP_Renderer.asset:50-51` | Good for many lights. Depth priming on mobile TBDR adds a pass ([profile]). |
| MSAA | 8× | `URP.asset:28` | Game cameras have `m_AllowMSAA: 0`, so it is mostly inert, but any camera that allows MSAA would pay 8×. Use none or 2× on mobile and 2×/4× on desktop, set per tier. |
| Opaque and depth texture | always on | `URP.asset:22-23` | An extra copy pass every frame. Expensive bandwidth on tile-based mobile GPUs. Enable only on tiers or cameras whose features need it (water, L2Fx compositor, soft particles). |
| HDR | off | `URP.asset:26` | Matches Interlude's LDR look and saves bandwidth. Keep it off on mobile. |
| Color space | **Gamma** | `ProjectSettings.asset:50` | Legacy path, but it matches UE2 Interlude. Keep it only if parity captures require it, and document that as a deliberate divergence. |
| Main light shadows | off, shadowmap res 8192 configured | `URP.asset:45-46` | If shadows are enabled later, 8192 is far too large; set it per tier (1024 mobile, 2048–4096 desktop). |
| Additional lights | per-vertex | `URP.asset:47` | Cheap. |
| SSAO | on in `URP_Renderer`, off in `URP_Renderer_UltraLow` | renderer features | Full-screen cost on every tier today. |
| Decals | on | renderer features | Check usage. |
| Custom feature | `L2FxCompositorRendererFeature` (`renderPassEvent 550`) | `URP_Renderer.asset:66` | Effect compositor; cost [profile]. |
| SRP Batcher | on | `URP.asset:72` | Good. |
| GPU Resident Drawer | off | `URP.asset:86` | See §2.3. |
| Volume update | every frame | `URP.asset:94` | Use ViaScripting when volumes are static. |
| Adaptive Performance flag | on | `URP.asset:79` | **The package is not installed**, so the flag does nothing. |
| Render Graph | `m_EnableRenderGraph: 0` | `UniversalRenderPipelineGlobalSettings.asset:32` | In 6000.3, Compatibility Mode requires the `URP_COMPATIBILITY_MODE` define, which is only referenced in `L2TerrainOverlayRenderPass.cs`. 4 of 6 custom passes implement `RecordRenderGraph`; confirm the other 2 run in a player build. |
| Post-process variant stripping | `m_StripUnusedPostProcessingVariants: 0` | Global settings | Turn on to reduce build size and warm-up time. |

### 2.2 Quality tiers (`ProjectSettings/QualitySettings.asset`)

- There are 6 tiers. Only **Very High** and **Ultra** set `customRenderPipeline`; the other four fall back to the GraphicsSettings default, which is the **same** `URP.asset`. So every tier gets the same MSAA, SSAO and copy passes. `URP_UltraLow.asset` (MSAA 1, shadow distance 0, SSAO off) is not referenced by any tier. [read]
- `m_PerPlatformDefaultQuality`: Standalone 0, Android 0, iOS not listed (`:322-325`). Tier 0 "Very Low" sets `globalTextureMipmapLimit: 1` (`:22`, half-resolution textures) and `lodBias 0.3`. No runtime code calls `QualitySettings.SetQualityLevel`, and `GameSettings.cs` only stores audio and resolution. **Desktop players would start at Very Low.** Confirm in a player build. [read]
- QualitySettings `antiAliasing`, `shadows` and `pixelLightCount` are ignored under URP; the pipeline asset decides. Those values are noise. [read]
- `FpsLimiter.cs` forces `vSyncCount = 0` and `targetFrameRate = 60` (5 when unfocused) on every platform. That overrides the tiers' `vSyncCount: 1` and is wrong for 120 Hz ProMotion and for 30 fps mobile. [read]

### 2.3 Draw calls, batching, instancing, culling

| Item | Evidence | Tag |
|------|----------|-----|
| Custom L2 shaders (`Shaders/World/L2ActorModulate2X.shader`, `L2StaticMeshModulate2X`, `L2ActorEnvSpecular`, FX emitters) | Use `CBUFFER_START(UnityPerMaterial)` (SRP Batcher compatible), `#pragma target 2.0`, one `shader_feature_local` (`_ALPHATEST_ON`) and `multi_compile_instancing`. Very lean variant set. **No `DOTS_INSTANCING_ON`**, so these shaders cannot use the GPU Resident Drawer or BRG. | [read] |
| URP/Lit materials | 16,199 materials. 7,030 of them are icon materials under `SysTextures/Icon` (import artifacts, but they ship via Resources). The world-deco ones mostly have specular and env reflections off. Instancing variants are enabled on 1,174 of 24,712 materials. | [read] |
| MaterialPropertyBlocks | `L2ActorPointLights`, `ItemEnchantGlowView`, `L2WeaponChargeGroup`, `L2StaticMeshSunMaskRuntime`, `EffectPart`, and others. A renderer with an MPB is **not** SRP-batched, and every pawn SMR gets one from actor point lights. | [read], cost [profile] |
| Deco layers | `Maps/16_25/16_25_DecoLayer.prefab` has **18,853** `MeshRenderer`s and `16_24` has 12,432. `L2InstancedPropRenderer` (`Graphics.DrawMeshInstanced`) is used only in `17_25` and `l2_lobby`. `LayerCullDistance` sets a 30 m cull distance on the Deco layer. Static batching is on and dynamic batching is off. | [read] |
| LOD | 4 `LODGroup`s in all prefabs. No mesh LODs for characters or deco. `com.whinarn.unitymeshsimplifier` is installed but not wired into import. | [read] |
| Occlusion | 31 scenes with `m_OcclusionCullingData: {fileID: 0}`. Cameras have `m_OcclusionCulling: 1`, which does nothing without baked data. Baked OC does not fit streamed tiles anyway; GPU occlusion through the GPU Resident Drawer does. | [read] |
| Camera | far clip 1024, post-processing off on 6 of 7 cameras. | [read] |

### 2.4 Overdraw, transparency, post-processing

- Effects: skill FX are transparent sprite and mesh emitters (`L2FxUnifiedSpriteEmitter`, `L2FxUnifiedMeshEmitter`) plus the L2Fx compositor pass. In a siege, overdraw from 100+ concurrent casts is the main GPU risk on mobile. There is no global FX budget or "others' effects" limit. [read], cost [profile]
- `L2PostProcess.asset` has many components active (Bloom, DoF, Lens Distortion, Chromatic Aberration, Film Grain, Panini, SS Lens Flare, color stack). Only one camera enables post. Check in the Frame Debugger which passes actually run. [profile]
- Shadows are off today, which suits crowds. If a tier enables them, character shadows should follow the crowd tier (near pawns only). Retail Interlude also lets players switch shadows off.

### 2.5 Crowd rendering strategy (recommendation)

An L2 pawn is several skinned parts (face, hair, upper, lower, gloves, boots) plus weapons and attachments. The FBX import settings make this heavier:

- `animationType: 2` (Generic) and `optimizeGameObjects: 0` on all 4,297 FBX, so every bone is a GameObject `Transform`. [read]
- Prefab animators default to `AlwaysAnimate`, and `CrowdAnimTest.KeepAnimatorUpdating` switches them to `CullUpdateTransforms`, so the state machine still runs off-screen. Its comment says this works around bad import bounds. Fix the bounds instead of animating off-screen. [read]
- `PlayerSettings gpuSkinning: 1` (compute skinning) is on. [read]

Proposed shared mechanism: one system, with tiers chosen by profile.

| Tier | Who | Animation | Mesh | Extras |
|------|-----|-----------|------|--------|
| T0 | self, party, target, attackers | Animator every frame | full parts | all FX, nameplate, shadows if the tier allows |
| T1 | near, up to `MaxFullPawns` | Animator every frame | **parts merged into 1 SMR per pawn at equip time** (same skeleton; texture array or atlas) | weapon FX |
| T2 | mid | Animator in manual update at 10–15 Hz, staggered (PlayableGraph `DirectorUpdateMode.Manual`), or `Animator.speed` stepping | merged SMR, fewer bones (Optimize Game Objects with exposed attach bones) | no enchant glow or abnormal FX |
| T3 | far | baked vertex-animation textures (VAT), drawn with `Graphics.RenderMeshInstanced`/`RenderMeshIndirect` | low-poly baked mesh | nameplate only |
| T4 | beyond limit | not drawn | — | count shown, as with Interlude display limits |

- The thresholds live in a `CrowdLodProfile` ScriptableObject per quality tier (desktop and mobile are just different profile assets). No platform branches in code.
- Missing VAT or merged-mesh data for a pawn logs an error naming the pawn's class or NPC id and the tier. The pawn stays at a visibly higher tier; it is never hidden silently.
- Interlude parity: the retail options include "GPU Animation", "Reflections", "Shader" and "Weather effects", and community guides cite NPC/player display limits for sieges. Verify the exact OptionWnd controls in `interface.xdat` before exposing tier knobs (`ref/` missing locally).

**GPU Resident Drawer** (Forward+ and SRP Batcher are already set): worth trying for the static deco layers, as an alternative to converting everything to `L2InstancedPropRenderer`. It needs `DOTS_INSTANCING_ON` support in `L2StaticMeshModulate2X` and no MPBs on those renderers, and it gives GPU occlusion culling. It does not help SMRs. [profile]

---

## 3. Assets

### 3.1 Layout and build inclusion

| Folder | Size | Notes |
|--------|------|-------|
| `Assets/Resources/Data/Animations` | 13 GB | 4,287 `.anim` (4.6 GB) + 1,684 FBX (664 MB) + controllers |
| `Assets/Resources/Data/Maps` | 3.4 GB | 501 `.asset`, 223 prefabs, per-tile folders up to 207 MB |
| `Assets/Resources/Data/SysTextures` | 1.2 GB | 14,063 textures |
| `Assets/Resources/Data/Textures` | 935 MB | 6,430 textures |
| `Assets/StreamingAssets` | 121 MB | FMOD banks (~20 MB) + LUT/meta data |

Everything under `Resources/` is packed into the player whether it is referenced or not. The Resources index loads at startup and assets cannot be unloaded per group. Google Play limits the base module to 200 MB (larger content needs Play Asset Delivery), and iOS has its own download limits. **Mobile needs Addressables or AssetBundles with per-tile, per-race and per-effect groups, and remote delivery.** Scenes also live in `Resources/Scenes` and are listed in `EditorBuildSettings.asset`. [read]

### 3.2 Textures (22,560 metas)

| Fact | Count | Tag |
|------|-------|-----|
| Read/Write enabled (CPU copy kept) | **17,169** (SysTextures 13,681 incl. all race texture folders; UI 1,312; Textures 2,141) | [read] |
| Mipmaps off | 9,117: icons 7,040 (correct for UI), UI 2,005 (correct). World textures have mips. | [read] |
| Streaming mipmaps | 0 textures; `streamingMipmapsActive: 0` on all tiers | [read] |
| Default max size | 2048. Sampled race textures are 256² and 512², so VRAM per texture is small; the problem is the count and the Read/Write copies. | [read] |
| Standalone overrides | 7,040 icons forced to RGBA32 uncompressed. UI Toolkit's dynamic atlas can batch 32×32 icons; confirm the atlas setting. | [read] |
| Android overrides | 10,694 platform blocks, **none overridden**. Android default compression is unset (`m_BuildTargetDefaultTextureCompressionFormat` `m_Formats: 00000000`). iOS: 1 block. | [read] |
| Crunch | off | [read] |

Runtime readers that legitimately need Read/Write: `L2VertMeshFrameBank`, `L2StaticMeshSunVisibility`, `L2CaveTerrainGate`, `L2BrushPortal`, `L2DoorPassage`, and the editor-only Yebis capture tools. Everything else should have Read/Write off.

**Target policy**, enforced by one `AssetPostprocessor` plus folder presets so nobody hand-tunes 22k metas:

| Class | Desktop (Win/Mac/Linux) | Android / iOS | Mips | R/W |
|-------|-------------------------|---------------|------|-----|
| World / character albedo | BC7 (or DXT1/5) | ASTC 6×6 | on, streaming on | off |
| Normal / mask | BC5 / BC7 | ASTC 4×4 or 5×5 | on | off |
| UI / icons (pixel art) | RGBA32 or BC7, point/bilinear by window | ASTC 4×4 (check banding) | off | off |
| FX sprites | BC7 | ASTC 6×6 | on | off |
| Data textures (LUTs read on CPU) | uncompressed | uncompressed | off | **on, whitelisted** |

If a texture has no matching rule, the postprocessor logs its path and folder. It never falls back silently.

### 3.3 Meshes and animation

- **Standalone `.anim`** (for example `Animations/DarkElf/FDarkElf/Clips/FDarkElf_m000_b.ao_Social_Cheer_FDarkElf.anim`, 8.4 MB, 28,696 keys): `m_Compressed: 0`, `m_UseHighQualityCurve: 1`, every bone keyed every frame. FBX `animationCompression` does not apply to them. Run keyframe reduction (rotation, position and scale error thresholds) and drop constant curves in the converter (`docs/convert-*`), or emit FBX with Optimal compression. Expected gain is several times smaller memory and load time. [read], gain [profile]
- FBX: `optimizeGameObjects 0` (all 4,297), `importAnimation 1` on 3,894 (most are mesh-only), `importCameras`/`importLights` 1, Read/Write on 208, `meshCompression 0`. Turn on Optimize Game Objects with an **exposed-bones list per skeleton profile**: weapon, shield, bow and effect bones used by `Gear.*.FindRecursive("Shield Bone")` and similar. Keep that list as data, not as `if` checks per race. [read]
- SkinnedMeshRenderers in prefabs: `m_UpdateWhenOffscreen 0`, `m_Quality 0` (auto). Fine once the bounds are correct. [read]

### 3.4 Audio

- 566 Unity clips: all Decompress On Load, Vorbis quality 100%, preload off, no streaming. Long clips such as ambience and music should use Streaming or Compressed In Memory at quality 50–70%. [read]
- FMOD banks: ~20 MB in `StreamingAssets` (Music 11 MB, Amb 6 MB). On Android these sit inside the APK; the FMOD loader handles that, but the custom `File.*` readers do not (§5). [read]

### 3.5 Memory budgets (proposal; validate with Memory Profiler, which is installed)

| Platform class | Total app RSS | Textures | Meshes | Anim clips | Audio |
|----------------|---------------|----------|--------|------------|-------|
| Desktop min-spec (8 GB RAM, 2 GB VRAM) | ≤ 3 GB | ≤ 1.2 GB VRAM | ≤ 300 MB | ≤ 300 MB | ≤ 100 MB |
| Mac Apple Silicon 8 GB (unified) | ≤ 2.5 GB | ≤ 800 MB | ≤ 250 MB | ≤ 250 MB | ≤ 80 MB |
| Android mid (4–6 GB) | ≤ 1.5 GB | ≤ 500 MB | ≤ 150 MB | ≤ 150 MB | ≤ 50 MB |
| iOS (A13+, 4 GB) | ≤ 1.4 GB (stay well under jetsam) | ≤ 450 MB | ≤ 150 MB | ≤ 150 MB | ≤ 50 MB |

---

## 4. Is the GPU used well? APIs, tiers, pacing, scaling, 4K

| Platform | Configured | Comment | Tag |
|----------|-----------|---------|-----|
| Windows | **D3D11 only**, automatic off (`m_APIs: 02000000`) | Add DX12 as an opt-in and A/B it: graphics jobs plus split submission help in draw-call-heavy crowds. Keep DX11 as the documented fallback, and log which API was chosen at boot. | [read], gain [profile] |
| macOS | not listed, so Automatic (Metal) | Metal is right. Graphics jobs are **off** for Mac; try them. Build as Apple Silicon or Universal (set in the build profile, not in ProjectSettings). | [read] |
| Linux | not listed, so Automatic (Vulkan, then GL) | OK. Graphics jobs on. | [read] |
| Android | Vulkan + GLES3, Automatic | OK. Graphics jobs off; try them on Vulkan. Swappy on (good). ARM64 only (good). Min SDK 23; target SDK unset (Play requires the current level). | [read] |
| iOS | Metal | OK. Min iOS 13. | [read] |

- **GPU skinning:** on (compute). GLES3.0 devices without compute fall back to CPU skinning; filter them with a Play Store device filter or a GLES3.1 minimum. [read]
- **Frame pacing:** `FpsLimiter` hard-codes 60 fps with vSync off, and `enableFrameTimingStats: 0` (`ProjectSettings.asset:155`), so `FrameTimingManager` has no data for dynamic resolution or for pacing telemetry. Replace both with a `FramePacingProfile` per tier: vSync or target rate, 30/60/120, and an unfocused rate. [read]
- **Resolution scaling:** `m_RenderScale 1`, upscaling filter Auto, cameras `m_AllowDynamicResolution: 0`. UI Toolkit draws at native resolution in its own overlay, so render scale only affects the 3D world, which is what the "3D Render Scale" option added by some modern Interlude clients does (not retail Interlude). Recommended: render scale per tier (mobile 0.7–0.85, 4K desktop 0.67–0.77) with FSR 1 (spatial, cheap) or STP (Unity 6 temporal, compute-capable devices only), driven by FrameTimingManager. [read], gain [profile]
- **4K readiness:** `GameSettings.Resolutions` is a fixed list that stops at 2560×1440 instead of using `Screen.resolutions`. `resizableWindow: 0`. The UI uses ScaleWithScreenSize from 1280×720, which produces a non-integer 3× scale of Interlude pixel-art textures at 4K and blurs them. See §6 for integer-step UI scaling. [read]
- **Not used today:** Adaptive Performance (package missing), GPU Resident Drawer, GPU occlusion, PSO/shader prewarm (no `.shadervariants` and no GraphicsStateCollection; first-use compile hitches on Metal and Vulkan are likely [profile]).

**Answer: no, the GPU is not used optimally yet.** One heavy pipeline asset serves every tier, Windows is limited to DX11, there is no upscaling, opaque and depth copies run unconditionally, the deco layers are not instanced or culled, and there is no crowd tiering.

---

## 5. Multi-platform readiness

| Area | Finding | Tag | Fix (shared mechanism) |
|------|---------|-----|------------------------|
| Input | New Input System only (`activeInputHandler: 1`). One action map `InGame` (76 actions) with a single control scheme `KBM`: 45 keyboard bindings, 4 mouse, 1 VirtualMouse. **No Touch or Gamepad schemes.** `PollInput` reads `Keyboard.current`/`Mouse.current` directly in 4 files. | [read] | Add Touch and Gamepad control schemes to the same actions. Touch layouts use Input System `OnScreenStick`/`OnScreenButton` bound to `Move`, `Attack`, `NextTarget` and the skillbar actions. Replace `PollInput` key polling with actions. |
| Windows-only API | `Fineage.Game/Input/NativeFunctions.cs` uses `user32` `SetCursorPos`/`GetCursorPos` under `#if UNITY_STANDALONE_WIN` (also `UI/L2GameUI.Input.Core.cs:13,23`). On every other platform `SetCursorPos` returns `true` and does nothing: a **silent fallback**. | [read] | `Mouse.current.WarpCursorPosition` works on Win, Mac and Linux. Expose `CanWarpCursor` in a `PlatformCapabilities` profile, and log once when it is unsupported. |
| Platform defines | 144 `#if UNITY_EDITOR`, 3 `UNITY_STANDALONE_WIN`, no Android/iOS defines, 0 `Application.platform` checks. | [read] | Good baseline. Keep platform differences in profiles. |
| StreamingAssets | `Fineage.Game/Rendering/Lut/*` and `Database/L2CelestialLut.Load.Core.cs` call `File.Exists`/`File.ReadAllText` on `Application.streamingAssetsPath`. **On Android that path is inside the APK, so the reads always fail.** `L2HazeLut.Core.cs` returns silently (its warning is commented out). | [read] | One `IStreamingAssetReader` (UnityWebRequest on Android, File elsewhere, chosen once at boot) that logs any missing file with its full path. |
| Editor-only paths at runtime | `Rendering/Effects/V2/Composite/CompositeEffectPrefabYamlParser.cs` reads `Application.dataPath/Resources/Data/Effects/*.prefab` YAML. That works only in the Editor; in a player it returns `false` silently. `Fineage.Networking/L2DiagPaths.cs` writes logs to the parent of `dataPath`, which is read-only on iOS, Android and signed macOS bundles. | [read] | Bake the composite data into an asset at import. Diagnostics go to `Application.persistentDataPath`. |
| FMOD | `Plugins/FMOD/platforms/android` contains only `src/PlatformAndroid.cs`. The Android native libraries (`fmod.aar`/`libfmod*.so`) are **missing**. Win, Mac, Linux and iOS libraries are present. | [read] | Add the FMOD Android plugin. Have FMOD init failure log the platform and missing library. |
| Scripting backend | Standalone: Mono (default, unset). Android: IL2CPP, stripping Low. iOS: IL2CPP (forced). **No `link.xml`.** VContainer (reflection unless its source generator is enabled), Newtonsoft.Json (`Plugins/NuGet`), HtmlAgilityPack XPath and 24 files using reflection are stripping and AOT risks. | [read] | `link.xml` for VContainer-injected types and Json DTOs; enable VContainer codegen; add a CI IL2CPP smoke build (Android) that boots to the lobby. Use IL2CPP for desktop release builds as well. |
| Orientation / safe area | `defaultScreenOrientation: 4` (auto-rotate, portrait allowed). `androidRenderOutsideSafeArea: 1`. **No `Screen.safeArea` use.** | [read] | Landscape-only for phones. Add a `SafeAreaRoot` (§6). |
| Window / desktop | `fullscreenMode: 3` (windowed), `resizableWindow: 0`, `runInBackground: 1`, default 1024×768. | [read] | Make the window resizable and drive layout from the UI profile. |
| Build list | `EditorBuildSettings.asset` contains `Game.unity` twice (one entry disabled) plus many tile scenes from `Resources/Scenes`. | [read] | Move to Addressables scenes. |

---

## 6. UI per platform: one mechanism, per-platform layout profiles

**Today** [read]:

- UI Toolkit with one `PanelSettings` (`Resources/Data/UI/_Elements/Lineage2.asset`): ScaleWithScreenSize, reference 1280×720, match 0.5, reference DPI 96.
- Window positions are baked into UXML/USS (74 files with `position: absolute`) and set in code (`style.left` 44 sites, `style.top` 37), for example `UI/L2Window.Drag.cs:43` and `UI/Game/Skillbar/SkillbarWindow.Dock.Core.cs:35`.
- Windows are registered in `UI/L2GameUI.Load.Core.cs` as `if (Service != null) Service.AddWindow(root)`. A missing service means a missing window with **no log**, which breaks the no-silent-fallback rule.
- No safe area, DPI or touch handling.
- Nameplates and bubbles are drawn by a custom batched drawer, not per-entity VisualElements. That is good for crowds and works on every platform.

**Proposed architecture:**

```text
PlatformCapabilities (resolved once at boot; logged)
  pointer: Mouse | Touch | Gamepad, screenDiagonalInches (Screen.dpi), aspect, safeArea, hasHover
        │
        ▼
UiLayoutResolver ──selects──► UiLayoutProfile (ScriptableObject; user can override in Options)
                                 ├─ id: Desktop | TouchPhone | TouchTablet
                                 ├─ panelScale: IntegerSteps(1x/2x/3x by height) | ConstantPhysicalSize(dpi)
                                 ├─ themeSheets: [interlude.uss, touch-hit-targets.uss]
                                 ├─ inputScheme: KBM | Touch | Gamepad (Input System control scheme)
                                 └─ windows: WindowLayoutEntry[]
                                       windowId (stable, = interface.xdat control name)
                                       anchor (9-point) + offset (reference px)
                                       sizeOverride?, visibleByDefault, uxmlVariant? (touch skillbar, etc.)
                                       dockGroup (e.g. bottom-right action cluster on touch)
        │
        ▼
L2Window.ApplyLayout(WindowLayoutEntry)   ← windows never read the platform
SafeAreaRoot (VisualElement)              ← padding from Screen.safeArea via RuntimePanelUtils;
                                            reapplied on resolution/orientation change
User-moved positions                      ← saved per (profileId, windowId), applied on top of profile
```

Rules:

- **Desktop profile = Interlude parity.** Anchors and sizes come from `interface.xdat` (control name, size, anchor). The window id matches the xdat control name, so the profile can be generated from xdat by an editor tool and is not hand-made.
- **Touch profiles are a deliberate divergence** (Interlude has no touch UI). They need a ticket that records the divergence, per `AGENTS.md`. The window content is the same; only the placement, a larger theme for hit targets, and touch-specific UXML variants (skillbar, on-screen joystick, action cluster) differ.
- **No platform `if`s.** Only `UiLayoutResolver` looks at `PlatformCapabilities`; every window consumes its `WindowLayoutEntry`.
- **No silent fallbacks.** If a profile has no entry for a window, log an error with the window id and profile id, then show the window at its UXML default so the gap is visible. `L2GameUI.LoadUI` logs every service that is null.
- **DPI / 4K:** on desktop, use integer UI scale steps (1× up to about 1080p, 2× at 1440p–4K, with a user override) so Interlude bitmaps stay crisp. On touch, use ConstantPhysicalSize with a minimum hit target of about 7–9 mm. A UI scale option does not exist in Interlude; document it as a divergence.
- **Safe areas:** every window parents under `SafeAreaRoot`. Fullscreen backgrounds and world-space nameplates stay outside it.

---

## 7. Prioritized roadmap (small PRs)

### 7.1 Frame-time budgets (targets for measurement)

| Platform class | Target fps | Frame | Main thread | Render thread | GPU | Siege floor |
|----------------|-----------|-------|-------------|---------------|-----|-------------|
| Desktop min-spec (4-core, GTX 1050/RX 560 class) | 60 | 16.6 ms | ≤ 10 ms | ≤ 6 ms | ≤ 14 ms | 30 fps |
| macOS Apple Silicon (M1 base) | 60 (120 on ProMotion if headroom) | 16.6 ms | ≤ 10 ms | ≤ 6 ms | ≤ 14 ms | 30 fps |
| Android mid (Snapdragon 7-series / Mali-G7x) | 30 | 33.3 ms | ≤ 18 ms (thermal headroom) | ≤ 10 ms | ≤ 22 ms | 25 fps |
| iOS (A13+) | 30 / 60 by tier | 33.3 / 16.6 ms | ≤ 18 / 10 ms | ≤ 10 / 6 ms | ≤ 22 / 14 ms | 25 fps |

Benchmarks: (a) Giran town square at peak (about 300 pawns in view) and (b) a siege field with 500+ pawns and continuous skill FX. Run both as a deterministic **offline crowd replay**: recorded or synthetic zone packets fed into `NetSession`, extending the Gremlin smoke (#129). They should run in CI on desktop and manually on devices.

### 7.2 Steps

Gain and cost are expected values from code reading; every "gain" is **[profile]** until measured.

| # | PR-sized step | Gain | Cost | How to measure |
|---|---------------|------|------|----------------|
| **P0 — measure and correctness** |||||
| 1 | Add `ProfilerMarker`s: `Fineage.Net.Read`, `.Decode`, `.Drain`, `.Apply`; `Fineage.Move.Step`; `Fineage.Nameplates.Update`; `Fineage.ActorLights.Push`; `Fineage.Spawn`; `Fineage.Tile.Load`; `Fineage.Crowd.Tier`. Enable `enableFrameTimingStats`. Add a debug overlay with CPU/GPU ms from FrameTimingManager. | Enabler | S | Markers visible in Profiler and Profile Analyzer; overlay works in a player build |
| 2 | Offline crowd replay bench (synthetic N=100/300/1000 pawns moving and casting) as a Gremlin scenario. | Enabler | M | Bench prints p50/p95/p99 frame ms and per-marker ms |
| 3 | `NetMainThreadPump`: no Unity API off the main thread (use a `Stopwatch` timestamp); make `EntitySpawn` (and other state-creating opcodes) non-droppable; log drop counts per opcode. | Correctness in sieges | S | Unit test that posts 2× `MaxPendingNormal` from a worker thread: no exception, spawns delivered |
| 4 | Quality wiring: per-tier URP assets (Mobile-Low, Mobile-High, Desktop-Low/Med/High/Ultra; reuse `URP_UltraLow` for the lowest), each with MSAA, opaque/depth copy, SSAO, render scale and shadow res set. Fix `m_PerPlatformDefaultQuality` (Standalone → High, Android/iOS → Mobile tiers). Add a `QualityProfile` resolved at boot and logged. | Large GPU win on mobile and low-end; fixes "starts at Very Low" | S | GPU ms per tier in the Frame Debugger and RenderDoc/Xcode; texture mip level in a build |
| 5 | Release log hygiene: `m_StackTraceTypes` None for Log/Warning in release; strip unused post variants. | Small CPU win, smaller build | S | `Fineage.*` markers; build size report |
| **P1 — CPU hot paths** |||||
| 6 | Nameplates: `OverlapSphereNonAlloc`, cached delegates, upsert only entities in range (from the crowd tier array once #13 lands; a spatial grid until then). | O(N) → O(visible) per frame | S | `Fineage.Nameplates.Update` ms at N=1000; GC Alloc = 0 B/frame |
| 7 | `L2ActorPointLights`: register and unregister on spawn/despawn instead of `FindObjectsByType` each second; write per-actor light data through a structured buffer or global arrays indexed per actor so SMRs keep SRP Batcher compatibility. | Removes 1 Hz spikes; restores SRP batching | M | Spike in `Fineage.ActorLights.Push`; "SRP Batcher compatible" count in the Frame Debugger |
| 8 | `Gear.LateUpdate`: cache the shield transform at equip time; no per-frame `name`. | Allocation-free per pawn | S | GC Alloc in LateUpdate at N=300 |
| 9 | Packet pipeline: reuse the header buffer, rent the body from `ArrayPool`, decode on the socket thread into pooled messages, coalesce position/status latest-wins per object, budget the main-thread apply in ms. | Main-thread ms and GC in sieges | M | `Fineage.Net.*` ms and GC Alloc/frame at 2k packets/s |
| 10 | Consolidate per-pawn callbacks (`NetworkTransformReceive`, `Gravity*`, `L2PawnYaw`, `MoveNpc`) into `MoveAllCharacters` ticks; move remote motion from `FixedUpdate` to `Update`; set `maximumDeltaTime` to 0.1 in `TimeManager` and stop setting it from code. | Fewer callbacks; no fixed-step spiral | M | `PlayerLoop` FixedUpdate count per frame; `Fineage.Move.Step` ms at N=1000 |
| 11 | Burst/Jobs movement and yaw (`IJobParallelForTransform`) plus batched ground `RaycastCommand`. | Spreads movement across cores | M | Main-thread `Fineage.Move.Step` ms vs. worker time |
| 12 | Async tile streaming: no sync `Resources.Load`; `CanLoadTile` uses a manifest. | Removes multi-100 ms hitches | M | `Fineage.Tile.Load` max ms; p99 frame time while crossing tiles |
| **P2 — crowd and GPU** |||||
| 13 | Crowd tier system, step 1: Burst tier assignment plus `CrowdLodProfile` assets; tiers drive nameplates, FX and attachments. | Foundation for 14–16 | M | Tier histogram overlay; `Fineage.Crowd.Tier` ms |
| 14 | Crowd step 2: Animator update-rate throttling for T2 (manual PlayableGraph or stepped speed); fix SMR bounds and drop `CullUpdateTransforms` for off-screen pawns. | Animation CPU drops with distance | M | `Animator.Update`/`PreLateUpdate.DirectorUpdateAnimation*` ms at N=300 |
| 15 | Crowd step 3: merge parts into one SMR per pawn at equip time (texture array/atlas) + FBX Optimize Game Objects with an exposed-bone list per skeleton profile. | About 5–7× fewer skinned draws and transforms per pawn | L (split per race) | Draw calls and SetPass in the Frame Debugger; Transform count; skinning GPU ms |
| 16 | Crowd step 4: VAT-baked far tier, drawn instanced. | 1000+ pawns affordable | L | GPU ms at N=1000 with T3 active |
| 17 | FX budget profile: max concurrent effects per tier, others' FX distance and priority (own/party/target first). Verify against Interlude options in xdat. | Caps siege overdraw | M | GPU ms and overdraw view in the siege bench |
| 18 | Deco: GPU Resident Drawer trial (add `DOTS_INSTANCING_ON` to `L2StaticMeshModulate2X`, no MPBs on deco) **or** convert the remaining DecoLayers to `L2InstancedPropRenderer` with a Burst frustum-cull job. | Large CPU render-thread win in towns | M–L | Batches and render-thread ms in 16_25 |
| 19 | Render scale + FSR1/STP per tier; dynamic resolution from FrameTimingManager on mobile and 4K. | GPU-bound frames at 4K and mobile | M | GPU ms at 4K native vs. 0.67 |
| 20 | Windows DX12 opt-in, graphics jobs on Mac/Android/iOS (A/B); PSO prewarm via GraphicsStateCollection. | Render-thread ms; fewer first-use hitches | M | Render-thread ms; hitch count in the first 2 min |
| **P3 — assets and platforms** |||||
| 21 | Texture import policy postprocessor + presets (R/W off except a whitelist, ASTC/BC7, streaming mips on). Reimport in chunks by folder. | Memory roughly halved where R/W was on; mobile-ready formats | M (long reimport) | Memory Profiler texture totals; build size |
| 22 | Animation keyframe reduction in the converter (start with one race). | Clip memory and load time | M | Memory Profiler AnimationClip total; clip size on disk |
| 23 | Addressables migration out of `Resources` (Maps tiles first, then animations per race, then effects). Remote groups for mobile. | Enables mobile size and memory; faster startup | L (staged) | Startup ms to lobby; resident memory; APK/IPA size |
| 24 | Platform blockers: FMOD Android libraries, `IStreamingAssetReader`, `L2DiagPaths` to `persistentDataPath`, composite-effect data baked at import, cursor warp through the Input System, landscape-only phones. | Android/iOS boot | S each | Android/iOS player boots to the lobby with sound; no missing-file errors |
| 25 | IL2CPP readiness: `link.xml`, VContainer codegen, IL2CPP Android CI smoke build. | Prevents runtime stripping crashes | S–M | CI job green; boots to the lobby |
| 26 | UI: `PlatformCapabilities` + `UiLayoutProfile` + `SafeAreaRoot` + Desktop profile generated from xdat (no visual change). | Foundation for touch | M | Desktop pixel diff = 0 vs. current |
| 27 | UI: TouchPhone profile + Touch control scheme + on-screen controls (ticketed divergence). | Mobile playable | M–L | Device playtest; hit-target size check |

**Suggested order:** 1 → 3 → 4 → 2 → 6/8 → 9 → 10 → 13 → 14 → 21 → 12 → 15 → 18 → 19, with the platform blockers (24, 25) running in parallel once mobile work starts.

---

## 8. What needs on-device profiling before committing

- Real cost per pawn of the per-entity callbacks and the Animator at N=100/300/1000 (desktop min-spec, M1, a mid Android device, iPhone 11).
- GPU split: SSAO, opaque/depth copy, L2Fx compositor, decals and transparent FX overdraw in the siege bench, captured with RenderDoc, Xcode GPU capture and Snapdragon Profiler/AGI.
- Whether the remaining 2 custom render passes without `RecordRenderGraph` run in a player build.
- Shader and PSO first-use hitches on Metal and Vulkan.
- Memory totals: textures with and without R/W, animation clips before and after reduction, Resources index overhead.
- Thermal behaviour on mobile over a 20-minute siege at 30 fps.
- Whether DX12 or graphics jobs help or hurt per platform.
