# AI-assisted asset remaster (research notes)

Status: research only, not scheduled. Goal: higher-fidelity look (4K-ready) for characters, weapons, environment and effects without a full manual art pass.

## Tools surveyed (2026-11)

| Tool | What it does | Fit for Fineage |
|------|--------------|-----------------|
| [NVIDIA RTX Remix AI Texture Tools](https://docs.omniverse.nvidia.com/kit/docs/rtx_remix/latest/docs/howto/learning-aitexturetools.html) (PBRFusion) | Batch 4x texture upscale + generates albedo, normal, roughness, height maps. "Quality" mode tiles 256x256 and keeps full resolution. | Best value for environment, armor, props. Batch pipeline, local GPU. |
| [Meshy AI Texturing](https://docs.meshy.ai/en/webapp/guides/3d-model/ai-texturing) | Re-textures an existing model from a text prompt or reference image, keeps UVs, 2K/4K/8K, optional PBR maps and lighting removal. Paid credits. | Characters and weapons restyle. |
| [ReskinIT (Unity Asset Store)](https://assetstore.unity.com/packages/tools/generative-ai/reskinit-ai-texturing-decals-377408) | In-editor reskin / tileable PBR / decals, UV- and shape-aware, bring-your-own Gemini key. | Quick iteration inside Unity. |
| [Tripo](https://www.tripo3d.ai/blog/improve-redo-textures) | Texture regen, local "magic brush" fixes, AI upscale, PBR generator. | Alternative to Meshy. |
| Reference case: [Painkiller RTX](https://developer.nvidia.com/blog/how-painkiller-rtx-uses-generative-ai-to-modernize-game-assets-at-scale/) | Small modding team batch-converted thousands of legacy textures to PBR with PBRFusion. | Proof the batch approach scales. |

## Notes / limits

- These tools change textures, not geometry. For 4K, low-poly silhouettes stay visible: needs higher-detail meshes or at least baked normal maps.
- Original textures are diffuse-only with baked lighting; generated PBR sets require URP Lit materials and per-asset QA under new lighting.
- Effects (skills, enchant glow) are better rebuilt in VFX Graph / shaders than passed through image models.
- Output size: 4K PBR sets multiply memory/disk; needs per-platform texture budgets and streaming (see client performance audit).
- Likely pipeline: batch PBR + upscale for environment -> restyle heroes (characters, weapons) with a texture model -> manual pass on hero assets -> per-platform import presets (ASTC mobile, BC7 desktop).
