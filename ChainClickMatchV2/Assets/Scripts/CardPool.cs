using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 卡牌对象池：被消除/回收的卡牌 SetActive(false) 存放复用，
/// 需要时（生成棋盘 / 掉落补新卡）取出并激活，避免频繁 Instantiate/Destroy。
/// </summary>
public class CardPool
{
    private readonly GameObject _prefab;
    private readonly Transform _parent;

    /// <summary>池内闲置（已禁用）的卡牌。</summary>
    private readonly Stack<GameObject> _inactive = new Stack<GameObject>();

    /// <summary>当前池内闲置卡牌数量。</summary>
    public int CountInactive => _inactive.Count;

    /// <summary>池对应的卡牌 prefab（用于检测 prefab 切换需重建池）。</summary>
    public GameObject Prefab => _prefab;

    public CardPool(GameObject prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }

    /// <summary>取出一张卡牌（池内有闲置则复用，否则新实例化），并激活。</summary>
    public GameObject Get()
    {
        GameObject card = _inactive.Count > 0 ? _inactive.Pop() : Object.Instantiate(_prefab, _parent);
        card.SetActive(true);
        return card;
    }

    /// <summary>回收卡牌到池内（禁用并挂回父物体下），等待复用。</summary>
    public void Release(GameObject card)
    {
        if (card == null)
        {
            return;
        }
        card.SetActive(false);
        card.transform.SetParent(_parent);
        _inactive.Push(card);
    }

    /// <summary>销毁池内全部闲置卡牌（棋盘重建等场景）。</summary>
    public void Clear()
    {
        while (_inactive.Count > 0)
        {
            GameObject card = _inactive.Pop();
            if (card == null)
            {
                continue;
            }
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Object.Destroy(card);
            }
            else
            {
                Object.DestroyImmediate(card);
            }
#else
            Object.Destroy(card);
#endif
        }
    }
}
