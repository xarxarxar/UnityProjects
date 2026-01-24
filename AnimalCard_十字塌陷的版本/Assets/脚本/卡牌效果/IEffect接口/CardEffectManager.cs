using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffect
{
    IEnumerator Play();
}
public class CardEffectManager : MonoBehaviour
{

    public static CardEffectManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void PlayEffect(IEffect effect)
    {
        StartCoroutine(effect.Play());
    }
}
