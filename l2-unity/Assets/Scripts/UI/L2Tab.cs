using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class L2Tab
{
    [SerializeField] string _tabName = "Tab";
    [SerializeField] protected bool _autoscroll = true;
    protected ScrollView _scrollView;
    protected Scroller _scroller;
    private VisualElement _tabContainer;
    private VisualElement _tabHeader;
    protected VisualElement _windowEle;
    public string TabName { get { return _tabName; } }


    public VisualElement TabContainer { get { return _tabContainer; } }
    public VisualElement TabHeader { get { return _tabHeader; } }
    public Scroller Scroller { get { return _scroller; } }
    protected float _scrollStepSize = 32f;










}
