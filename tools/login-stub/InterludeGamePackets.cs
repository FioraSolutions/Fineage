using System.Text;
using Fineage.ZoneServer.Accounts;

namespace Fineage.LoginStub;

public static class InterludeGamePackets
{
    public const int CharCreateFailReasonTooMany = 0x01;
    public const int CharCreateFailReasonNameExists = 0x02;
    public const int CharCreateFailReasonIncorrectName = 0x04;
    public const int CharCreateFailReasonNotAllowed = 0x05;

    public sealed class CharacterCreateRequest
    {
        public string Name { get; set; } = "";
        public int Race { get; set; }
        public int Female { get; set; }
        public int ClassId { get; set; }
        public int HairStyle { get; set; }
        public int HairColor { get; set; }
        public int Face { get; set; }
    }

    public static bool TryParseCharacterCreate(ReadOnlySpan<byte> rx, out CharacterCreateRequest req)
    {
        req = new CharacterCreateRequest();
        if (rx.Length < 4 || rx[0] != 0x0B)
            return false;

        int pos = 1;
        if (pos >= rx.Length)
            return false;

        int nameByteLen = rx[pos++];
        if (nameByteLen <= 0 || pos + nameByteLen > rx.Length)
            return false;

        req.Name = Encoding.Unicode.GetString(rx.Slice(pos, nameByteLen));
        pos += nameByteLen;

        if (pos >= rx.Length || rx[pos++] != 0)
            return false;

        if (!TryReadI(rx, ref pos, out int race) ||
            !TryReadI(rx, ref pos, out int female) ||
            !TryReadI(rx, ref pos, out int classId) ||
            !TryReadI(rx, ref pos, out _) ||
            !TryReadI(rx, ref pos, out _) ||
            !TryReadI(rx, ref pos, out _) ||
            !TryReadI(rx, ref pos, out _) ||
            !TryReadI(rx, ref pos, out _) ||
            !TryReadI(rx, ref pos, out _) ||
            !TryReadI(rx, ref pos, out int hairStyle) ||
            !TryReadI(rx, ref pos, out int hairColor) ||
            !TryReadI(rx, ref pos, out int face))
            return false;

        req.Race = race;
        req.Female = female;
        req.ClassId = classId;
        req.HairStyle = hairStyle;
        req.HairColor = hairColor;
        req.Face = face;
        return true;
    }

    static bool TryReadI(ReadOnlySpan<byte> buf, ref int pos, out int value)
    {
        value = 0;
        if (pos + 4 > buf.Length)
            return false;
        value = System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(buf.Slice(pos, 4));
        pos += 4;
        return true;
    }

    public static byte[] BuildCharCreateOk() =>
        new PacketBuilder().C(0x19).D(1).ToArray();

    public static byte[] BuildCharCreateFail(int reason) =>
        new PacketBuilder().C(0x1A).D(reason).ToArray();

    public static byte[] BuildCharTemplatesHumanFighter()
    {
        var p = new PacketBuilder()
            .C(0x17)
            .D(1);
        AppendTemplate(p, HumanFighterDefaults.RaceHuman, HumanFighterDefaults.ClassFighter);
        return p.ToArray();
    }

    static void AppendTemplate(PacketBuilder p, int race, int classId)
    {
        p.D(race).D(classId);
        p.D(0x46).D(HumanFighterDefaults.BaseStr).D(0x0A);
        p.D(0x46).D(HumanFighterDefaults.BaseDex).D(0x0A);
        p.D(0x46).D(HumanFighterDefaults.BaseCon).D(0x0A);
        p.D(0x46).D(HumanFighterDefaults.BaseInt).D(0x0A);
        p.D(0x46).D(HumanFighterDefaults.BaseWit).D(0x0A);
        p.D(0x46).D(HumanFighterDefaults.BaseMen).D(0x0A);
    }

    public static byte[] BuildCharSelectionInfo(string account, IReadOnlyList<StubCharacter> characters)
    {
        var p = new PacketBuilder()
            .C(0x13)
            .D(characters.Count);

        for (int i = 0; i < characters.Count; i++)
        {
            var c = characters[i];
            bool selected = i == 0;
            AppendCharSlot(p, c, account, selected);
        }

        return p.ToArray();
    }

    static void AppendCharSlot(PacketBuilder p, StubCharacter c, string account, bool selected)
    {
        p.S(c.Name)
            .D(c.ObjectId)
            .S(account)
            .D(1)
            .D(0)
            .D(0)
            .D(c.Sex)
            .D(c.Race)
            .D(c.ClassId)
            .D(1)
            .D(0).D(0).D(0)
            .F(200.0)
            .F(50.0)
            .D(0)
            .Q(0)
            .D(1)
            .D(0);

        for (int i = 0; i < 9; i++)
            p.D(0);

        for (int i = 0; i < 17; i++)
            p.D(0);

        for (int i = 0; i < 10; i++)
            p.D(0);

        p.D(0).D(0);
        p.D(0).D(0).D(0).D(0).D(0);
        p.D(c.HairStyle).D(c.HairColor).D(c.Face);
        p.F(200.0).F(50.0);
        p.D(0);
        p.D(c.ClassId);
        p.D(selected ? 1 : 0);
        p.C(0);
        p.D(0);
    }
}
