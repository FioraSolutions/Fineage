# Interlude sound dumps and derived tables

See `../world/README.md` and `../world/gap-report.md`.

`anim-sequence-notifies.tsv.gz` binds each MeshAnimation sequence to official notify times.
The runtime copy is `StreamingAssets/Data/Meta/AnimNotifies/anim-sequence-notifies.tsv`.

`official-waves.tsv` maps every official item, monster, and character wave
(`itemsound*`, `EtcSound`, `monsound*`, `chrsound*`) to its Resources path.
The runtime copy is `StreamingAssets/Data/Meta/Sounds/official-waves.tsv`.

`interfacesound` and `messengersound` waves are in the same `official-waves.tsv`.
System-message `item_sound` tokens are in `StreamingAssets/Data/Meta/systemmsg-e.tsv`.
Window open/close tokens are in `StreamingAssets/Data/Meta/Sounds/interface-window-sounds.tsv`.

`ambsound` and `AmbSound2` waves are in the same `official-waves.tsv` under `Data/Sounds/AmbSound/` and `Data/Sounds/AmbSound2/`.
AmbientSoundObject placement for every official map (185277 actors, including Lobby) is `StreamingAssets/Data/Meta/Sounds/official-ambient-objects.tsv.gz`, played by `OfficialAmbientSoundPlayback`.
