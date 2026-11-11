public partial class TableColumn
{
    public bool _alignTextCenter;
    public string _nameColumn;
    public float _manualWidth;
    public List<string> _listData;
    public float _leftIndent;
    public int _maxTextSize;
    public bool _isImage;
    private Dictionary<string, UnityEngine.Color> _allColors;







    public bool AlignTextCenter
    {
        get { return _alignTextCenter; }
        set { _alignTextCenter = value; }
    }

    public string NameColumn
    {
        get { return _nameColumn; }
        set { _nameColumn = value; }
    }

    public bool IsImage
    {
        get { return _isImage; }
        set { _isImage = value; }
    }

    public float ManualWidth
    {
        get { return _manualWidth; }
        set { _manualWidth = value; }
    }

    public int MaxTextSize
    {
        get { return _maxTextSize; }
        set { _maxTextSize = value; }
    }

    public List<string> ListData
    {
        get { return _listData; }
        set { _listData = value; }
    }

    public float LeftIndent
    {
        get { return _leftIndent; }
        set { _leftIndent = value; }
    }







}
