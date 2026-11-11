public partial class ChatWindow
{
    public static bool IsGmCommand(string text)
    {
        return text != null
            && text.Length > 1
            && text[0] == '/'
            && text[1] == '/';
    }

    bool CanSend(long cooldownMs)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        if (now < _flood)
            return false;

        _flood = now + cooldownMs;
        return true;
    }
}
