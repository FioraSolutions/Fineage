using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ModelTable : AbstractCache
{
    private void CacheFaces()
    {
        _faces = new GameObject[RACE_COUNT, FACE_COUNT];

        for (int r = 0; r < RACE_COUNT; r++)
        {
            CharacterRaceAnimation raceId = (CharacterRaceAnimation)r;
            string folder = RaceFolder(raceId);
            for (int f = 0; f < FACE_COUNT; f++)
                _faces[r, f] = Resources.Load<GameObject>(folder + "/Faces/" + raceId + "_f_" + f);
        }
    }

    private void CacheHair()
    {
        _hair = new GameObject[RACE_COUNT, HAIR_STYLE_COUNT * HAIR_COLOR_COUNT * 2];

        for (int r = 0; r < RACE_COUNT; r++)
        {
            CharacterRaceAnimation raceId = (CharacterRaceAnimation)r;
            string folder = RaceFolder(raceId);
            for (int style = 0; style < HAIR_STYLE_COUNT; style++)
            {
                for (int color = 0; color < HAIR_COLOR_COUNT; color++)
                {
                    int index = style * HAIR_STYLE_COUNT + color * 2;
                    string hairPrefix = folder + "/Hair/" + raceId + "_h_" + style + "_" + color;
                    _hair[r, index] = Resources.Load<GameObject>(hairPrefix + "_ah");
                    _hair[r, index + 1] = Resources.Load<GameObject>(hairPrefix + "_bh");
                }
            }
        }
    }

    private void CacheWeapons()
    {
        _weapons = LoadItemModels(
            TablePacketServices.Items.Weapons,
            weapon => weapon.Weapongrp != null ? weapon.Weapongrp.Model : null,
            "weapon");
    }
}
