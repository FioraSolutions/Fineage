using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMParam {
    private SMParamType _type;
    private object _value;

    public SMParamType Type { get { return _type; } set { _type = value;} }

    public SMParam(SMParamType type, object value) {
        _type = type;
        _value = value;
    }

    public SMParam(SMParamType type) {
        _type = type;
    }

    public void SetValue(object value) {
        this._value = value;
    }

    public string GetStringValue() {
        return (string)_value;
    }

    public int GetIntValue() {
        return (int)_value;
    }

    public long GetLongValue() {
        return (long)_value;
    }

    public int[] GetIntArrayValue() {
        return (int[])_value;
    }

    public float[] GetFloatArrayValue() {
        return (float[])_value;
    }

}
