using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace Fineage.Net.Contract
{
    public static partial class NetCodec
    {
        public static byte[] EncodeUseSkill(UseSkillNetMsg m) => Write(w =>
        {
            w.WriteInt32(m.SkillId);
            w.WriteInt32(m.TargetObjectId);
            w.WriteByte(m.CtrlPressed ? (byte)1 : (byte)0);
        });

        public static UseSkillNetMsg DecodeUseSkill(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new UseSkillNetMsg
            {
                SkillId = r.ReadInt32(),
                TargetObjectId = r.ReadInt32(),
                CtrlPressed = r.ReadByte() != 0
            };
        }

        public static byte[] EncodeMagicSkillUse(MagicSkillUseNetMsg m) => Write(w =>
        {
            w.WriteInt32(m.CasterObjectId);
            w.WriteInt32(m.TargetObjectId);
            w.WriteInt32(m.SkillId);
            w.WriteInt32(m.SkillLevel);
            w.WriteInt32(m.HitTimeMs);
            w.WriteInt32(m.ReuseDelayMs);
        });

        public static MagicSkillUseNetMsg DecodeMagicSkillUse(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new MagicSkillUseNetMsg
            {
                CasterObjectId = r.ReadInt32(),
                TargetObjectId = r.ReadInt32(),
                SkillId = r.ReadInt32(),
                SkillLevel = r.ReadInt32(),
                HitTimeMs = r.ReadInt32(),
                ReuseDelayMs = r.ReadInt32()
            };
        }

        public static byte[] EncodeReviveRequest(ReviveRequestNetMsg m) => Write(w => w.WriteInt32(m.Action));
        public static ReviveRequestNetMsg DecodeReviveRequest(ReadOnlySpan<byte> p) =>
            new() { Action = new Reader(p).ReadInt32() };

        public static byte[] EncodeRevive(ReviveNetMsg m) => Write(w => w.WriteInt32(m.ObjectId));
        public static ReviveNetMsg DecodeRevive(ReadOnlySpan<byte> p) =>
            new() { ObjectId = new Reader(p).ReadInt32() };

        public static byte[] EncodeTeleportToLocation(TeleportToLocationNetMsg m) => Write(w =>
        {
            w.WriteInt32(m.ObjectId);
            w.WriteInt32(m.X);
            w.WriteInt32(m.Y);
            w.WriteInt32(m.Z);
            w.WriteInt32(m.Heading);
        });

        public static TeleportToLocationNetMsg DecodeTeleportToLocation(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new TeleportToLocationNetMsg
            {
                ObjectId = r.ReadInt32(),
                X = r.ReadInt32(),
                Y = r.ReadInt32(),
                Z = r.ReadInt32(),
                Heading = r.ReadInt32()
            };
        }

        public static byte[] EncodeAbnormalStatusUpdate(AbnormalStatusUpdateNetMsg m) => Write(w =>
        {
            w.WriteInt32(m.ObjectId);
            var effects = m.Effects ?? Array.Empty<AbnormalEffectNet>();
            w.WriteInt32(effects.Length);
            for (int i = 0; i < effects.Length; i++)
            {
                var e = effects[i] ?? new AbnormalEffectNet();
                w.WriteInt32(e.Id);
                w.WriteInt32(e.Value);
                w.WriteInt32(e.DurationSeconds);
                w.WriteString(e.Slot ?? "");
            }
        });

        public static AbnormalStatusUpdateNetMsg DecodeAbnormalStatusUpdate(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            int objectId = r.ReadInt32();
            int n = r.ReadInt32();
            if (n < 0)
                n = 0;
            var effects = new AbnormalEffectNet[n];
            for (int i = 0; i < n; i++)
            {
                effects[i] = new AbnormalEffectNet
                {
                    Id = r.ReadInt32(),
                    Value = r.ReadInt32(),
                    DurationSeconds = r.ReadInt32(),
                    Slot = r.ReadString()
                };
            }

            return new AbnormalStatusUpdateNetMsg { ObjectId = objectId, Effects = effects };
        }

        public static byte[] EncodeNpcTalk(NpcTalkNetMsg m) => Write(w => w.WriteInt32(m.NpcObjectId));
        public static NpcTalkNetMsg DecodeNpcTalk(ReadOnlySpan<byte> p) =>
            new() { NpcObjectId = new Reader(p).ReadInt32() };

        public static byte[] EncodeBypassRequest(BypassRequestNetMsg m) => Write(w =>
        {
            w.WriteInt32(m.NpcObjectId);
            w.WriteString(m.Command ?? "");
        });

        public static BypassRequestNetMsg DecodeBypassRequest(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new BypassRequestNetMsg { NpcObjectId = r.ReadInt32(), Command = r.ReadString() };
        }

        public static byte[] EncodeNpcHtml(NpcHtmlNetMsg m) => Write(w =>
        {
            w.WriteInt32(m.NpcObjectId);
            w.WriteInt32(m.NpcId);
            w.WriteString(m.Html ?? "");
            w.WriteInt32(m.ItemId);
        });

        public static NpcHtmlNetMsg DecodeNpcHtml(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new NpcHtmlNetMsg
            {
                NpcObjectId = r.ReadInt32(),
                NpcId = r.ReadInt32(),
                Html = r.ReadString(),
                ItemId = r.ReadInt32()
            };
        }
    }
}
