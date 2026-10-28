using System.Collections.Generic;

public class AbnormalStatusUpdateDto : IWireDto
{

    public List<EffectHolder> ListEffect { get; set; }
    public AbnormalStatusUpdateDto()
    {
        ListEffect = new List<EffectHolder>();
    }

    public void ReadFrom(PacketReader reader)
    {
        int size = reader.ReadSh();
        for(int i=0; i < size; i++)
        {
            
            int id = reader.ReadI();
            int value = reader.ReadSh();
            int duration = reader.ReadI();
            EffectHolder holder = new EffectHolder(id, value, duration);
            ListEffect.Add(holder);
        }
        //Debug.Log("AbnormalStatusUpdate update data");
    }

    /// <summary>Built from Zone V2 <see cref="ZoneV2MsgType.AbnormalStatusUpdate"/> (1017).</summary>
    public static AbnormalStatusUpdateDto FromZoneEffects(ZoneV2AbnormalEffect[] effects)
    {
        var dto = new AbnormalStatusUpdateDto { FullEffectSnapshot = true };
        if (effects == null)
            return dto;
        for (int i = 0; i < effects.Length; i++)
        {
            ZoneV2AbnormalEffect effect = effects[i];
            dto.ListEffect.Add(new EffectHolder(effect.Id, effect.Value, effect.DurationSeconds));
        }

        return dto;
    }

    /// <summary>Zone wire replaces the full effect list (including empty on clear).</summary>
    public bool FullEffectSnapshot { get; set; }
}

public class EffectHolder
{
    public int _id;
    public int _value;
    //sec
    public int _duration;

    public EffectHolder(int id , int value , int duration)
    {
        _id = id;
        _value = value;
        _duration = duration;
    }

    public int Id
    {
        get { return _id; }
    }

    public int Value
    {
        get { return _value; }
    }

    public int Duration
    {
        get { return _duration; }
    }
}