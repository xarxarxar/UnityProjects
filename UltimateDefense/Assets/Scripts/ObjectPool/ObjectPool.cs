using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通用对象池，适用于任意继承 MonoBehaviour 的类型 T
/// </summary>
public class ObjectPool<T> where T : Component
{
    private readonly T _prefab;
    private readonly Transform _parent;
    private readonly Queue<T> _poolQueue;
    private readonly HashSet<T> _inPoolSet; // 用于跟踪哪些对象在池中

    /// <summary>
    /// 构造对象池
    /// </summary>
    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;
        _poolQueue = new Queue<T>();
        _inPoolSet = new HashSet<T>();

        for (int i = 0; i < initialSize; i++)
        {
            var obj = CreateNew();
            obj.gameObject.SetActive(false);
            _poolQueue.Enqueue(obj);
            _inPoolSet.Add(obj);
        }
    }

    /// <summary>
    /// 从池中取出对象，如果池为空则新建一个
    /// </summary>
    public T Get()
    {
        T obj = _poolQueue.Count > 0 ? _poolQueue.Dequeue() : CreateNew();
        _inPoolSet.Remove(obj);
        obj.gameObject.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 回收对象，重置并放入池中
    /// </summary>
    public void Return(T obj)
    {
        if (_inPoolSet.Contains(obj))
        {
            obj.gameObject.SetActive(false);
            return;
        }
        obj.gameObject.SetActive(false);
        _poolQueue.Enqueue(obj);
        _inPoolSet.Add(obj);
    }

    /// <summary>
    /// 实例化一个新对象
    /// </summary>
    private T CreateNew()
    {
        var instance = Object.Instantiate(_prefab, _parent);
        return instance;
    }

    /// <summary>
    /// 当前池中未使用对象的数量
    /// </summary>
    public int Count => _poolQueue.Count;
}
