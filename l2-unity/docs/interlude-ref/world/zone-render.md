# Zone render catalog

Derived from `world/maps/*.unr.json.gz` and `world/env/*.unr.json.gz`.
The gzip dumps stay the lossless source.
An empty cell means the property was not authored on that actor.
Engine.u `ZoneInfo` defaultproperties are not serialized in the map, so they are
not copied into this catalog. When `fog_start` is empty the runtime applies the
class default `DistanceFogStart=3000`.

Columns: `map`, `class`, `name`, `zone_tag`, `b_distance_fog`, `b_terrain_zone`, `fog_start`, `fog_end`, `fog_r`, `fog_g`, `fog_b`, `fog_a`, `tex_u_pan`, `tex_v_pan`, `lens_flares`, `lens_offsets`, `lens_scales`, `radius`, `limit_max_radius`, `b_directional`, `draw_scale`, `light_brightness`, `rot_pitch`, `rot_yaw`, `rot_roll`.

`NSun` rotation and `NMovableSunLight` brightness stay in the table.
Disc direction and color stay on the day/night LUT. `Radius` is a light radius;
the runtime scales the captured disc by `tileRadius / catalogMode`.
`Engine.Flare*` textures are not in Resources. A missing flare is logged by name.
