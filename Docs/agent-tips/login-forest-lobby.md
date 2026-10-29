# Login forest Lobby (Interlude Lobby.unr)

- `LoginForestPresenter` (called from `LoginCameraManager.Start`) builds the forest path vista on layer 17 for the `Login` camera only; placements live in `Resources/Data/Maps/l2_lobby/Meta/LoginForestActors.json`.
- The far void is a camera-anchored quad with `L2_Lobby_T/Jungle03` (backdrop). Keep `RenderSettings.fog` **off** here: URP linear fog washes Unlit backdrop + far actors to the fog colour (looks like a white void).
- Proof shot: `echo ~/Desktop/fineage-login-forest.png > /tmp/fineage-login-forest.outpath`, then open the Editor on `l2-unity` (not batchmode). `LoginForestShot` enters Play on Menu, waits for LoginWnd, writes the PNG and quits (~50 s with a warm Library).
- Unity reimport dirties lots of unrelated `field_deco_S/*.fbx.meta` / terrain `.mat` files; never stage those with a Lobby change.
