using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class InitPreb
{
    public GameObject pref;
    public int amount;
}

public class ObjectPool : MonoBehaviour
{
    private static ObjectPool instance;
    public static ObjectPool Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject("ObjectPool");
                instance = gameObject.AddComponent<ObjectPool>();
                DontDestroyOnLoad(gameObject);
            }
            return instance;
        }
        private set { }
    }

    Dictionary<int, List<GameObject>> pools = new Dictionary<int, List<GameObject>>();
    private object syncRoot = new object();

    [SerializeField]
    List<InitPreb> initPrebs;

    public bool onLog;
    
    private void Awake()
    {
        if (instance != null && instance.GetInstanceID() != this.GetInstanceID())
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this as ObjectPool;
            DontDestroyOnLoad(gameObject);
        }

#if !UNITY_EDITOR
            Debug.unityLogger.logEnabled = false;
#endif

        if (initPrebs != null)
        {
            for (int i = 0; i < initPrebs.Count; i++)
            {
                var preb = initPrebs[i];
                if (preb.pref != null)
                    CreateGameObject(preb.pref, preb.amount);
            }
        }
    }

    public GameObject CreateGameObject(GameObject prefab, int amount = 30)
    {
        int type = prefab.GetInstanceID();
        if (!pools.ContainsKey(type))
        {
            pools.Add(type, new List<GameObject>());
        }
        else return null;

        var pool = pools[type];

        for (int i = 0; i < amount; i++)
        {
            var go = Instantiate(prefab);
            go.transform.parent = transform;
            go.SetActive(false);
            pool.Add(go);
        }
        return pool.FirstOrDefault();
    }

    public GameObject GetGameObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int type = prefab.GetInstanceID();
        if (!pools.ContainsKey(type))
        {
            pools.Add(type, new List<GameObject>());
        }

        var pool = pools[type];

        GameObject go = null;
        for (int k = 0; k < pool.Count; k++)
        {
            go = pool[k];
            if (go == null)
            {
                pool.Remove(go);
                continue;
            }
            if (go.activeSelf == false)
            {
                Transform goTransform = go.transform;
                goTransform.position = position;
                goTransform.rotation = rotation;

                go.SetActive(true);
                return go;
            }
        }

        Debug.LogError("create new object from prefab:" + prefab.name);

        go = Instantiate(prefab, position, rotation);
        go.transform.parent = transform;
        pool.Add(go);
        return go;
    }

    public void ReleaseObject(GameObject go)
    {
        go.transform.SetParent(transform);
        go.SetActive(false);
    }

    public void ReleaseAll()
    {
        foreach (var pool in pools)
        {
            var gameObjects = pool.Value;
            for (int k = 0; k < gameObjects.Count; k++)
            {
                if (gameObjects[k] == null)
                {
                    gameObjects.RemoveAt(k);
                    k--;
                }
                else
                {
                    ReleaseObject(gameObjects[k]);
                }
            }
        }
    }
}
