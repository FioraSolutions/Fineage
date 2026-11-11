public partial class TableColumn
{
    public TableColumn(bool alignTextCenter, string nameColumn, float width, List<string> listData, float leftIndent)
    {
        _alignTextCenter = alignTextCenter;
        _nameColumn = nameColumn;
        _manualWidth = width;
        _listData = listData;
        _leftIndent = leftIndent;
        _maxTextSize = 0;
        _isImage = false;
    }

    public TableColumn(bool alignTextCenter, string nameColumn, float width, List<string> listData, float leftIndent, int maxTextSize)
    {
        _alignTextCenter = alignTextCenter;
        _nameColumn = nameColumn;
        _manualWidth = width;
        _listData = listData;
        _leftIndent = leftIndent;
        _maxTextSize = maxTextSize;
        _isImage = false;
    }

    public TableColumn(bool alignTextCenter, string nameColumn, float width, List<string> listData, float leftIndent, bool isImage)
    {
        _alignTextCenter = alignTextCenter;
        _nameColumn = nameColumn;
        _manualWidth = width;
        _listData = listData;
        _leftIndent = leftIndent;
        _maxTextSize = 0;
        _isImage = isImage;
    }

    public void SetData(List<string> listData)
    {
        if (_listData != null)
        {
            _listData.Clear();
            _listData = null;
        }
        _listData = listData;
    }
}
