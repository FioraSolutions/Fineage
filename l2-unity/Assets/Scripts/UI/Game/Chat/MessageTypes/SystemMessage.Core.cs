using System;
using UnityEngine;
using static SMParam;

public partial class SystemMessage
{
    public SystemMessage(SMParam[] smParams, SystemMessageDat messageData)
    {
        _params = smParams;
        _messageData = messageData;
    }

    public override string ToString()
    {
        string value = String.Copy(_messageData.Message);
        ProcessMessageParameters(ref value);
        return $"<color=#{_messageData.Color}>{value}</color>";
    }

    private void ProcessMessageParameters(ref string value)
    {
        for (int i = 1; i <= _params.Length; i++)
        {
            SMParam param = _params[i - 1];
            string replacement = GetParameterReplacement(param, i);

            if (replacement != null)
            {
                value = ReplacePlaceholders(value, replacement, i);
            }
        }
    }
}
