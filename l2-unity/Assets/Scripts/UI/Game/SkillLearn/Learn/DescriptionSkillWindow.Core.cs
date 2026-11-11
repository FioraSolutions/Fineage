using System;

public partial class DescriptionSkillWindow : L2PopupWindow
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _dataProvider = new DataProviderWindows();
            WireSkillLearnPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireSkillLearnPacketServices();
    }

    private void WireSkillLearnPacketServices()
    {
        SkillLearnPacketServices.Bind(skillDesc: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/SkillLearn/Learn/DescriptionSkillWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        _content = GetElementById("Content");
        _spLabel = (Label)GetElementById("spCount");
        _backButton = (Button)GetElementById("BackButton");
        _okButton = (Button)GetElementById("LearnButton");
        _backButton = (Button)GetElementById("BackButton");
        _backButton.RegisterCallback<ClickEvent>(OnBack);
        _okButton.RegisterCallback<ClickEvent>(OnLearnSkill);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
        OnCenterScreen(_root);

    }
}
