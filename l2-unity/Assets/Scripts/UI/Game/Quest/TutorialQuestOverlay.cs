using Fineage.Net.Contract;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>Mobius tutorial packets: HTML panel + screen question mark.</summary>
public sealed class TutorialQuestOverlay : MonoBehaviour
{
    private static TutorialQuestOverlay _instance;
    private UIDocument _document;
    private VisualElement _root;
    private VisualElement _htmlHost;
  private VisualElement _questionMark;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            TutorialPacketServices.Bind(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    void EnsureUi()
    {
        if (_root != null)
            return;
        _document = gameObject.GetComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>();
        _root = new VisualElement { name = "TutorialOverlay" };
        _root.style.position = Position.Absolute;
        _root.style.left = 0;
        _root.style.right = 0;
        _root.style.top = 0;
        _root.style.bottom = 0;
        _root.pickingMode = PickingMode.Ignore;
        _document.rootVisualElement.Add(_root);

        _questionMark = new VisualElement { name = "TutorialQuestionMark" };
        _questionMark.style.width = 48;
        _questionMark.style.height = 48;
        _questionMark.style.position = Position.Absolute;
        _questionMark.style.left = Length.Percent(50);
        _questionMark.style.top = 120;
        var markTex = QuestNpcMarkTextureCatalog.Load(QuestNpcMarkKind.Available);
        if (markTex != null)
            _questionMark.style.backgroundImage = new StyleBackground(markTex);
        _questionMark.style.display = DisplayStyle.None;
        _root.Add(_questionMark);

        _htmlHost = new VisualElement { name = "TutorialHtml" };
        _htmlHost.style.display = DisplayStyle.None;
        _htmlHost.style.width = 320;
        _htmlHost.style.minHeight = 120;
        _htmlHost.style.position = Position.Absolute;
        _htmlHost.style.left = Length.Percent(50);
        _htmlHost.style.top = 180;
        _root.Add(_htmlHost);

    }

    public void Apply(TutorialUiNetMsg msg)
    {
        EnsureUi();
        switch (msg.Kind)
        {
            case TutorialUiKind.ShowHtml:
                if (string.IsNullOrEmpty(msg.Html))
                {
                    Debug.LogError("[Tutorial] ShowHtml with empty body.");
                    return;
                }
                HtmlWindow html = GameWindowPacketServices.Html;
                if (html == null)
                {
                    Debug.LogError("[Tutorial] HtmlWindow is not bound.");
                    return;
                }
                html.InjectToWindow(msg.Html);
                html.ShowWindow();
                break;
            case TutorialUiKind.CloseHtml:
                GameWindowPacketServices.Html?.HideWindow();
                break;
            case TutorialUiKind.QuestionMark:
                _questionMark.style.display = msg.On ? DisplayStyle.Flex : DisplayStyle.None;
                break;
            case TutorialUiKind.EnableClientEvent:
                // Client flags hook for RequestTutorialClientEvent — stored for input bridge.
                QuestTutorialClientEvents.SetEnabled(msg.EventId, msg.On);
                break;
        }
    }
}