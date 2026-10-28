using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using VContainer;

public class CharSelectWindow : L2Window {
    [Inject] LoginRuntime _login;

    private CharacterSelector Characters => LoginPacketServices.Characters;

    private GameClient Game => LoginPacketServices.Game;

    private GameManager Manager => LoginPacketServices.Manager;

    private VisualTreeAsset _arrowInputTemplate;
    private ArrowInputManipulator _charNameManipulator;
    private List<CharSelectionInfoPackage> _characters;
    private List<CharSelectInfoPackage> _charactersInterlude;
    private List<LobbyCharacter> _lobbyCharacters;
    private int _lobbySelectIndex = -1;
    private bool _deleteArmed;
    private int _deleteArmedSlot = -1;
    private Button _startButton;
    private Button _deleteButton;
    private Button _createButton;
    private Label _deleteButtonLabel;
    private Label _lobbyStatus;

    private Label _levelLabel;
    private Label _classLabel;
    private Label _hpLabel;
    private Label _mpLabel;
    private Label _expLabel;
    private Label _spLabel;
    private Label _karmaLabel;

    private VisualElement _HPBar;
    private VisualElement _HPBarBG;
    private VisualElement _MPBar;
    private VisualElement _MPBarBG;
    private VisualElement _ExpBarBG;
    private VisualElement _ExpBar;
    private CharSelectInfoPackage _selectChar;

    private static CharSelectWindow _instance;
    public static CharSelectWindow Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            WireLoginPacketServices();
        } else {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireLoginPacketServices();
    }

    private void WireLoginPacketServices()
    {
        LoginPacketServices.Bind(login: _login);
    }

    private void OnDestroy() {
        _instance = null;
    }

    private void Update() {
        if (!_isWindowHidden) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                AudioManager.Instance.PlayUISound("click_01");
                ReLoginPressed();
            } else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
                AudioManager.Instance.PlayUISound("click_01");
                StartGamePressed();
            }
        }
    }

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Login/CharSelectWindow"); 
        _arrowInputTemplate = LoadAsset("Data/UI/_Elements/Template/ArrowInput");
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        _startButton = (Button) GetElementById("StartButton");
        _startButton.AddManipulator(new ButtonClickSoundManipulator(_startButton));
        _startButton.RegisterCallback<ClickEvent>(evt => StartGamePressed());

        _deleteButton = (Button)GetElementById("DeleteButton");
        _deleteButton.AddManipulator(new ButtonClickSoundManipulator(_deleteButton));
        _deleteButton.RegisterCallback<ClickEvent>(evt => DeletePressed());
        _deleteButtonLabel = _deleteButton.Q<Label>("ButtonLabel");

        _createButton = (Button)GetElementById("CreateButton");
        _createButton.AddManipulator(new ButtonClickSoundManipulator(_createButton));
        _createButton.RegisterCallback<ClickEvent>(evt => CreatePressed());

        Button reloginButton = (Button)GetElementById("ReloginButton");
        reloginButton.AddManipulator(new ButtonClickSoundManipulator(reloginButton));
        reloginButton.RegisterCallback<ClickEvent>(evt => ReLoginPressed());

        VisualElement userNameInputContainer = GetElementById("UserSelectContainer");
        VisualElement userNameInput = _arrowInputTemplate.Instantiate()[0];
        _charNameManipulator = new ArrowInputManipulator(userNameInput, "Name", new string[] { }, -1, (index, value) => {
            if (_lobbyCharacters != null)
                Characters.SelectLobbyCharacter(index);
            else
                Characters.SelectInterludeCharacter(index);
        });

        _levelLabel = (Label)GetElementById("LevelLabel");
        _classLabel = (Label)GetElementById("ClassLabel");
        _hpLabel = (Label)GetElementById("HPText");
        _mpLabel = (Label)GetElementById("MPText");
        _expLabel = (Label)GetElementById("ExpText");
        _spLabel = (Label)GetElementById("SPLabel");
        _karmaLabel = (Label)GetElementById("KarmaLabel");

        _HPBar = GetElementById("HPBar");
        _HPBarBG = GetElementById("HPBarBG");
        _MPBarBG = GetElementById("MPBarBG");
        _MPBar = GetElementById("MPBar");
        _ExpBar = GetElementById("ExpBar");
        _ExpBarBG = GetElementById("ExpBarBG");

        userNameInput.AddManipulator(_charNameManipulator);
        userNameInputContainer.Add(userNameInput);

        VisualElement detail = GetElementById("CharSelectDetailWindow");
        _lobbyStatus = new Label(string.Empty);
        _lobbyStatus.name = "LobbyStatusLabel";
        _lobbyStatus.style.whiteSpace = WhiteSpace.Normal;
        _lobbyStatus.style.marginLeft = 16;
        _lobbyStatus.style.marginRight = 12;
        _lobbyStatus.style.marginTop = 6;
        _lobbyStatus.style.fontSize = 12;
        _lobbyStatus.style.color = new Color(0.9f, 0.75f, 0.45f);
        if (detail != null)
            detail.Add(_lobbyStatus);

        SetEmptyData();
        if (_lobbyCharacters != null)
        {
            int selected = _lobbySelectIndex;
            ApplyLobbyNames();
            if (selected >= 0)
                SelectLobbySlot(selected);
        }
    }

    public void SetLobbyCharacters(IReadOnlyList<LobbyCharacter> characters)
    {
        _lobbyCharacters = characters == null
            ? new List<LobbyCharacter>()
            : new List<LobbyCharacter>(characters);
        _deleteArmed = false;
        _deleteArmedSlot = -1;
        _lobbySelectIndex = -1;
        if (_charNameManipulator == null)
            return;
        ApplyLobbyNames();
    }

    public void SelectLobbySlot(int index)
    {
        _lobbySelectIndex = index;
        if (_charNameManipulator == null || _lobbyCharacters == null)
            return;
        if (index < 0 || index >= _lobbyCharacters.Count)
        {
            SetEmptyData();
            ShowLobbyStatus(_lobbyCharacters.Count == 0 ? "No characters yet." : "");
            RefreshLobbyActions();
            return;
        }

        LobbyCharacter character = _lobbyCharacters[index];
        if (_deleteArmed && _deleteArmedSlot != character.Slot)
            _deleteArmed = false;
        _levelLabel.text = character.Level.ToString();
        _classLabel.text = LobbyCharacterText.ClassLabel(character.ClassId);
        _hpLabel.text = "-";
        _mpLabel.text = "-";
        _expLabel.text = "-";
        _spLabel.text = "-";
        _karmaLabel.text = "-";
        if (index != _charNameManipulator.Index)
            _charNameManipulator.SelectIndex(index);

        if (character.PendingDelete)
            ShowLobbyStatus("Pending deletion. " + LobbyCharacterText.DeleteCountdown(character.DeleteSecondsLeft));
        else if (_deleteArmed && _deleteArmedSlot == character.Slot)
            ShowLobbyStatus("Delete " + character.Name + "? Press Delete again to confirm.");
        else
            ShowLobbyStatus("");

        RefreshLobbyActions();
    }

    public void ShowLobbyStatus(string text)
    {
        if (_lobbyStatus != null)
            _lobbyStatus.text = text ?? "";
    }

    public void RefreshLobbyActions()
    {
        if (_lobbyCharacters == null)
            return;
        LobbyCharacter selected = SelectedLobbyCharacter();
        bool pending = selected != null && selected.PendingDelete;
        if (_startButton != null)
            _startButton.SetEnabled(LobbyCharacterText.CanEnter(selected));
        if (_deleteButton != null)
            _deleteButton.SetEnabled(selected != null);
        if (_createButton != null)
            _createButton.SetEnabled(LobbyCharacterText.CanCreate(_lobbyCharacters.Count));
        if (_deleteButtonLabel != null)
        {
            if (pending)
                _deleteButtonLabel.text = "Restore";
            else if (_deleteArmed && selected != null && _deleteArmedSlot == selected.Slot)
                _deleteButtonLabel.text = "Confirm";
            else
                _deleteButtonLabel.text = "Delete";
        }
    }

    void ApplyLobbyNames()
    {
        string[] names = new string[_lobbyCharacters.Count];
        for (int i = 0; i < _lobbyCharacters.Count; i++)
            names[i] = _lobbyCharacters[i] != null ? _lobbyCharacters[i].Name : "";
        _charNameManipulator.UpdateValues(names);
        EnableInterludeButtons();
        if (_lobbyCharacters.Count == 0)
        {
            SetEmptyData();
            ShowLobbyStatus("No characters yet.");
        }

        RefreshLobbyActions();
    }

    LobbyCharacter SelectedLobbyCharacter()
    {
        if (_lobbyCharacters == null || _lobbySelectIndex < 0 || _lobbySelectIndex >= _lobbyCharacters.Count)
            return null;
        return _lobbyCharacters[_lobbySelectIndex];
    }

    void EnableInterludeButtons()
    {
        if (_lobbyCharacters != null)
            return;
        if (_startButton != null)
            _startButton.SetEnabled(true);
        if (_deleteButton != null)
            _deleteButton.SetEnabled(true);
        if (_createButton != null)
            _createButton.SetEnabled(true);
        if (_deleteButtonLabel != null)
            _deleteButtonLabel.text = "Delete";
    }

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
            _classLabel.text = ((CharacterClass)(_charactersInterlude[slot].ClassId)).ToString();
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
        if (_HPBarBG != null && _HPBar != null)
        {
            float hpRatio = (float)_charactersInterlude[slot].Hp / (float) _charactersInterlude[slot].MaxHp;
            float bgWidth = _HPBarBG.resolvedStyle.width;
            float barWidth = bgWidth * hpRatio;
            _HPBar.style.width = barWidth;
        }

        if (_MPBarBG != null && _MPBar != null)
        {
            float mpRatio = (float)_charactersInterlude[slot].Mp / (float) _charactersInterlude[slot].MaxMp;
            float bgWidth = _MPBarBG.resolvedStyle.width;
            float barWidth = bgWidth * mpRatio;
            _MPBar.style.width = barWidth;
        }

        if (_ExpBarBG != null && _ExpBar != null)
        {
            float bgWidth = _ExpBarBG.resolvedStyle.width;
            //  float procent = (float)_charactersInterlude[slot].ExpProcent;
            float expRatio = (float)_charactersInterlude[slot].Exp / (float)_charactersInterlude[slot].MaxExp();
            float barWidth = bgWidth * expRatio;
            _ExpBar.style.width = barWidth;
        }
    }

    public CharSelectInfoPackage GetSelectChar()
    {
        return _selectChar;
    }
    private void StartGamePressed()
    {
        if (_lobbyCharacters != null)
        {
            LobbyCharacter selected = SelectedLobbyCharacter();
            if (!LobbyCharacterText.CanEnter(selected))
                return;
            if (_startButton != null)
                _startButton.SetEnabled(false);
            ZoneV2Client zone = ZoneV2Client.Instance;
            if (zone != null)
                zone.SelectLobbyCharacter(selected);
            return;
        }

        Characters.ConfirmSelection();
    }

    private void ReLoginPressed() {
        if (_lobbyCharacters != null)
        {
            ZoneV2Client zone = ZoneV2Client.Instance;
            if (zone != null)
                zone.CloseLobby();
            Manager.OnRelogin();
            return;
        }

        Manager.OnRelogin();
        Game.Disconnect();
    }

    private void CreatePressed() {
        if (_lobbyCharacters != null)
        {
            if (!LobbyCharacterText.CanCreate(_lobbyCharacters.Count))
            {
                ShowLobbyStatus(LobbyCharacterText.ErrorMessage("character_limit", null));
                return;
            }

            GameManager manager = Manager;
            if (manager != null)
                manager.OnCreateUser(null);
            return;
        }

        Game.Send(new NewCharacterCommand());
    }

    private void DeletePressed()
    {
        if (_lobbyCharacters != null)
        {
            LobbyCharacter selected = SelectedLobbyCharacter();
            if (selected == null)
                return;
            ZoneV2Client zone = ZoneV2Client.Instance;
            if (zone == null)
                return;
            if (selected.PendingDelete)
            {
                _deleteArmed = false;
                zone.RestoreLobbyCharacter(selected);
                ShowLobbyStatus("Restoring " + selected.Name + "...");
                return;
            }

            if (!_deleteArmed || _deleteArmedSlot != selected.Slot)
            {
                _deleteArmed = true;
                _deleteArmedSlot = selected.Slot;
                ShowLobbyStatus("Delete " + selected.Name + "? Press Delete again to confirm.");
                RefreshLobbyActions();
                return;
            }

            _deleteArmed = false;
            zone.DeleteLobbyCharacter(selected);
            ShowLobbyStatus("Deleting " + selected.Name + "...");
            RefreshLobbyActions();
            return;
        }

        Characters.TryToDeleteCharacter();
    }
}