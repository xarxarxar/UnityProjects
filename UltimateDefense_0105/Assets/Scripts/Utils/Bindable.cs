using Newtonsoft.Json;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Bindable<T>
{
    private T _value;

    [JsonIgnore]
    public Action<T> OnValueChanged;

    public T Value
    {
        get => _value;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(_value, value))
            {
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
    }

    public Bindable(T initialValue)
    {
        _value = initialValue;
    }

    public Bindable() { }

    //不触发事件的赋值（用于初始化或加载存档）
    public void SetSilent(T v)
    {
        _value = v;
    }

    //初始化 UI 时可立即触发一次
    public void ForceNotify()
    {
        OnValueChanged?.Invoke(_value);
    }
}
