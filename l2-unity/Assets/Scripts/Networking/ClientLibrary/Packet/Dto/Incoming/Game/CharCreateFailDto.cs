public class CharCreateFailDto : IWireDto
{
    private string  _text = "";
    public string Text { get { return _text; } }
    

    public void ReadFrom(PacketReader reader)
    {
        int id = reader.ReadI();
        _text = ErrorType.GetErrorText(id);
    }
}
