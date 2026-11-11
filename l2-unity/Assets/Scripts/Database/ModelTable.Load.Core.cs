using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ModelTable : AbstractCache
{
    public void Initialize()
    {
        CacheRaceContainers();
        CacheFaces();
        CacheHair();
        CacheWeapons();
        CacheEtcItems();
        CacheArmors();
        CacheNpcs();
    }

    private static string RaceFolder(CharacterRaceAnimation raceId)
    {
        CharacterRace race = CharacterRaceParser.ParseRace(raceId);
        return "Data/Animations/" + race + "/" + raceId;
    }

    private void CacheRaceContainers()
    {
        _playerContainers = new GameObject[RACE_COUNT];
        _userContainers = new GameObject[RACE_COUNT];
        _pawnContainers = new GameObject[RACE_COUNT];

        for (int r = 0; r < RACE_COUNT; r++)
        {
            CharacterRaceAnimation raceId = (CharacterRaceAnimation)r;
            string folder = RaceFolder(raceId);
            _playerContainers[r] = Resources.Load<GameObject>(folder + "/Player_" + raceId);
            _userContainers[r] = Resources.Load<GameObject>(folder + "/User_" + raceId);
            _pawnContainers[r] = Resources.Load<GameObject>(folder + "/Pawn_" + raceId);
        }
    }
}
