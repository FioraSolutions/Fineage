/// <summary>
/// Forwards enter-world success/failure to the char-select window when it is open.
/// </summary>
public static class CharSelectEnterWorld
{
    public static bool IsEntering =>
        LoginPacketServices.CharSelect != null && LoginPacketServices.CharSelect.IsEnteringWorld;

    public static void Failed(string message)
    {
        if (LoginPacketServices.CharSelect != null)
            LoginPacketServices.CharSelect.NotifyEnterWorldFailed(message);
    }

    public static void Succeeded()
    {
        if (LoginPacketServices.CharSelect != null)
            LoginPacketServices.CharSelect.NotifyEnterWorldSucceeded();
    }
}
