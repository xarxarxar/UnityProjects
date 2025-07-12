using System;
using System.Collections.Generic;

//可绑定数据，方便监听
public class Bindable<T>
{
    private T _value;
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
}
