using System.Text;

public sealed class AuthLoginDto : IOutgoingDto
{
    public string Account;
    public int PlayKey1;
    public int PlayKey2;
    public int LoginKey1;
    public int LoginKey2;

    public void WriteTo(PacketWriter writer)
    {
        // Interlude C→S AuthLogin (matches login-stub ClientSmoke):
        // C accountLen, account ASCII, C 0, D playKey2, D playKey1, D loginKey1, D loginKey2 (LE).
        // Do NOT use WriteSOther+GameOverwriteOpcode — that overwrote the length byte with opcode
        // and produced UTF-16/BOM payloads that never matched the smoke path.
        string account = Account ?? string.Empty;
        byte[] ascii = Encoding.ASCII.GetBytes(account);
        if (ascii.Length > 255)
        {
            byte[] trimmed = new byte[255];
            System.Buffer.BlockCopy(ascii, 0, trimmed, 0, 255);
            ascii = trimmed;
        }

        writer.WriteB((byte)ascii.Length);
        writer.WriteB(ascii);
        writer.WriteB(0);
        // WriteI → LE on wire (toByteArray BE + Reverse), same as ClientSmoke D().
        writer.WriteI(PlayKey2);
        writer.WriteI(PlayKey1);
        writer.WriteI(LoginKey1);
        writer.WriteI(LoginKey2);
    }
}
