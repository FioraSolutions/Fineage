using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class CharSelectWindow
{
    public void SetCharacterList(List<CharSelectionInfoPackage> characters) {
        _characters = characters;
        string[] charArray = characters.Select(character => character.Name).ToArray();
        _charNameManipulator.UpdateValues(charArray);
    }

    public void SetInterludeCharacterList(List<CharSelectInfoPackage> characters)
    {
        _lobbyCharacters = null;
        _charactersInterlude = characters;
        string[] charArray = characters.Select(character => character.Name).ToArray();
        _charNameManipulator.UpdateValues(charArray);
        EnableInterludeButtons();
        ShowLobbyStatus("");
    }

    public void SelectSlot(int slot) {
        if (slot == _charNameManipulator.Index) {
            return;
        }

        if (slot == -1) {
            _levelLabel.text = "0";
            _classLabel.text = "??";
            _hpLabel.text = "0/0";
            _mpLabel.text = "0/0";
            _expLabel.text = "0.00%";
            _spLabel.text = "0";
            _karmaLabel.text = "0";
            _charNameManipulator.ClearInput();
        } else if(slot < _charactersInterlude.Count) {
            _selectChar = _charactersInterlude[slot];
            _levelLabel.text = _charactersInterlude[slot].Lvl.ToString();
            _classLabel.text =  ((CharacterClass)(_characters[slot].ClassId)).ToString();
            _hpLabel.text = $"{_charactersInterlude[slot].Hp}/{_charactersInterlude[slot].MaxHp}";
            _mpLabel.text = $"{_charactersInterlude[slot].Mp}/{_charactersInterlude[slot].MaxMp}";
            _expLabel.text = _charactersInterlude[slot].ExpProcent.ToString("0.00") + "%";
            _spLabel.text = _charactersInterlude[slot].Sp.ToString();
            _karmaLabel.text = _charactersInterlude[slot].Karma.ToString();

            StartCoroutine(UpdateBarsInterlude(slot));

            _charNameManipulator.SelectIndex(slot);
        }
    }

    private void SetEmptyData()
    {
        _levelLabel.text = "0";
        _classLabel.text = "";
        _hpLabel.text = "0/0";
        _mpLabel.text = "0/0";
        _expLabel.text = "0.00%";
        _spLabel.text = "0";
        _karmaLabel.text = "0";
        _charNameManipulator.ClearInput();
    }

    public void SelectInterludeSlot(int slot)
    {
        
        if (slot == _charNameManipulator.Index)
        {
            return;
        }
        
        if (slot == -1)
        {
            _levelLabel.text = "0";
            _classLabel.text = "??";
            _hpLabel.text = "0/0";
            _mpLabel.text = "0/0";
            _expLabel.text = "0.00%";
            _spLabel.text = "0";
            _karmaLabel.text = "0";
            _charNameManipulator.ClearInput();
        }
        else if (slot < _charactersInterlude.Count)
        {
            _selectChar = _charactersInterlude[slot];
            int s = _charactersInterlude[slot].Lvl;
            int classId = _charactersInterlude[slot].ClassId;
            _levelLabel.text = _charactersInterlude[slot].Lvl.ToString();
            _classLabel.text = LobbyCharacterText.ClassLabel(_charactersInterlude[slot].ClassId);
            _hpLabel.text = $"{_charactersInterlude[slot].Hp}/{_charactersInterlude[slot].MaxHp}";
            _mpLabel.text = $"{_charactersInterlude[slot].Mp}/{_charactersInterlude[slot].MaxMp}";
            _expLabel.text = _charactersInterlude[slot].ExpProcent.ToString("0.00") + "%";
            _spLabel.text = _charactersInterlude[slot].Sp.ToString();
            _karmaLabel.text = _charactersInterlude[slot].Karma.ToString();

            StartCoroutine(UpdateBarsInterlude(slot));

            _charNameManipulator.SelectIndex(slot);
        }
    }


   // public void SelectInterludeSlot(int slot)
   // {
       // if (slot == _charNameManipulator.Index)
       // {
        //    return;
        //}

       // if (slot == -1)
       // {
           // _levelLabel.text = "0";
           // _classLabel.text = "??";
           // _hpLabel.text = "0/0";
           // _mpLabel.text = "0/0";
         //   _expLabel.text = "0.00%";
          // //// _spLabel.text = "0";
           // _karmaLabel.text = "0";
            //_charNameManipulator.ClearInput();
       // }
      //  else if (slot < _characters.Count)
      //  {
           // _levelLabel.text = _characters[slot].PlayerStats.Level.ToString();
           // _classLabel.text = ((CharacterClass)(_characters[slot].ClassId)).ToString();
           // _hpLabel.text = $"{_characters[slot].PlayerStatus.Hp}/{_characters[slot].PlayerStats.MaxHp}";
           // _mpLabel.text = $"{_characters[slot].PlayerStatus.Mp}/{_characters[slot].PlayerStats.MaxMp}";
           // _expLabel.text = _characters[slot].ExpPercent.ToString("0.00") + "%";
          //  _spLabel.text = _characters[slot].Sp.ToString();
          //  _karmaLabel.text = _characters[slot].Karma.ToString();

         //   StartCoroutine(UpdateBars(slot));

        //    _charNameManipulator.SelectIndex(slot);
       // }
    //}

    IEnumerator UpdateBarsInterlude(int slot)
    {
        yield return new WaitForEndOfFrame();
        float hpRatio = _charactersInterlude[slot].MaxHp > 0
            ? (float)_charactersInterlude[slot].Hp / (float)_charactersInterlude[slot].MaxHp
            : 0f;
        float mpRatio = _charactersInterlude[slot].MaxMp > 0
            ? (float)_charactersInterlude[slot].Mp / (float)_charactersInterlude[slot].MaxMp
            : 0f;
        double maxExp = _charactersInterlude[slot].MaxExp();
        float expRatio = maxExp > 0
            ? (float)(_charactersInterlude[slot].Exp / maxExp)
            : 0f;
        ApplyBarWidths(hpRatio, mpRatio, expRatio);
    }

    IEnumerator UpdateLobbyBars(int curHp, int maxHp, int curMp, int maxMp, float expRatio)
    {
        yield return new WaitForEndOfFrame();
        float hpRatio = maxHp > 0 ? (float)curHp / maxHp : 0f;
        float mpRatio = maxMp > 0 ? (float)curMp / maxMp : 0f;
        ApplyBarWidths(hpRatio, mpRatio, Mathf.Clamp01(expRatio));
    }

    void ApplyBarWidths(float hpRatio, float mpRatio, float expRatio)
    {
        L2ResourceBarView.SetFill(_HPBar, _HPBarBG, hpRatio);
        L2ResourceBarView.SetFill(_MPBar, _MPBarBG, mpRatio);
        L2ResourceBarView.SetFill(_ExpBar, _ExpBarBG, expRatio);
    }
}
