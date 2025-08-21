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

    /// <summary>
    /// 构造对象池
    /// </summary>
    /// <param name="prefab">需要池化的预制体</param>
    /// <param name="initialSize">初始池大小</param>
    /// <param name="parent">可选的父物体</param>
    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;
        _poolQueue = new Queue<T>();

        for (int i = 0; i < initialSize; i++)
        {
            var obj = CreateNew();
            obj.gameObject.SetActive(false);
            _poolQueue.Enqueue(obj);
        }
    }

    /// <summary>
    /// 从池中取出对象，如果池为空则新建一个
    /// </summary>
    public T Get()
    {
        T obj = _poolQueue.Count > 0 ? _poolQueue.Dequeue() : CreateNew();
        obj.gameObject.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 回收对象，重置并放入池中
    /// </summary>
    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        _poolQueue.Enqueue(obj);
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
