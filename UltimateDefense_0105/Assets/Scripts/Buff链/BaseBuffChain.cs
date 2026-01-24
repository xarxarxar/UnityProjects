using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Buff链
/// </summary>
public class BuffChain
{
    public AdditionBuffChain AddChain=new AdditionBuffChain();
    public MultiBuffChain MultiChain=new MultiBuffChain();

    public void Clear()
    {
        AddChain.Clear();
        MultiChain.Clear();
    }
}

/// <summary>
/// Buff链的基类
/// </summary>
public abstract class BaseBuffChain
{
    private bool _dirty = true;
    private float _cachedValue;

    public List<BuffModifier> buffModifiers = new List<BuffModifier>();
    //最终的结果
    public float Result
    {
        get
        {
            if (_dirty)
            {
                _cachedValue = FinalResultCompute();
                _dirty = false;
            }
            return _cachedValue;
        }
    }

    /// <summary>
    /// 判断是否已有这个source的Buff
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public bool HasBuff(object source)
    {
        return buffModifiers.Exists(b => b.Source == source);
    }

    /// <summary>
    /// 添加一个buff
    /// </summary>
    /// <param name="buffModifier"></param>
    public virtual void AddBuff(BuffModifier buffModifier)
    {
        buffModifier.OnValueChanged += MarkDirty;
        buffModifiers.Add(buffModifier);
        MarkDirty();
    }

    /// <summary>
    /// 移除一个buff
    /// </summary>
    /// <param name="buffModifier"></param>
    public virtual void RemoveBuff(BuffModifier buffModifier)
    {
        if (buffModifiers.Contains(buffModifier))
        {
            buffModifiers.Remove(buffModifier);
            MarkDirty();
        }
        
    }

    /// <summary>
    /// 移除某个Source的所有Buff
    /// </summary>
    /// <param name="source"></param>
    public virtual void RemoveBuff(object source)
    {
        int removedCount = buffModifiers.RemoveAll(b => b.Source == source);
        if (removedCount > 0)
        {
            MarkDirty();
        }
    }

    /// <summary>
    /// 清空这个buff链
    /// </summary>
    public virtual void Clear()
    {
        buffModifiers.Clear();
        MarkDirty();
    }

    /// <summary>
    /// 计算最终结果
    /// </summary>
    /// <returns></returns>
    public abstract float FinalResultCompute();

    private void MarkDirty()
    {
        _dirty = true;
    }
}

/// <summary>
/// 加法Buff链
/// </summary>
public class AdditionBuffChain : BaseBuffChain
{
    public override float FinalResultCompute()
    {
        float result=0;
        foreach(BuffModifier v in buffModifiers)
        {
            result += v.Value;
        }
        return result;
    }
}

/// <summary>
/// 乘法buff链
/// </summary>
public class MultiBuffChain : BaseBuffChain 
{
    public override float FinalResultCompute()
    {
        float result = 1;
        foreach (BuffModifier v in buffModifiers)
        {
            result *= v.Value;
        }
        return result;
    }
}


/// <summary>
/// buff的识别
/// </summary>
public class BuffModifier 
{
    private float _value;
    public float Value
    {
        get => _value;
        set
        {
            _value = value;
            OnValueChanged?.Invoke();
        }
    }

    public object Source;
    public System.Action OnValueChanged;
}