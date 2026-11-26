# Login lobby — numeric check against Lobby.unr

Source dump: `l2-unity/docs/interlude-ref/world/env/Lobby.json` (lossless property export).
Shared placement: `L2Ue2ActorXform` (position, rotator, DrawScale×DrawScale3D, PrePivot) and `L2Ue2Terrain` (height and corner).
EditMode: `LoginLobbyDumpEditModeTests`.

## Camera

| | original | Unity |
|---|---|---|
| actor | `InterpolationPoint5` | login camera transform |
| Location UU | (-8316.73828, 238312.094, -6687.98975) | (4539.2780, -127.3903, -158.4141) m |
| Rotation UU (pitch, yaw, roll) | (65508, 98304, 0) | euler (-359.846, 540.000, -0.000) ° |
| FOV | 50° horizontal (`PlayerController.uc` FovAngle) | vertical 29.395° at 16:9, 38.553° at 4:3 |
| why this actor | SceneManager0 Tag=Logon_Warp Actions has array_size but no object_array. InterpolationPoint5 is the only InterpolationPoint whose horizontal forward aims at StaticMeshActor73 lobby_back within 3 deg (error 1.98 deg, distance 1474 UU). Camera25 Tag=U2Viewport2 is the UnrealEd viewport and is not a candidate. | |

Aim error to `StaticMeshActor73` lobby_back is 1.98°. The old presenter used Camera25 (Tag `U2Viewport2`) and a vertical FOV of 35 with no source.

## Terrain

TerrainInfo0 Location (-16384.0, 245760.0, 160.651276), TerrainScale (64.0, 64.0, 128.0), sectors 256.
DrawScale 10.0 is stored and not applied.
Height = Location.Z + (sample − 32768) × (TerrainScale.Z / 256). Height.bmp is bottom-up; file row = (height − 1 − v).
At the login eye, texel (254, 11) sample 19431 sheet Z -6507.85; sheet − eye = 180.1 UU. The eye is under the sheet. The sheet is still drawn.
Far static-mesh bases (8–10k UU) sit on this sheet within about 2 UU. Bases inside 2k UU of the eye sit about 370–385 UU below it. That residual is in the dump, not a per-lobby offset.

Layer alpha maps are the `AlphaMap` fields on TerrainInfo0 in `l2_lobby.t3d` (Lobby_terrain_t.D_M_01, D_M_02, DW_M_03, and the rest of the layer list). A missing alpha file is already a named error in `L2TerrainInfoParser`.

## Thirty largest static meshes

Size is max(|DrawScale×DrawScale3D|). Unity local scale includes the +90° mesh yaw fix used for Interlude FBX. PrePivot is absent on these actors. `sheet−base` is the height residual in UU.

| actor | mesh | UE location | UE rotator | UE scale | Unity position m | Unity euler | Unity local scale | sheet−base UU |
|---|---|---|---|---|---|---|---|---|
| StaticMeshActor73 | lobby_back | (-9790.0, 238363.0, -6520.0) | (0, -49152, 0) | (5.200, 4.000, 5.400) | (4540.25, -124.19, -186.48) | (-0.00, -270.00, -0.00) | (5.200, 5.400, 4.000) | 186.7 |
| StaticMeshActor695 | deco_Doren_stone003 | (-10119.3, 249760.3, -6489.9) | (0, 0, 0) | (3.000, 3.000, 3.000) | (4757.34, -123.62, -192.75) | (-0.00, 0.00, -0.00) | (3.000, 3.000, 3.000) | 2.6 |
| StaticMeshActor145 | Dwarf_stone04 | (-22418.5, 239226.2, -6260.1) | (0, 0, 5116) | (2.300, 2.300, 2.300) | (4556.69, -119.24, -427.02) | (-0.00, 0.00, -28.10) | (2.300, 2.300, 2.300) | 331.3 |
| StaticMeshActor1098 | grass001 | (-15219.9, 245647.2, -6273.0) | (0, 12308, 0) | (2.000, 2.000, 2.000) | (4678.99, -119.49, -289.90) | (-0.00, 67.61, -0.00) | (2.000, 2.000, 2.000) | -1.8 |
| StaticMeshActor1075 | grass010 | (-15058.7, 245328.7, -6367.2) | (0, -4812, 0) | (2.000, 2.000, 2.000) | (4672.93, -121.28, -286.83) | (-0.00, -26.43, -0.00) | (2.000, 2.000, 2.000) | -1.1 |
| StaticMeshActor1074 | grass010 | (-15060.0, 245305.9, -6367.2) | (0, 4612, 0) | (2.000, 2.000, 2.000) | (4672.49, -121.28, -286.86) | (-0.00, 25.33, -0.00) | (2.000, 2.000, 2.000) | -11.1 |
| StaticMeshActor1117 | Oren_elf_grass006 | (-10288.0, 247189.9, -6476.6) | (0, 10100, 0) | (2.000, 2.000, 2.000) | (4708.38, -123.36, -195.96) | (-0.00, 55.48, -0.00) | (2.000, 2.000, 2.000) | -155.7 |
| StaticMeshActor1064 | grass010 | (-15046.8, 245336.0, -6367.2) | (0, 2460, 0) | (2.000, 2.000, 2.000) | (4673.07, -121.28, -286.60) | (-0.00, 13.51, -0.00) | (2.000, 2.000, 2.000) | -1.1 |
| StaticMeshActor1069 | grass010 | (-15079.0, 245302.9, -6367.2) | (0, 12348, 0) | (2.000, 2.000, 2.000) | (4672.44, -121.28, -287.22) | (-0.00, 67.83, -0.00) | (2.000, 2.000, 2.000) | -11.1 |
| StaticMeshActor1067 | grass010 | (-15033.1, 245317.6, -6367.2) | (0, 11884, 0) | (2.000, 2.000, 2.000) | (4672.72, -121.28, -286.35) | (-0.00, 65.28, -0.00) | (2.000, 2.000, 2.000) | -1.1 |
| StaticMeshActor510 | grass001 | (-15316.4, 245071.9, -6326.2) | (0, 0, 0) | (2.000, 2.000, 2.000) | (4668.04, -120.50, -291.74) | (-0.00, 0.00, -0.00) | (2.000, 2.000, 2.000) | -66.7 |
| StaticMeshActor1095 | grass001 | (-15045.4, 244686.9, -6431.2) | (0, 0, 0) | (2.000, 2.000, 2.000) | (4660.70, -122.50, -286.58) | (-0.00, 0.00, -0.00) | (2.000, 2.000, 2.000) | 2.3 |
| StaticMeshActor1071 | grass010 | (-15077.7, 245325.7, -6367.2) | (0, 2616, 0) | (2.000, 2.000, 2.000) | (4672.87, -121.28, -287.19) | (-0.00, 14.37, -0.00) | (2.000, 2.000, 2.000) | -1.1 |
| StaticMeshActor1081 | grass001 | (-14838.7, 244849.9, -6427.4) | (0, 0, 0) | (2.000, 2.000, 2.000) | (4663.81, -122.43, -282.64) | (-0.00, 0.00, -0.00) | (2.000, 2.000, 2.000) | -3.9 |
| StaticMeshActor1099 | grass001 | (-15228.9, 245622.2, -6273.0) | (0, 0, 0) | (1.700, 1.700, 1.700) | (4678.52, -119.49, -290.07) | (-0.00, 0.00, -0.00) | (1.700, 1.700, 1.700) | -9.8 |
| StaticMeshActor1096 | grass001 | (-14949.9, 245243.2, -6377.0) | (0, 0, 0) | (1.700, 1.700, 1.700) | (4671.30, -121.47, -284.76) | (-0.00, 0.00, -0.00) | (1.700, 1.700, 1.700) | -6.3 |
| StaticMeshActor965 | grass001 | (-15061.3, 244626.0, -6434.2) | (0, 15024, 0) | (1.600, 1.600, 1.600) | (4659.54, -122.56, -286.88) | (-0.00, 82.53, -0.00) | (1.600, 1.600, 1.600) | -0.7 |
| StaticMeshActor1199 | Dwarf_grass004 | (-22186.1, 240025.6, -6327.2) | (0, -28512, 0) | (1.600, 1.600, 1.600) | (4571.92, -120.52, -422.59) | (-0.00, -156.62, -0.00) | (1.600, 1.600, 1.600) | -2.7 |
| StaticMeshActor1315 | Oren_elf_grass006 | (-10231.7, 247238.2, -6473.6) | (0, 24900, 0) | (1.600, 1.600, 1.600) | (4709.30, -123.31, -194.89) | (-0.00, 136.78, -0.00) | (1.600, 1.600, 1.600) | -70.7 |
| StaticMeshActor1202 | Hall_door01 | (-17296.0, 240025.9, -6029.0) | (0, 0, 0) | (1.000, 1.600, 1.000) | (4571.92, -114.84, -329.45) | (-0.00, 0.00, -0.00) | (1.000, 1.000, 1.600) | -75.8 |
| StaticMeshActor1082 | grass001 | (-14858.9, 244867.1, -6423.8) | (0, 0, 0) | (1.600, 1.600, 1.600) | (4664.14, -122.36, -283.03) | (-0.00, 0.00, -0.00) | (1.600, 1.600, 1.600) | -3.1 |
| StaticMeshActor980 | grass001 | (-14798.7, 244192.8, -6432.2) | (0, 0, 0) | (1.600, 1.600, 1.600) | (4651.29, -122.52, -281.88) | (-0.00, 0.00, -0.00) | (1.600, 1.600, 1.600) | -0.1 |
| StaticMeshActor401 | Hall_door01 | (-16358.0, 240026.0, -6029.0) | (0, 0, 0) | (1.000, 1.600, 1.000) | (4571.92, -114.84, -311.58) | (-0.00, 0.00, -0.00) | (1.000, 1.000, 1.600) | -112.8 |
| StaticMeshActor1105 | grass010 | (-15060.1, 244357.9, -6441.9) | (0, 0, 0) | (1.600, 1.600, 1.600) | (4654.44, -122.70, -286.86) | (-0.00, 0.00, -0.00) | (1.600, 1.600, 1.600) | -0.5 |
| StaticMeshActor95 | Orc_grass003 | (-21857.5, 251036.9, -6001.6) | (0, -1008, 0) | (1.500, 1.500, 1.500) | (4781.66, -114.32, -416.33) | (-0.00, -5.54, -0.00) | (1.500, 1.500, 1.500) | -2.7 |
| StaticMeshActor500 | Aden_gardengrass006 | (-8466.0, 238393.0, -6750.0) | (0, -5120, 0) | (1.500, 1.500, 1.500) | (4540.82, -128.57, -161.26) | (-0.00, -28.12, -0.00) | (1.500, 1.500, 1.500) | 338.7 |
| StaticMeshActor632 | Aden_gardengrass006 | (-8457.0, 238237.0, -6752.0) | (0, -24576, 0) | (1.500, 1.500, 1.500) | (4537.85, -128.61, -161.09) | (-0.00, -135.00, -0.00) | (1.500, 1.500, 1.500) | 405.2 |
| StaticMeshActor1155 | deco_Doren_stone004 | (-10103.9, 250038.8, -6433.5) | (0, 1160, 0) | (1.500, 1.500, 1.500) | (4762.64, -122.54, -192.45) | (-0.00, 6.37, -0.00) | (1.500, 1.500, 1.500) | 100.6 |
| StaticMeshActor876 | elf_grass003 | (-12353.8, 246428.2, -6467.2) | (0, -14060, 0) | (1.500, 1.500, 1.500) | (4693.87, -123.19, -235.31) | (-0.00, -77.23, -0.00) | (1.500, 1.500, 1.500) | 3.9 |
| StaticMeshActor924 | elf_grass003 | (-12355.0, 246421.0, -6467.2) | (0, 0, 0) | (1.500, 1.500, 1.500) | (4693.73, -123.19, -235.33) | (-0.00, 0.00, -0.00) | (1.500, 1.500, 1.500) | 3.9 |

## Emitters in the login view

In view means within the 50° horizontal FOV of the login camera and inside ZoneInfo0 fog end (10000 UU). Unity scale is the yaw-fix-0 mapping (UE.Y, UE.Z, UE.X).

| actor | UE location | DrawScale | distance UU | Unity position m | Unity scale |
|---|---|---|---|---|---|
| Emitter5 | (-9319.4, 238524.5, -6602.0) | 0.100000001 | 1025 | (4543.32, -125.75, -177.51) | (0.100, 0.100, 0.100) |
| Emitter4 | (-9320.4, 238520.6, -6601.0) | 0.100000001 | 1025 | (4543.25, -125.73, -177.53) | (0.100, 0.100, 0.100) |
| Emitter17 | (-16911.7, 239513.8, -6380.0) | 0.5 | 8679 | (4562.17, -121.52, -322.13) | (0.500, 0.500, 0.500) |
| Emitter58 | (-16787.0, 238894.6, -6399.0) | 0.5 | 8490 | (4550.37, -121.89, -319.75) | (0.500, 0.500, 0.500) |
| Emitter77 | (-16841.4, 239345.4, -6311.0) | 0.5 | 8587 | (4558.96, -120.21, -320.79) | (0.500, 0.500, 0.500) |
| Emitter2 | (-8494.8, 238343.1, -6763.6) | 1 | 181 | (4539.87, -128.83, -161.81) | (1.000, 1.000, 1.000) |
| Emitter13 | (-16838.4, 239126.2, -6311.0) | 0.5 | 8560 | (4554.78, -120.21, -320.73) | (0.500, 0.500, 0.500) |
| Emitter18 | (-8694.0, 238153.0, -6629.1) | 0.100000001 | 409 | (4536.25, -126.27, -165.60) | (0.100, 0.100, 0.100) |
| Emitter3 | (-9122.0, 238413.0, -6610.1) | 0.100000001 | 812 | (4541.20, -125.91, -173.75) | (0.100, 0.100, 0.100) |
| Emitter8 | (-9210.0, 238292.0, -6595.1) | 0.100000001 | 893 | (4538.90, -125.62, -175.43) | (0.100, 0.100, 0.100) |
| Emitter6 | (-8617.0, 238321.0, -6730.1) | 0.100000001 | 300 | (4539.45, -128.19, -164.13) | (0.100, 0.100, 0.100) |
| Emitter14 | (-9349.6, 238229.1, -6732.6) | 0.5 | 1036 | (4537.70, -128.24, -178.09) | (0.500, 0.500, 0.500) |
| Emitter22 | (-8621.6, 238179.1, -6748.6) | 0.5 | 333 | (4536.74, -128.55, -164.22) | (0.500, 0.500, 0.500) |

13 emitters are inside that frustum. The package has 33 Emitter actors; the rest are outside the login view.

## What #1793 had wrong

- Camera: the presenter used Camera25, the UnrealEd viewport (`Tag=U2Viewport2`), and a vertical FOV of 35. The gameplay FOV is the PlayerController default 50 horizontal. The warp target is not stored: `Actions` has no `object_array`.
- Scale: `(5.20, 5.40, 4.00)` on lobby_back was written as a literal. It is DrawScale 4 × DrawScale3D (1.3, 1, 1.35) after the shared +90° FBX yaw fix, i.e. Unity local (UE.X, UE.Z, UE.Y) = (5.2, 5.4, 4.0).
- Floor: the height sheet matches static-mesh bases past 8k UU. Hiding the sheet removed the floor. The login eye is under the sheet by the residual above; the sheet stays visible and the gap is logged.
