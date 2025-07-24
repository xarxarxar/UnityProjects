using Newtonsoft.Json;
using System;
using System.Collections.Generic;

//可绑定数据，方便监听
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

    //新增构造函数
    public Bindable(T initialValue)
    {
        _value = initialValue;
    }

    //可选：无参构造函数（保留原始行为）
    public Bindable() { }
}
