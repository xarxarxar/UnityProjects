using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffect
{
    IEnumerator Play();
    void ForceStop();   //新增,强制中断
}
public class CardEffectManager : MonoBehaviour
{

    public static CardEffectManager instance;

    private readonly List<IEffect> runningEffects = new();

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator PlayEffect(IEffect effect)
    {
        runningEffects.Add(effect);

        yield return effect.Play();

        runningEffects.Remove(effect);
    }

    /// <summary>
    /// 游戏结束 / 强制中断所有特效
    /// </summary>
    public void StopAllEffects()
    {
        foreach (var effect in runningEffects)
        {
            effect.ForceStop();
        }

        runningEffects.Clear();
    }
}
